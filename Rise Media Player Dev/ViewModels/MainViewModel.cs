using Microsoft.Toolkit.Uwp.UI;
using Rise.App.ChangeTrackers;
using Rise.App.Common;
using Rise.App.Indexing;
using Rise.App.Props;
using Rise.App.Views;
using Rise.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Search;

namespace Rise.App.ViewModels
{
    public class MainViewModel : ViewModel
    {
        private IndexingHelper Indexer => App.Indexer;

        private bool _isIndexing = false;
        /// <summary>
        /// Whether or not are we currently indexing. This is to avoid
        /// unnecessarily indexing concurrently.
        /// </summary>
        public bool IsIndexing
        {
            get => _isIndexing;
            set => Set(ref _isIndexing, value);
        }

        /// <summary>
        /// Whether or not is there a recrawl queued. If true,
        /// <see cref="IndexLibrariesAsync"/> will call itself when necessary.
        /// </summary>
        public bool QueuedReindex = false;

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

            QueryPresets.VideoQueryOptions.
                SetThumbnailPrefetch(ThumbnailMode.VideosView, 238, ThumbnailOptions.None);
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

        private bool _isLoading = false;
        /// <summary>
        /// Gets or sets a value indicating whether the lists are currently being updated. 
        /// </summary>
        public bool IsLoading
        {
            get => _isLoading;
            set => Set(ref _isLoading, value);
        }

        /// <summary>
        /// Gets the complete list of data from the database.
        /// </summary>
        public async Task GetListsAsync()
        {
            IsLoading = true;

            IEnumerable<Song> songs = await App.Repository.Songs.GetAsync();

            if (songs != null)
            {
                IEnumerable<Album> albums = await App.Repository.Albums.GetAsync();
                IEnumerable<Artist> artists = await App.Repository.Artists.GetAsync();
                IEnumerable<Genre> genres = await App.Repository.Genres.GetAsync();
                IEnumerable<Video> videos = await App.Repository.Videos.GetAsync();
                ObservableCollection<PlaylistViewModel> playlists = await App.PBackendController.GetAsync();
                ObservableCollection<NotificationViewModel> notifications = await App.NBackendController.GetAsync();

                Songs.Clear();
                foreach (Song s in songs)
                {
                    if(!songs.Contains(s))
                    {
                        Songs.Add(new SongViewModel(s));
                    }
                }

                Albums.Clear();
                if (albums != null)
                {
                    foreach (Album a in albums)
                    {
                        if(!albums.Contains(a))
                        {
                            Albums.Add(new AlbumViewModel(a));
                        }
                    }
                }

                Artists.Clear();
                if (artists != null)
                {
                    foreach (Artist a in artists)
                    {
                        if(!artists.Contains(a))
                        {
                            Artists.Add(new ArtistViewModel(a));
                        }
                    }
                }

                Genres.Clear();
                if (genres != null)
                {
                    foreach (Genre g in genres)
                    {
                        if(!genres.Contains(g))
                        {
                            Genres.Add(new GenreViewModel(g));
                        }
                    }
                }

                Videos.Clear();
                if (videos != null)
                {
                    foreach (Video v in videos)
                    {
                        if(!videos.Contains(v))
                        {
                            Videos.Add(new VideoViewModel(v));
                        }
                    }
                }

                Playlists.Clear();
                if (playlists != null)
                {
                    foreach (PlaylistViewModel p in playlists)
                    {
                         Playlists.Add(p);
                    }
                }

                Notifications.Clear();
                if (notifications != null)
                {
                    foreach (NotificationViewModel n in notifications)
                    {
                        Notifications.Add(n);
                    }
                }
            }
        }
        public async Task StartFullCrawlAsync()
        {
            await SongsTracker.HandleMusicFolderChanges();
            await IndexLibrariesAsync();
            await UpdateItemsAsync();
        }

        public async Task IndexLibrariesAsync()
        {
            if (!CanIndex)
            {
                return;
            }

            if (IsIndexing)
            {
                QueuedReindex = true;
                return;
            }

            IsIndexing = true;

            Indexer.CancelTask();
            await Indexer.IndexLibraryAsync(App.MusicLibrary,
                QueryPresets.SongQueryOptions,
                Indexer.Token,
                SaveMusicModelsAsync,
                IndexerOption.UseIndexerWhenAvailable,
                PropertyPrefetchOptions.MusicProperties,
                Properties.DiscProperties);

            await Indexer.IndexLibraryAsync(App.VideoLibrary,
                QueryPresets.VideoQueryOptions,
                Indexer.Token,
                SaveVideoModelAsync,
                IndexerOption.UseIndexerWhenAvailable,
                PropertyPrefetchOptions.VideoProperties);

            if (QueuedReindex)
            {
                QueuedReindex = false;
                await IndexLibrariesAsync();
            }

            IsIndexing = false;
        }

