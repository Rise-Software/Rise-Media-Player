using Rise.App.ChangeTrackers;
using Rise.Common;
using Rise.Common.Constants;
using Rise.Common.Extensions;
using Rise.Common.Extensions.Markup;
using Rise.Common.Helpers;
using Rise.Data.Json;
using Rise.Data.Messages;
using Rise.Data.ViewModels;
using Rise.Models;
using Rise.NewRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using TagLib.Ape;
using Windows.Storage;
using Windows.Storage.FileProperties;

namespace Rise.App.ViewModels
{
    public sealed class MainViewModel : ViewModel
    {
        public event EventHandler IndexingStarted;
        public event EventHandler MetadataFetchingStarted;
        public event EventHandler<IndexingFinishedEventArgs> IndexingFinished;

        private bool _isScanning;

        /// <summary>
        /// Whether the app is currently looking for new media.
        /// </summary>
        public bool IsScanning
        {
            get => _isScanning;
            private set => Set(ref _isScanning, value);
        }

        private uint _indexedMedia = 0;
        
        /// <summary>
        /// The media indexed so far.
        /// </summary>
        public uint IndexedMedia
        {
            get => _indexedMedia;
            set => Set(ref _indexedMedia, value);
        }

        private uint _indexedSongs = 0;
        private uint _indexedVideos = 0;

        // Amount of indexed items. These are used to provide data to the
        // IndexingFinished event.
        private uint IndexedSongs
        {
            get => _indexedSongs;
            set => Set(ref _indexedSongs, value);
        }

        private uint IndexedVideos
        {
            get => _indexedVideos;
            set => Set(ref _indexedVideos, value);
        }

        /// <summary>
        /// Helps cancel indexing related Tasks.
        /// </summary>
        private readonly CancellableTaskHelper IndexingCancelHelper = new();

        /// <summary>
        /// The collection of songs in the list. 
        /// </summary>
        public readonly SafeObservableCollection<SongViewModel> Songs = new();

        /// <summary>
        /// The collection of albums in the list. 
        /// </summary>
        public readonly SafeObservableCollection<AlbumViewModel> Albums = new();

        /// <summary>
        /// The collection of artists in the list. 
        /// </summary>
        public readonly SafeObservableCollection<ArtistViewModel> Artists = new();

        /// <summary>
        /// The collection of genres in the list. 
        /// </summary>
        public readonly SafeObservableCollection<GenreViewModel> Genres = new();

        /// <summary>
        /// The collection of videos in the list. 
        /// </summary>
        public readonly SafeObservableCollection<VideoViewModel> Videos = new();

        private JsonBackendController<PlaylistViewModel> _pBackend;
        public JsonBackendController<PlaylistViewModel> PBackend
            => _pBackend ??= JsonBackendController<PlaylistViewModel>.Get("SavedPlaylists");
        public SafeObservableCollection<PlaylistViewModel> Playlists => PBackend.Items;

        private JsonBackendController<BasicNotification> _nBackend;
        public JsonBackendController<BasicNotification> NBackend
            => _nBackend ??= JsonBackendController<BasicNotification>.Get("Messages");

        /// <summary>
        /// Gets the complete list of data from the database.
        /// </summary>
        public async Task GetListsAsync()
        {
            var songs = await Repository.GetItemsAsync<Song>();

            // If we have no songs, we have no albums, artists or genres
            if (songs != null)
            {
                foreach (var item in songs)
                    Songs.Add(new(item));

                var albums = await Repository.GetItemsAsync<Album>();
                if (albums != null)
                {
                    foreach (var item in albums)
                        Albums.Add(new(item));
                }

                var artists = await Repository.GetItemsAsync<Artist>();
                if (artists != null)
                {
                    foreach (var item in artists)
                        Artists.Add(new(item));
                }

                var genres = await Repository.GetItemsAsync<Genre>();
                if (genres != null)
                {
                    foreach (var item in genres)
                        Genres.Add(new(item));
                }
            }

            var videos = await Repository.GetItemsAsync<Video>();
            if (videos != null)
            {
                foreach (var item in videos)
                    Videos.Add(new(item));
            }
        }

        public async Task StartFullCrawlAsync()
        {
            try
            {
                await StartFullCrawlAsync(new CancellationToken());
            }
            catch (OperationCanceledException) { }
        }

