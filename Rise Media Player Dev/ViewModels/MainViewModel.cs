using Rise.App.ChangeTrackers;
using Rise.Common;
using Rise.Common.Constants;
using Rise.Common.Extensions;
using Rise.Common.Helpers;
using Rise.Data.ViewModels;
using Rise.Models;
using Rise.NewRepository;
using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Rise.App.ViewModels
{
    public class MainViewModel : ViewModel
    {
        public event EventHandler IndexingStarted;
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

        // Amount of indexed items. These are used to provide data to the
        // IndexingFinished event.
        private uint IndexedSongs = 0;
        private uint IndexedVideos = 0;

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

        /// <summary>
        /// The collection of playlists in the list. 
        /// </summary>
        public readonly SafeObservableCollection<PlaylistViewModel> Playlists = new();

        /// <summary>
        /// The collection of playlists in the list. 
        /// </summary>
        public readonly SafeObservableCollection<NotificationViewModel> Notifications = new();

        /// <summary>
        /// Gets the complete list of data from the database.
        /// </summary>
        public async Task GetListsAsync()
        {
            // Clear the collections
            Songs.Clear();
            Albums.Clear();
            Artists.Clear();
            Genres.Clear();

            Videos.Clear();
            Playlists.Clear();
            Notifications.Clear();

            var songs = await Repository.GetItemsAsync<Song>();

            // If we have no songs, we have no albums, artists or genres
            if (songs != null)
            {
                foreach (var item in songs)
                {
                    Songs.Add(new(item));
                }

                var albums = await Repository.GetItemsAsync<Album>();
                if (albums != null)
                {
                    foreach (var item in albums)
                    {
                        Albums.Add(new(item));
                    }
                }

                var artists = await Repository.GetItemsAsync<Artist>();
                if (artists != null)
                {
                    foreach (var item in artists)
                    {
                        Artists.Add(new(item));
                    }
                }

                var genres = await Repository.GetItemsAsync<Genre>();
                if (genres != null)
                {
                    foreach (var item in genres)
                    {
                        Genres.Add(new(item));
                    }
                }
            }

            var videos = await Repository.GetItemsAsync<Video>();
            if (videos != null)
            {
                foreach (var item in videos)
                {
                    Videos.Add(new(item));
                }
            }

            // Playlists may contain songs or videos
            if (songs != null || videos != null)
            {
                var playlists = await App.PBackendController.GetAsync();
                if (playlists != null)
                {
                    foreach (var item in playlists)
                    {
                        Playlists.Add(item);
                    }
                }
            }

            var notifications = await App.NBackendController.GetAsync();
            if (notifications != null)
            {
                foreach (var item in notifications)
                {
                    Notifications.Add(item);
                }
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

        public async Task StartFullCrawlAsync(CancellationToken token)
        {
            await IndexingCancelHelper.CompletePendingAsync(token);
            await IndexingCancelHelper.RunAsync(StartFullCrawlImpl(IndexingCancelHelper.Token));
        }

        private async Task StartFullCrawlImpl(CancellationToken token)
        {
            IsScanning = true;
            IndexingStarted?.Invoke(this, EventArgs.Empty);

            await IndexLibrariesAsync(token).ConfigureAwait(false);
            token.ThrowIfCancellationRequested();

            if (App.SViewModel.FetchOnlineData)
                _ = FetchArtistsArtAsync(token);

            _ = SongsTracker.HandleMusicFolderChangesAsync(token);
            _ = VideosTracker.HandleVideosFolderChangesAsync(token);

            IndexingFinished?.Invoke(this, new(IndexedSongs, IndexedVideos));
            IsScanning = false;

            IndexedSongs = 0;
            IndexedVideos = 0;
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
                }
            }, token);

            var videosTask = Task.Run(async () =>
            {
                await foreach (var video in App.VideoLibrary.IndexAsync(QueryPresets.VideoQueryOptions,
                    PropertyPrefetchOptions.VideoProperties))
                {
                    if (await SaveVideoModelAsync(video, true).ConfigureAwait(false))
                        IndexedVideos++;
                }
            }, token);

            await Task.WhenAll(songsTask, videosTask);

            await Repository.UpsertQueuedAsync();
        }

        private async Task FetchArtistsArtAsync(CancellationToken token)
        {
            foreach (var artist in Artists)
            {
                if (token.IsCancellationRequested)
                    return;

                artist.Picture = await Task.Run(() => GetArtistImage(artist.Name));
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
                Any(s => s.Model.Equals(song) || s.Location == file.Path);

            // Check if album exists.
            bool albumExists = Albums.
                Any(a => a.Model.Title == song.Album);

            // Check if artist exists.
            bool artistExists = Artists.
                Any(a => a.Model.Name == song.Artist);

            // Check if genre exists.
            bool genreExists = Genres.
                Any(g => g.Model.Name == song.Genres);

            // If album isn't there already, add it to the database.
            if (!albumExists)
            {
                // Set AlbumViewModel data.
                AlbumViewModel alvm = new()
                {
                    Title = song.Album,
                    Artist = song.AlbumArtist,
                    Genres = song.Genres,
                    Thumbnail = song.Thumbnail,
                    Year = song.Year
                };

                // Add new data to the MViewModel.
                await alvm.SaveAsync(queue);
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
                        alvm.Model.Artist = song.AlbumArtist;
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
                    {
                        await alvm.SaveAsync(queue);
                    }
                }

                if (song.Thumbnail == null)
                {
                    song.Thumbnail = alvm.Thumbnail;
                }
            }

            // If artist isn't there already, add it to the database.
            if (!artistExists)
            {
                ArtistViewModel arvm = new()
                {
                    Name = song.Artist,
                    Picture = URIs.ArtistThumb
                };

                await arvm.SaveAsync(queue);
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

                await arvm.SaveAsync(queue);
            }

            // If genre isn't there already, add it to the database.
            if (!genreExists)
            {
                GenreViewModel gvm = new()
                {
                    Name = song.Genres
                };

                await gvm.SaveAsync();
            }

            // If song isn't there already, add it to the database
            if (!songExists)
            {
                SongViewModel svm = new(song);
                await svm.SaveAsync(queue);
            }

            return !songExists;
        }

        public string GetArtistImage(string artist)
        {
            if (App.SViewModel.FetchOnlineData && artist != "Unknown Artist")
            {
                try
                {
                    string m_strFilePath = URLs.Deezer + "/search/artist/?q=" + artist + "&output=xml";

                    using var wc = new WebClient();
                    string xmlStr = wc.DownloadString(m_strFilePath);

                    var xmlDoc = new XmlDocument();
                    xmlDoc.LoadXml(xmlStr);

                    var node = xmlDoc.DocumentElement.SelectSingleNode("/root/data/artist/picture_medium");
                    if (node != null)
                        return node.InnerText.Replace("<![CDATA[ ", string.Empty).Replace(" ]]>", string.Empty);
                }
                catch (Exception)
                {

                }
            }

            return URIs.ArtistThumb;
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
                StorageItemThumbnail thumbnail = await file.GetThumbnailAsync(ThumbnailMode.VideosView, 238);

                string filename = vid.Title.AsValidFileName();
                bool result = await thumbnail.SaveToFileAsync($@"{filename}.png");

                if (result)
                {
                    vid.Thumbnail = $@"ms-appdata:///local/{filename}.png";
                }
                else
                {
                    vid.Thumbnail = URIs.AlbumThumb;
                }

                thumbnail?.Dispose();
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
