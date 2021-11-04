using Microsoft.Toolkit.Uwp.UI;
using Rise.Models;
using RMP.App.ChangeTrackers;
using RMP.App.Indexers;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

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
            IsLoading = true;
            IEnumerable<Song> songs = await App.Repository.Songs.GetAsync();

            // If there are no songs, don't bother loading lists
            if (songs == null)
            {
                await SongsTracker.SetupMusicTracker();
                await SongIndexer.IndexAllSongsAsync();
                await SongsTracker.HandleMusicFolderChanges(App.MusicFolders);
                return;
            }

            IEnumerable<Album> albums = await App.Repository.Albums.GetAsync();
            IEnumerable<Artist> artists = await App.Repository.Artists.GetAsync();
            IEnumerable<Genre> genres = await App.Repository.Genres.GetAsync();

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

            Genres.Clear();
            if (genres != null)
            {
                foreach (Genre g in genres)
                {
                    Genres.Add(new GenreViewModel(g));
                }
            }

            IsLoading = false;

            await SongsTracker.SetupMusicTracker();
            await SongIndexer.IndexAllSongsAsync();
            await SongsTracker.HandleMusicFolderChanges(App.MusicFolders);
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

                foreach (GenreViewModel modifiedGenre in Genres
                    .Where(genre => genre.IsModified))
                {
                    if (modifiedGenre.WillRemove)
                    {
                        await App.Repository.Genres.DeleteAsync(modifiedGenre.Model);
                    }
                    else
                    {
                        await App.Repository.Genres.UpsertAsync(modifiedGenre.Model);
                    }
                }

                await GetListsAsync();
                IsLoading = false;
            });
        }
    }
}