        public Task StartFullCrawlAsync(CancellationToken token)
        {
            return IndexingCancelHelper.CompletePendingAsync(token)
                .ContinueWith(async _ => await IndexingCancelHelper.RunAsync(StartFullCrawlImpl(IndexingCancelHelper.Token)));
        }

        private async Task StartFullCrawlImpl(CancellationToken token)
        {
            IsScanning = true;
            IndexingStarted?.Invoke(this, EventArgs.Empty);

            await IndexLibrariesAsync(token).ConfigureAwait(false);
            token.ThrowIfCancellationRequested();

            await Task.WhenAll(
                SongsTracker.CheckDuplicatesAsync(),
                VideosTracker.CheckDuplicatesAsync()
            );

            await Task.WhenAll(
                SongsTracker.HandleLibraryChangesAsync(true),
                VideosTracker.HandleLibraryChangesAsync(true)
            );

            await Repository.UpsertQueuedAsync();
            await Repository.DeleteQueuedAsync();

            if (App.SViewModel.FetchOnlineData)
            {
                MetadataFetchingStarted?.Invoke(this, EventArgs.Empty);
                await FetchArtistsArtAsync(token);
            }

            IndexingFinished?.Invoke(this, new(IndexedSongs, IndexedVideos));
            IsScanning = false;

            IndexedSongs = 0;
            IndexedVideos = 0;
            IndexedMedia = 0;
        }

        private async Task IndexLibrariesAsync(CancellationToken token)
        {
            var songsTask = Task.Run(async () =>
            {
                await foreach (var song in App.MusicLibrary.IndexAsync(QueryPresets.SongQueryOptions,
                    PropertyPrefetchOptions.MusicProperties, SongProperties.DiscProperties))
                {
                    if (await SaveMusicModelsAsync(song, true).ConfigureAwait(false))
                        IndexedSongs++;

                    IndexedMedia++;
                }
            }, token);

            var videosTask = Task.Run(async () =>
            {
                await foreach (var video in App.VideoLibrary.IndexAsync(QueryPresets.VideoQueryOptions,
                    PropertyPrefetchOptions.VideoProperties))
                {
                    if (await SaveVideoModelAsync(video, true).ConfigureAwait(false))
                        IndexedVideos++;

                    IndexedMedia++;
                }
            }, token);

            await Task.WhenAll(songsTask, videosTask);
        }

        public async Task FetchArtistsArtAsync(CancellationToken token = default)
        {
            using var wc = new HttpClient(new HttpClientHandler()
            {
                Proxy = null,
                UseProxy = false
            });

            if (!WebHelpers.IsInternetAccessAvailable())
                return;

            foreach (var artist in Artists)
            {
                if (token != null && token.IsCancellationRequested)
                    return;

                // The ms-appx prefix is used for files within the app
                // bundle, and if it isn't present, it means a custom
                // image has already been applied
                if (!artist.Picture.StartsWith("ms-appx"))
                    return;

                if (!WebHelpers.IsInternetAccessAvailable())
                    return;

                string pic = await GetArtistImageAsync(artist.Name, wc).ConfigureAwait(false);
                if (!string.IsNullOrEmpty(pic))
                    artist.Picture = pic;
            }
        }

