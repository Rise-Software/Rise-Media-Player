using Microsoft.Toolkit.Uwp.UI;
using Rise.App.ChangeTrackers;
using Rise.App.Views;
using Rise.Common;
using Rise.Common.Constants;
using Rise.Common.Extensions;
using Rise.Data.ViewModels;
using Rise.Models;
using Rise.Repository.SQL;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Xml;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Search;

namespace Rise.App.ViewModels
{
    public class MainViewModel : ViewModel
    {
        private XmlDocument xmlDoc = new();
        private List<string> imagelinks = new();
        #region Events
        public event EventHandler IndexingStarted;
        public event EventHandler<IndexingFinishedEventArgs> IndexingFinished;
        #endregion

        #region Fields
        // Amount of indexed items. These are used to provide data to the
        // IndexingFinished event.
        private uint IndexedSongs = 0;
        private uint IndexedVideos = 0;

        /// <summary>
        /// Whether or not are we currently indexing. This is to avoid
        /// unnecessarily indexing concurrently.
        /// </summary>
        private bool IsIndexing = false;

        /// <summary>
        /// Whether or not is there a recrawl queued. If true,
        /// <see cref="IndexLibrariesAsync"/> will call itself when necessary.
        /// </summary>
        private bool QueuedReindex = false;

        /// <summary>
        /// Whether or not can indexing start. This is set to true once
        /// <see cref="MainPage"/> loads up, at which point running from the
        /// UI thread is possible.
        /// </summary>
        public bool CanIndex = false;

        /// <summary>
        /// Whether or not the search bar is focused.
        /// </summary>
        public bool IsSearchActive = false;
        #endregion

        /// <summary>
        /// Creates a new MainViewModel.
        /// </summary>
        public MainViewModel()
        {
            FilteredSongs = new AdvancedCollectionView(Songs);
            FilteredPlaylists = new AdvancedCollectionView(Playlists);
            FilteredAlbums = new AdvancedCollectionView(Albums);
            FilteredArtists = new AdvancedCollectionView(Artists);
            FilteredGenres = new AdvancedCollectionView(Genres);
            FilteredVideos = new AdvancedCollectionView(Videos);
            FilteredNotifications = new AdvancedCollectionView(Notifications);

            QueryPresets.SongQueryOptions.
                SetThumbnailPrefetch(ThumbnailMode.MusicView, 134, ThumbnailOptions.None);
            QueryPresets.SongQueryOptions.
                IndexerOption = IndexerOption.UseIndexerWhenAvailable;

            QueryPresets.VideoQueryOptions.
                SetThumbnailPrefetch(ThumbnailMode.VideosView, 238, ThumbnailOptions.None);
            QueryPresets.VideoQueryOptions.
                IndexerOption = IndexerOption.UseIndexerWhenAvailable;
        }

        /// <summary>
        /// The collection of songs in the list. 
        /// </summary>
        public ObservableCollection<SongViewModel> Songs { get; set; }
            = new ObservableCollection<SongViewModel>();
        public AdvancedCollectionView FilteredSongs { get; set; }

        /// <summary>
        /// The collection of albums in the list. 
        /// </summary>
        public ObservableCollection<AlbumViewModel> Albums { get; set; }
            = new ObservableCollection<AlbumViewModel>();
        public AdvancedCollectionView FilteredAlbums { get; set; }

        /// <summary>
        /// The collection of artists in the list. 
        /// </summary>
        public ObservableCollection<ArtistViewModel> Artists { get; set; }
            = new ObservableCollection<ArtistViewModel>();
        public AdvancedCollectionView FilteredArtists { get; set; }

        /// <summary>
        /// The collection of genres in the list. 
        /// </summary>
        public ObservableCollection<GenreViewModel> Genres { get; set; }
            = new ObservableCollection<GenreViewModel>();
        public AdvancedCollectionView FilteredGenres { get; set; }

        /// <summary>
        /// The collection of videos in the list. 
        /// </summary>
        public ObservableCollection<VideoViewModel> Videos { get; set; }
            = new ObservableCollection<VideoViewModel>();
        public AdvancedCollectionView FilteredVideos { get; set; }

        /// <summary>
        /// The collection of playlists in the list. 
        /// </summary>
        public ObservableCollection<PlaylistViewModel> Playlists { get; set; }
            = new ObservableCollection<PlaylistViewModel>();
        public AdvancedCollectionView FilteredPlaylists { get; set; }

        /// <summary>
        /// The collection of playlists in the list. 
        /// </summary>
        public ObservableCollection<NotificationViewModel> Notifications { get; set; }
            = new ObservableCollection<NotificationViewModel>();
        public AdvancedCollectionView FilteredNotifications { get; set; }

        private SongViewModel _selectedSong;
        /// <summary>
        /// Gets or sets the currently selected song.
        /// </summary>
        public SongViewModel SelectedSong
        {
            get => _selectedSong;
            set => Set(ref _selectedSong, value);
        }

