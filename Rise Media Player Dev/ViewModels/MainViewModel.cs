using Microsoft.Toolkit.Uwp.UI;
using Rise.Models;
using RMP.App.ChangeTrackers;
using RMP.App.Common;
using RMP.App.Indexing;
using RMP.App.Props;
using RMP.App.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Search;

namespace RMP.App.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        // private readonly DispatcherQueue dispatcherQueue = DispatcherQueue.GetForCurrentThread();
        public Indexer Indexer => App.Indexer;

        /// <summary>
        /// Whether or not are we currently indexing. This is to avoid
        /// unnecessarily indexing concurrently.
        /// </summary>
        public bool IsIndexing = false;

        /// <summary>
        /// Whether or not is there a recrawl queued. If true,
        /// <see cref="IndexSongsAsync"/> will call itself when necessary.
        /// </summary>
        public bool QueuedReindex = false;

        /// <summary>
        /// Whether or not can indexing start. This is set to true once
        /// <see cref="MainPage"/> loads up, at which point indexing starts.
        /// </summary>
        public bool CanIndex = false;

        /// <summary>
        /// Creates a new MainViewModel.
        /// </summary>
        public MainViewModel()
        {
            FilteredSongs = new AdvancedCollectionView(Songs);
            FilteredAlbums = new AdvancedCollectionView(Albums);
            FilteredArtists = new AdvancedCollectionView(Artists);
            FilteredGenres = new AdvancedCollectionView(Genres);

            _ = Task.Run(GetListsAsync);
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
            bool skip = false;
            IsLoading = true;

            IEnumerable<Song> songs = await App.Repository.Songs.GetAsync();

            // If there are no songs, don't bother loading lists
            if (songs == null)
            {
                skip = true;
            }

            if (!skip)
            {
                IEnumerable<Album> albums = await App.Repository.Albums.GetAsync();
                IEnumerable<Artist> artists = await App.Repository.Artists.GetAsync();
                IEnumerable<Genre> genres = await App.Repository.Genres.GetAsync();

                Songs.Clear();
                foreach (Song s in songs)
                {
                    if (!s.Removed)
                    {
                        Songs.Add(new SongViewModel(s));
                    }
                }

                Albums.Clear();
                if (albums != null)
                {
                    foreach (Album a in albums)
                    {
                        if (!a.Removed)
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
                        if (!a.Removed)
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
                        Genres.Add(new GenreViewModel(g));
                    }
                }
                IsLoading = false;
            }

            await KnownFolders.MusicLibrary.
                TrackForegroundAsync(QueryPresets.SongQueryOptions,
                SongsTracker.MusicQueryResultChanged);

            while (!CanIndex)
            {
                await Task.Delay(1000);
            }

            await StartFullCrawlAsync();
        }

        public async Task StartFullCrawlAsync()
        {
            await App.RefreshMusicLibrary();
            await SongsTracker.HandleMusicFolderChanges(App.MusicFolders);
            await IndexSongsAsync();

            await App.Repository.Songs.UpsertQueuedAsync();
            await App.Repository.Albums.UpsertQueuedAsync();
            await App.Repository.Artists.UpsertQueuedAsync();
            await App.Repository.Genres.UpsertQueuedAsync();
        }

        public async Task IndexSongsAsync()
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
                SongFileIndexed,
                IndexerOption.UseIndexerWhenAvailable,
                PropertyPrefetchOptions.MusicProperties,
                Properties.DiscProperties);

            if (QueuedReindex)
            {
                QueuedReindex = false;
                await IndexSongsAsync();
            }

            IsIndexing = false;
        }

        private async Task SongFileIndexed(StorageFile file)
        {
            Song song = await file.AsSongModelAsync();
            await SaveModelsAsync(song, file);
        }

        /// <summary>
        /// Saves a song to the repository and ViewModel.
        /// </summary>
        /// <param name="song">Song to add.</param>
        /// <param name="file">The song file.</param>
        public async Task SaveModelsAsync(Song song, StorageFile file)
        {
            // Check if song exists.
            bool songExists = Songs.
                Any(s => s.Model.Equals(song));

            // Check if album exists.
            bool albumExists = Albums.
                Any(a => a.Model.Title == song.Album &&
                    a.Model.Artist == song.AlbumArtist);

            // Check if artist exists.
            bool artistExists = Artists.
                Any(a => a.Model.Name == song.Artist);

            // Check if genre exists.
            bool genreExists = Genres.
                Any(g => g.Model.Name == song.Genres);

            // If song isn't there already, add it to the database
            if (!songExists)
            {
                SongViewModel svm = new SongViewModel(song);
                await svm.SaveAsync();
            }

            // If album isn't there already, add it to the database.
            if (!albumExists)
            {
                string thumb = "ms-appx:///Assets/Default.png";

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
                }

                // Set AlbumViewModel data.
                AlbumViewModel alvm = new AlbumViewModel
                {
                    Title = song.Album,
                    Artist = song.AlbumArtist,
                    Genres = song.Genres,
                    Thumbnail = thumb
                };

                // Add new data to the MViewModel.
                alvm.Save();
            }
            else
            {
                AlbumViewModel alvm = Albums.
                    First(a => a.Model.Title == song.Album &&
                               a.Model.Artist == song.AlbumArtist);

                // Update album information, in case previous songs don't have it
                // and the album is known.
                if (alvm.Model.Title != "UnknownAlbumResource")
                {
                    if (alvm.Model.Artist == "UnknownArtistResource")
                    {
                        alvm.Model.Artist = song.AlbumArtist;
                    }

                    if (alvm.Thumbnail == "ms-appx:///Assets/Default.png")
                    {
                        // Get song thumbnail and make a PNG out of it.
                        StorageItemThumbnail thumbnail = await file.GetThumbnailAsync(ThumbnailMode.MusicView, 134);

                        string filename = song.Album.AsValidFileName();
                        filename = await FileHelpers.SaveBitmapFromThumbnailAsync(thumbnail, $@"{filename}.png");

                        if (filename != "/")
                        {
                            alvm.Thumbnail = $@"ms-appdata:///local/{filename}.png";
                        }
                    }
                }
            }

            // If artist isn't there already, add it to the database.
            if (!artistExists)
            {
                ArtistViewModel arvm = new ArtistViewModel
                {
                    Name = song.Artist,
                    Picture = "ms-appx:///Assets/Default.png"
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
                    Picture = "ms-appx:///Assets/Default.png"
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
        }

        /// <summary>
        /// Saves any modified data and reloads the data lists from the database.
        /// </summary>
        public void Sync()
        {
            _ = Task.Run(async () =>
            {
                IsLoading = true;
                foreach (SongViewModel modifiedSong in Songs
                    .Where(song => song.IsModified))
                {
                    if (modifiedSong.Removed)
                    {
                        await App.Repository.Songs.DeleteAsync(modifiedSong.Model);
                    }
                    else
                    {
                        await App.Repository.Songs.QueueUpsertAsync(modifiedSong.Model);
                    }
                }

                foreach (AlbumViewModel modifiedAlbum in Albums
                    .Where(album => album.IsModified))
                {
                    if (modifiedAlbum.Removed)
                    {
                        await App.Repository.Albums.DeleteAsync(modifiedAlbum.Model);
                    }
                    else
                    {
                        await App.Repository.Albums.QueueUpsertAsync(modifiedAlbum.Model);
                    }
                }

                foreach (ArtistViewModel modifiedArtist in Artists
                    .Where(artist => artist.IsModified))
                {
                    if (modifiedArtist.Removed)
                    {
                        await App.Repository.Artists.DeleteAsync(modifiedArtist.Model);
                    }
                    else
                    {
                        await App.Repository.Artists.QueueUpsertAsync(modifiedArtist.Model);
                    }
                }

                foreach (GenreViewModel modifiedGenre in Genres
                    .Where(genre => genre.IsModified))
                {
                    if (modifiedGenre.WillRemove)
                    {
                        await App.Repository.Genres.DeleteAsync(modifiedGenre.Model);
                    }
                    else
                    {
                        await App.Repository.Genres.QueueUpsertAsync(modifiedGenre.Model);
                    }
                }

                await GetListsAsync();
                IsLoading = false;
            });
        }
    }
}