        /// <summary>
        /// Saves a song to the repository and ViewModel.
        /// </summary>
        /// <param name="file">Song file to add.</param>
        /// <returns>true if the song didn't exist beforehand,
        /// otherwise false.</returns>
        public async Task<bool> SaveMusicModelsAsync(StorageFile file, bool queue = false)
        {
            var song = await Song.GetFromFileAsync(file);

            // Check if song exists.
            bool songExists = Songs.
                Any(s => s.Model.Equals(song));

            // Check if album exists.
            bool albumExists = Albums.
                Any(a => a.Model.Title == song.Album);

            // Check if artist exists.
            bool artistExists = Artists.
                Any(a => a.Model.Name == song.Artist);

            // Check if genre exists.
            bool genreExists = Genres.
                Any(g => g.Model.Name == song.Genres);

            var albumArtist = song.AlbumArtist.ReplaceIfNullOrWhiteSpace(song.Artist);

            List<Task> tasks = new();

            // If album isn't there already, add it to the database.
            if (!albumExists)
            {
                // Set AlbumViewModel data.
                AlbumViewModel alvm = new()
                {
                    Title = song.Album,
                    Artist = albumArtist,
                    Genres = song.Genres,
                    Thumbnail = song.Thumbnail,
                    Year = song.Year
                };

                // Add new data to the MViewModel.
                tasks.Add(alvm.SaveAsync(queue));
            }
            else
            {
                AlbumViewModel alvm = Albums.FirstOrDefault(a => a.Model.Title == song.Album);

                // Update album information, in case previous songs don't have it
                // and the album is known.
                if (alvm.Model.Title != "UnknownAlbumResource")
                {
                    bool save = false;
                    if (alvm.Model.Artist == "UnknownArtistResource")
                    {
                        alvm.Model.Artist = albumArtist;
                        save = true;
                    }

                    if (alvm.Thumbnail == URIs.AlbumThumb)
                    {
                        alvm.Thumbnail = song.Thumbnail;
                        save = true;
                    }

                    if (alvm.Year == 0)
                    {
                        alvm.Year = song.Year;
                        save = true;
                    }

                    if (save)
                        tasks.Add(alvm.SaveAsync(queue));
                }

                song.Thumbnail ??= alvm.Thumbnail;
            }

            // If artist isn't there already, add it to the database.
            if (!artistExists)
            {
                ArtistViewModel arvm = new()
                {
                    Name = song.Artist,
                    Picture = URIs.ArtistThumb
                };

                tasks.Add(arvm.SaveAsync(queue));
            }

            // Check for the album artist as well.
            artistExists = Artists.
                Any(a => a.Model.Name == song.Artist || a.Model.Name == song.AlbumArtist);

            // If album artist isn't there already, add it to the database.
            if (!artistExists)
            {
                ArtistViewModel arvm = new()
                {
                    Name = song.AlbumArtist,
                    Picture = URIs.ArtistThumb
                };

                tasks.Add(arvm.SaveAsync(queue));
            }

            // If genre isn't there already, add it to the database.
            if (!genreExists)
            {
                GenreViewModel gvm = new()
                {
                    Name = song.Genres
                };

                tasks.Add(gvm.SaveAsync());
            }

            // If song isn't there already, add it to the database
            if (!songExists)
            {
                SongViewModel svm = new(song);
                tasks.Add(svm.SaveAsync(queue));
            }

            await Task.WhenAll(tasks);

            return !songExists;
        }

        public async Task<string> GetArtistImageAsync(string artist, HttpClient wc)
        {
            if (artist != ResourceHelper.GetString("UnknownArtistResource"))
            {
                try
                {
                    string m_strFilePath = URLs.Deezer + "/search/artist/?q=" + artist + "&output=xml";

                    wc ??= new HttpClient(new HttpClientHandler()
                    {
                        Proxy = null,
                        UseProxy = false
                    });

                    string xmlStr = await wc.GetStringAsync(m_strFilePath);

                    var xmlDoc = new XmlDocument();
                    xmlDoc.LoadXml(xmlStr);

                    var node = xmlDoc.DocumentElement.SelectSingleNode("/root/data/artist/picture_medium");
                    if (node != null)
                        return node.InnerText.Replace("<![CDATA[ ", string.Empty).Replace(" ]]>", string.Empty);
                }
                catch { }
            }

            return string.Empty;
        }

        /// <summary>
        /// Saves a video to the repository and ViewModel.
        /// </summary>
        /// <param name="file">Video file to add.</param>
        /// <returns>true if the video didn't exist beforehand,
        /// otherwise false.</returns>
        public async Task<bool> SaveVideoModelAsync(StorageFile file, bool queue = false)
        {
            var video = await Video.GetFromFileAsync(file);

            bool videoExists = Videos.
                Any(v => v.Model.Equals(video));

            if (!videoExists)
            {
                VideoViewModel vid = new(video);

                // Get song thumbnail and make a PNG out of it.
                using StorageItemThumbnail thumbnail = await file.GetThumbnailAsync(ThumbnailMode.VideosView, 238);

                string filename = vid.Title.AsValidFileName();
                bool result = await thumbnail.SaveToFileAsync($@"{filename}.png");

                vid.Thumbnail = result ? $@"ms-appdata:///local/{filename}.png" : URIs.AlbumThumb;

                await vid.SaveAsync(queue);
            }

            return !videoExists;
        }
    }

    public class IndexingFinishedEventArgs : EventArgs
    {
        public uint IndexedSongs { get; private set; }
        public uint IndexedVideos { get; private set; }

        public IndexingFinishedEventArgs(uint songs, uint videos)
        {
            IndexedSongs = songs;
            IndexedVideos = videos;
        }
    }
}