        private VideoViewModel _selectedVideo;
        /// <summary>
        /// Gets or sets the currently selected video.
        /// </summary>
        public VideoViewModel SelectedVideo
        {
            get => _selectedVideo;
            set => Set(ref _selectedVideo, value);
        }

        /// <summary>
        /// Gets the complete list of data from the database.
        /// </summary>
        public async Task GetListsAsync()
        {
            IEnumerable<Song> songs = (await SQLRepository.Repository.Songs.GetAsync()).Distinct();

            if (songs != null)
            {
                IEnumerable<Album> albums = (await SQLRepository.Repository.Albums.GetAsync()).Distinct();
                IEnumerable<Artist> artists = (await SQLRepository.Repository.Artists.GetAsync()).Distinct();
                IEnumerable<Genre> genres = (await SQLRepository.Repository.Genres.GetAsync()).Distinct();
                IEnumerable<Video> videos = (await SQLRepository.Repository.Videos.GetAsync()).Distinct();

                ObservableCollection<PlaylistViewModel> playlists = await App.PBackendController.GetAsync();
                ObservableCollection<NotificationViewModel> notifications = await App.NBackendController.GetAsync();

                Songs.Clear();
                foreach (var item in songs)
                {
                    Songs.Add(new(item));
                }

                Albums.Clear();
                if (albums != null)
                {
                    foreach (var item in albums)
                    {
                        Albums.Add(new(item));
                    }
                }

                Artists.Clear();
                if (artists != null)
                {
                    foreach (var item in artists)
                    {
                        Artists.Add(new(item));
                    }
                }

                Genres.Clear();
                if (genres != null)
                {
                    foreach (var item in genres)
                    {
                        Genres.Add(new(item));
                    }
                }

                Videos.Clear();
                if (videos != null)
                {
                    foreach (var item in videos)
                    {
                        Videos.Add(new(item));
                    }
                }

                Playlists.Clear();
                if (playlists != null)
                {
                    foreach (var item in playlists)
                    {
                        Playlists.Add(item);
                    }
                }

                Notifications.Clear();
                if (notifications != null)
                {
                    foreach (var item in notifications)
                    {
                        Notifications.Add(item);
                    }
                }
            }
        }

        public async Task StartFullCrawlAsync()
        {
            // There are no direct callers to IndexLibrariesAsync outside of
            // this function, so we can just check here
            if (!CanIndex)
            {
                return;
            }

            IndexingStarted?.Invoke(this, EventArgs.Empty);

            await SongsTracker.HandleMusicFolderChanges();
            await VideosTracker.HandleVideosFolderChanges();
            await IndexLibrariesAsync();
            await SyncAsync();

            IndexingFinished?.Invoke(this, new(IndexedSongs, IndexedVideos));

            IndexedSongs = 0;
            IndexedVideos = 0;
        }

        private async Task IndexLibrariesAsync()
        {
            if (IsIndexing && QueuedReindex)
            {
                // If we're indexing and a reindex is queued,
                // don't bother doing anything
                return;
            }
            else if (IsIndexing && !QueuedReindex)
            {
                // If we're indexing and a reindex isn't queued,
                // queue a reindex and return
                QueuedReindex = true;
                return;
            }
            else if (!IsIndexing && QueuedReindex)
            {
                // If we're not indexing and a reindex is queued,
                // allow adding a reindex to the queue and continue
                QueuedReindex = false;
            }

            IsIndexing = true;
            await foreach (var song in App.MusicLibrary.IndexAsync(QueryPresets.SongQueryOptions,
                PropertyPrefetchOptions.MusicProperties, SongProperties.DiscProperties))
            {
                if (await SaveMusicModelsAsync(song))
                {
                    IndexedSongs++;
                }
            }

            await foreach (var video in App.VideoLibrary.IndexAsync(QueryPresets.VideoQueryOptions,
                PropertyPrefetchOptions.VideoProperties))
            {
                if (await SaveVideoModelAsync(video))
                {
                    IndexedVideos++;
                }
            }

            IsIndexing = false;
            if (QueuedReindex)
            {
                await IndexLibrariesAsync();
            }
        }

        /// <summary>
        /// Saves a song to the repository and ViewModel.
        /// </summary>
        /// <param name="file">Song file to add.</param>
        /// <returns>true if the song didn't exist beforehand,
        /// otherwise false.</returns>
        public async Task<bool> SaveMusicModelsAsync(StorageFile file)
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