        /// <summary>
        /// Saves a song to the repository and ViewModel.
        /// </summary>
        /// <param name="file">Song file to add.</param>
        public async Task SaveMusicModelsAsync(StorageFile file)
        {
            Song song = await file.AsSongModelAsync();

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
                string thumb = Resources.MusicThumb;

                // If the album is unknown, no need to get a thumbnail.
                if (song.Album != "UnknownAlbumResource")
                {
                    // Get song thumbnail and make a PNG out of it.
                    StorageItemThumbnail thumbnail = await file.GetThumbnailAsync(ThumbnailMode.MusicView, 200);

                    string filename = song.Album.AsValidFileName();
                    filename = await FileHelpers.SaveBitmapFromThumbnailAsync(thumbnail, $@"{filename}.png");

                    if (filename != "/")
                    {
                        thumb = $@"ms-appdata:///local/{filename}.png";
                    }

                    thumbnail.Dispose();
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
                AlbumViewModel alvm = Albums.
                    First(a => a.Model.Title == song.Album);

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

                    if (alvm.Thumbnail == Resources.MusicThumb)
                    {
                        // Get song thumbnail and make a PNG out of it.
                        StorageItemThumbnail thumbnail = await file.GetThumbnailAsync(ThumbnailMode.MusicView, 134);

                        string filename = song.Album.AsValidFileName();
                        filename = await FileHelpers.SaveBitmapFromThumbnailAsync(thumbnail, $@"{filename}.png");

                        if (filename != "/")
                        {
                            alvm.Thumbnail = $@"ms-appdata:///local/{filename}.png";
                            save = true;
                        }

                        thumbnail.Dispose();
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

                song.Thumbnail = alvm.Thumbnail;
            }

            // If artist isn't there already, add it to the database.
            if (!artistExists)
            {
                ArtistViewModel arvm = new ArtistViewModel
                {
                    Name = song.Artist,
                    Picture = Resources.MusicThumb
                };

                await arvm.SaveAsync();
            }

            // Check for the album artist as well.
            artistExists = Artists.
                Any(a => a.Model.Name == song.Artist);

            // If album artist isn't there already, add it to the database.
            if (!artistExists)
            {
                ArtistViewModel arvm = new ArtistViewModel
                {
                    Name = song.AlbumArtist,
                    Picture = Resources.MusicThumb
                };

                await arvm.SaveAsync();
            }

            // If genre isn't there already, add it to the database.
            if (!genreExists)
            {
                GenreViewModel gvm = new GenreViewModel
                {
                    Name = song.Genres
                };

                await gvm.SaveAsync();
            }

            // If song isn't there already, add it to the database
            if (!songExists)
            {
                SongViewModel svm = new SongViewModel(song);
                await svm.SaveAsync();
            }
        }

        /// <summary>
        /// Saves a video to the repository and ViewModel.
        /// </summary>
        /// <param name="file">Video file to add.</param>
        public async Task SaveVideoModelAsync(StorageFile file)
        {
            Video video = await file.AsVideoModelAsync();

            bool videoExists = Videos.
                Any(v => v.Model.Equals(video));

            if (!videoExists)
            {
                VideoViewModel vid = new(video);

                // Get song thumbnail and make a PNG out of it.
                StorageItemThumbnail thumbnail = await file.GetThumbnailAsync(ThumbnailMode.VideosView, 238);

                string filename = vid.Title.AsValidFileName();
                filename = await FileHelpers.SaveBitmapFromThumbnailAsync(thumbnail, $@"{filename}.png");

                if (filename != "/")
                {
                    vid.Thumbnail = $@"ms-appdata:///local/{filename}.png";
                }
                else
                {
                    vid.Thumbnail = Resources.MusicThumb;
                }

                thumbnail?.Dispose();
                await vid.SaveAsync();
            }
        }

        /// <summary>
        /// Upserts and removes all queued items.
        /// </summary>
        public async Task UpdateItemsAsync()
        {
            await App.Repository.Songs.UpsertQueuedAsync();
            await App.Repository.Albums.UpsertQueuedAsync();
            await App.Repository.Artists.UpsertQueuedAsync();
            await App.Repository.Genres.UpsertQueuedAsync();
            await App.Repository.Videos.UpsertQueuedAsync();

            await App.Repository.Songs.DeleteQueuedAsync();
            await App.Repository.Albums.DeleteQueuedAsync();
            await App.Repository.Artists.DeleteQueuedAsync();
            await App.Repository.Genres.DeleteQueuedAsync();
            await App.Repository.Videos.DeleteQueuedAsync();
        }

        /// <summary>
        /// Saves any modified data and reloads the data lists from the database.
        /// </summary>
        public async Task SyncAsync()
        {
            IsLoading = true;

            await GetListsAsync();
            IsLoading = false;
        }
    }
}
