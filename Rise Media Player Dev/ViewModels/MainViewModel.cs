using Rise.Models;
using RMP.App.ChangeTrackers;
using RMP.App.Indexers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using static RMP.App.Enums;

namespace RMP.App.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        // private readonly DispatcherQueue dispatcherQueue = DispatcherQueue.GetForCurrentThread();

        /// <summary>
        /// Creates a new MainViewModel.
        /// </summary>
        public MainViewModel()
        {
            _ = Task.Run(GetListsAsync);
        }

        /// <summary>
        /// The collection of songs in the list. 
        /// </summary>
        public ObservableCollection<SongViewModel> Songs { get; set; }
            = new ObservableCollection<SongViewModel>();

        /// <summary>
        /// The collection of albums in the list. 
        /// </summary>
        public ObservableCollection<AlbumViewModel> Albums { get; set; }
            = new ObservableCollection<AlbumViewModel>();

        private AlbumViewModel _selectedAlbum;

        /// <summary>
        /// Gets or sets the selected album, or null if no album is selected. 
        /// </summary>
        public AlbumViewModel SelectedAlbum
        {
            get => _selectedAlbum;
            set => Set(ref _selectedAlbum, value);
        }

        /// <summary>
        /// The collection of albums in the list. 
        /// </summary>
        public ObservableCollection<ArtistViewModel> Artists { get; set; }
            = new ObservableCollection<ArtistViewModel>();

        private ArtistViewModel _selectedArtist;

        /// <summary>
        /// Gets or sets the selected album, or null if no album is selected. 
        /// </summary>
        public ArtistViewModel SelectedArtist
        {
            get => _selectedArtist;
            set => Set(ref _selectedArtist, value);
        }

        /// <summary>
        /// Gets a list of filtered songs.
        /// </summary>
        public ObservableCollection<SongViewModel> FilteredSongs => FilterSongs();

        /// <summary>
        /// Gets or sets the strings to filter by for the filtered songs list.
        /// In order: Title, Album, Album Artist, Artist.
        /// </summary>
        public string[] Filters { get; set; }
            = new string[4] { "", "", "", "" };

        /// <summary>
        /// Gets or sets the bools that indicates whether or not
        /// to filter based on equality. In order: Title, Album,
        /// Album Artist, Artist.
        /// </summary>
        public bool[] StrictFilters { get; set; }
            = new bool[4] { false, false, false, false };

        /// <summary>
        /// Gets or sets the string that indicates how the
        /// sorting must be done.
        /// </summary>
        public SortMethods OrderBy { get; set; }

        /// <summary>
        /// Filters the song collection based on various filters.
        /// </summary>
        /// <returns>A filtered collection with songs.</returns>
        public ObservableCollection<SongViewModel> FilterSongs()
        {
            // Make sure the strings have values. If they
            // don't, we can't compare based on equality.
            for (int i = 0; i < Filters.Length; i++)
            {
                if (Filters[i] == "")
                {
                    StrictFilters[i] = false;
                }
            }

            IEnumerable<SongViewModel> enumerable;
            if (App.SViewModel.FilterByNameOnly)
            {
                enumerable =
                    from s in Songs
                    where (StrictFilters[0] ? s.Model.Title == Filters[0] : s.Model.Title.Contains(Filters[0]))
                       && (StrictFilters[1] ? s.Model.Album == Filters[1] : s.Model.Album.Contains(Filters[1]))
                       && (StrictFilters[3] ? s.Model.Artist == Filters[3] : s.Model.Artist.Contains(Filters[3]))
                    select s;
            }
            else
            {
                enumerable =
                    from s in Songs
                    where (StrictFilters[0] ? s.Model.Title == Filters[0] : s.Model.Title.Contains(Filters[0]))
                       && (StrictFilters[1] ? s.Model.Album == Filters[1] : s.Model.Album.Contains(Filters[1]))
                       && (StrictFilters[2] ? s.Model.AlbumArtist == Filters[2] : s.Model.AlbumArtist.Contains(Filters[2]))
                       && (StrictFilters[3] ? s.Model.Artist == Filters[3] : s.Model.Artist.Contains(Filters[3]))
                    select s;
            }

            switch (OrderBy)
            {
                case SortMethods.Title:
                    enumerable = enumerable.OrderBy(s => s.Title);
                    break;

                case SortMethods.Album:
                    enumerable = enumerable.OrderBy(s => s.Album);
                    break;

                case SortMethods.Artist:
                    enumerable = enumerable.OrderBy(s => s.Artist);
                    break;

                case SortMethods.AlbumArtist:
                    enumerable = enumerable.OrderBy(s => s.AlbumArtist);
                    break;

                case SortMethods.Random:
                    Random rng = new Random();
                    enumerable = enumerable.OrderBy(a => rng.Next());
                    break;

                default:
                    enumerable = enumerable.OrderBy(s => s.Disc).ThenBy(s => s.Track);
                    break;
            }

            return new ObservableCollection<SongViewModel>(enumerable);
        }

        /// <summary>
        /// Clears the currently applied filters.
        /// </summary>
        public void ClearFilters()
        {
            Filters = new string[4] { "", "", "", "" };
            StrictFilters = new bool[4] { false, false, false, false };
            OrderBy = SortMethods.Default;
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
            // _ = await dispatcherQueue.EnqueueAsync(() => IsLoading = true);

            IEnumerable<Song> songs = await App.Repository.Songs.GetAsync();

            // If there are no songs, don't bother loading lists
            if (songs == null)
            {
                await SongsTracker.SetupMusicTracker();
                await SongIndexer.IndexAllSongsAsync();
                await SongsTracker.HandleMusicFolderChanges(await KnownFolders.MusicLibrary.GetFoldersAsync());
                return;
            }

            IEnumerable<Album> albums = await App.Repository.Albums.GetAsync();
            IEnumerable<Artist> artists = await App.Repository.Artists.GetAsync();

            Songs.Clear();
            foreach (Song s in songs)
            {
                Songs.Add(new SongViewModel(s));
            }

            Albums.Clear();
            if (albums != null)
            {
                foreach (Album a in albums)
                {
                    Albums.Add(new AlbumViewModel(a));
                }
            }

            Artists.Clear();
            if (artists != null)
            {
                foreach (Artist a in artists)
                {
                    Artists.Add(new ArtistViewModel(a));
                }
            }

            IsLoading = false;

            await SongsTracker.SetupMusicTracker();
            await SongIndexer.IndexAllSongsAsync();
            await SongsTracker.HandleMusicFolderChanges(await KnownFolders.MusicLibrary.GetFoldersAsync());
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
                    if (modifiedSong.WillRemove)
                    {
                        await App.Repository.Songs.DeleteAsync(modifiedSong.Model);
                    }
                    else
                    {
                        await App.Repository.Songs.UpsertAsync(modifiedSong.Model);
                    }
                }

                foreach (AlbumViewModel modifiedAlbum in Albums
                    .Where(album => album.IsModified))
                {
                    if (modifiedAlbum.WillRemove)
                    {
                        await App.Repository.Albums.DeleteAsync(modifiedAlbum.Model);
                    }
                    else
                    {
                        await App.Repository.Albums.UpsertAsync(modifiedAlbum.Model);
                    }
                }

                foreach (ArtistViewModel modifiedArtist in Artists
                    .Where(artist => artist.IsModified))
                {
                    if (modifiedArtist.WillRemove)
                    {
                        await App.Repository.Artists.DeleteAsync(modifiedArtist.Model);
                    }
                    else
                    {
                        await App.Repository.Artists.UpsertAsync(modifiedArtist.Model);
                    }
                }

                await GetListsAsync();
                IsLoading = false;
            });
        }
    }
}