            // If album isn't there already, add it to the database.
            if (!albumExists)
            {
                string thumb = URIs.AlbumThumb;

                // If the album is unknown, no need to get a thumbnail.
                if (song.Album != "UnknownAlbumResource")
                {
                    // Get song thumbnail and make a PNG out of it.
                    StorageItemThumbnail thumbnail = await file.GetThumbnailAsync(ThumbnailMode.MusicView, 200);

                    string filename = song.Album.AsValidFileName();
                    filename = await thumbnail.SaveToFileAsync($@"{filename}.png");

                    if (filename != "/")
                    {
                        thumb = $@"ms-appdata:///local/{filename}.png";
                    }

                    thumbnail?.Dispose();
                }

                // Set AlbumViewModel data.
                AlbumViewModel alvm = new()
                {
                    Title = song.Album,
                    Artist = song.AlbumArtist,
                    Genres = song.Genres,
                    Thumbnail = thumb,
                    Year = song.Year
                };

                song.Thumbnail = thumb;

                // Add new data to the MViewModel.
                await alvm.SaveAsync();
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

                    if (alvm.Thumbnail == URIs.MusicThumb)
                    {
                        // Get song thumbnail and make a PNG out of it.
                        StorageItemThumbnail thumbnail = await file.GetThumbnailAsync(ThumbnailMode.MusicView, 134);

                        string filename = song.Album.AsValidFileName();
                        filename = await thumbnail.SaveToFileAsync($@"{filename}.png");

                        if (filename != "/")
                        {
                            alvm.Thumbnail = $@"ms-appdata:///local/{filename}.png";
                            save = true;
                        }

                        thumbnail?.Dispose();
                    }

                    if (alvm.Year == 0)
                    {
                        alvm.Year = song.Year;
                        save = true;
                    }

                    if (save)
                    {
                        await alvm.SaveAsync();
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
                string thumb = URIs.ArtistThumb;
                string image;
                image = await Task.Run(() => getartistimg(song.Artist));
                imagelinks.Add(song.Artist + " - " + image);
                foreach (string imagel in imagelinks)
                {
                    if (imagel.Contains(song.Artist))
                    {
                        thumb = imagel.Replace(song.Artist + " - ", "");
                    }
                }
                ArtistViewModel arvm = new();
                arvm.Name = song.Artist;
                arvm.Picture = thumb;

                await arvm.SaveAsync();
            }

            // Check for the album artist as well.
            artistExists = Artists.
                Any(a => a.Model.Name == song.Artist);

            // If album artist isn't there already, add it to the database.
            if (!artistExists)
            {
                string thumb = URIs.ArtistThumb;
                string image;
                image = await Task.Run(() => getartistimg(song.Artist));
                imagelinks.Add(song.Artist + " - " + image);
                foreach (string imagel in imagelinks)
                {
                    if (imagel.Contains(song.Artist))
                    {
                        thumb = imagel.Replace(song.Artist + " - ", "");
                    }
                }
                ArtistViewModel arvm = new();
                arvm.Name = song.Artist;
                arvm.Picture = thumb;

                await arvm.SaveAsync();
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
                await svm.SaveAsync();
            }

            return !songExists;
        }
        public string getartistimg(string artist)
        {
            try
            {
                string m_strFilePath = URLs.Deezer + "/search/artist/?q=" + artist + "&output=xml";
                string xmlStr;
                WebClient wc = new();
                xmlStr = wc.DownloadString(m_strFilePath);
                xmlDoc.LoadXml(xmlStr);

                XmlNode node = xmlDoc.DocumentElement.SelectSingleNode("/root/data/artist/picture_medium");
                if (node != null)
                {
                    string yes = node.InnerText.Replace("<![CDATA[ ", "").Replace(" ]]>", "");
                    return yes;
                }
            }
            catch (Exception)
            {

            }
            return URIs.ArtistThumb;
        }

        /// <summary>
        /// Saves a video to the repository and ViewModel.
        /// </summary>
        /// <param name="file">Video file to add.</param>
        /// <returns>true if the video didn't exist beforehand,
        /// otherwise false.</returns>
        public async Task<bool> SaveVideoModelAsync(StorageFile file)
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
                filename = await thumbnail.SaveToFileAsync($@"{filename}.png");

                if (filename != "/")
                {
                    vid.Thumbnail = $@"ms-appdata:///local/{filename}.png";
                }
                else
                {
                    vid.Thumbnail = URIs.MusicThumb;
                }

                thumbnail?.Dispose();
                await vid.SaveAsync();
            }

            return !videoExists;
        }

        /// <summary>
        /// Upserts and removes all queued items.
        /// </summary>
        public async Task UpdateItemsAsync()
        {
            await SQLRepository.Repository.Songs.UpsertQueuedAsync();
            await SQLRepository.Repository.Albums.UpsertQueuedAsync();
            await SQLRepository.Repository.Artists.UpsertQueuedAsync();
            await SQLRepository.Repository.Genres.UpsertQueuedAsync();
            await SQLRepository.Repository.Videos.UpsertQueuedAsync();

            await SQLRepository.Repository.Songs.DeleteQueuedAsync();
            await SQLRepository.Repository.Albums.DeleteQueuedAsync();
            await SQLRepository.Repository.Artists.DeleteQueuedAsync();
            await SQLRepository.Repository.Genres.DeleteQueuedAsync();
            await SQLRepository.Repository.Videos.DeleteQueuedAsync();
        }

        /// <summary>
        /// Saves any modified data and reloads the data lists from the database.
        /// </summary>
        public async Task SyncAsync()
        {
            await UpdateItemsAsync();
            await GetListsAsync();
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
