using Rise.Models;
using RMP.App.ChangeTrackers;
using RMP.App.Indexers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using static RMP.App.Common.Enums;

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

        /// <summary>
        /// The collection of artists in the list. 
        /// </summary>
        public ObservableCollection<ArtistViewModel> Artists { get; set; }
            = new ObservableCollection<ArtistViewModel>();

        /// <summary>
        /// The collection of genres in the list. 
        /// </summary>
        public ObservableCollection<GenreViewModel> Genres { get; set; }
            = new ObservableCollection<GenreViewModel>();

        /// <summary>
        /// Gets a list of filtered songs.
        /// </summary>
        public ObservableCollection<SongViewModel> FilteredSongs { get; set; }
            = new ObservableCollection<SongViewModel>();

        /// <summary>
        /// Filters a song collection based on the provided album.
        /// </summary>
        /// <returns>Enumerable with songs from the specified album.</returns>
        public IEnumerable<SongViewModel> SongsFromAlbum(AlbumViewModel album,
            ObservableCollection<SongViewModel> songs, bool merge, bool strictFiltering = true)
        {
            IEnumerable<SongViewModel> enumerable;
            if (merge)
            {
                if (strictFiltering)
                {
                    enumerable = songs.Where(s => s.Model.Album == album.Model.Title);
                }
                else
                {
                    enumerable = songs.Where(s => s.Model.Album.Contains(album.Model.Title));
                }
            }
            else
            {
                if (strictFiltering)
                {
                    enumerable = songs.Where(s => s.Model.Album == album.Model.Title
                    && s.Model.AlbumArtist == album.Model.Artist);
                }
                else
                {
                    enumerable = songs.Where(s => s.Model.Album.Contains(album.Model.Title)
                    && s.Model.AlbumArtist.Contains(album.Model.Artist));
                }
            }

            return enumerable;
        }

        /// <summary>
        /// Filters a song collection based on the provided artist.
        /// </summary>
        /// <returns>Enumerable with songs from the specified artist.</returns>
        public IEnumerable<SongViewModel> SongsFromArtist(ArtistViewModel artist,
            ObservableCollection<SongViewModel> songs, bool strictFiltering = true)
        {
            IEnumerable<SongViewModel> enumerable;
            if (strictFiltering)
            {
                enumerable = songs.Where(s => s.Model.Artist == artist.Model.Name);
            }
            else
            {
                enumerable = songs.Where(s => s.Model.Artist.Contains(artist.Model.Name));
            }

            return enumerable;
        }

        /// <summary>
        /// Sorts a list of songs.
        /// </summary>
        /// <param name="list">List to sort.</param>
        /// <param name="method">Preferred sorting method.</param>
        /// <returns>An IOrderedEnumerable with the sorted songs.</returns>
        public IEnumerable<SongViewModel> SortSongs(IEnumerable<SongViewModel> list,
            SortMethods method, bool descending = false)
        {
            Debug.WriteLine("Sorting...");
            IEnumerable<SongViewModel> songs;

            switch (method)
            {
                case SortMethods.Title:
                    songs = list.OrderBy(s => s.Title);
                    break;

                case SortMethods.Album:
                    songs = list.OrderBy(s => s.Album);
                    break;

                case SortMethods.AlbumArtist:
                    songs = list.OrderBy(s => s.AlbumArtist);
                    break;

                case SortMethods.Artist:
                    songs = list.OrderBy(s => s.Artist);
                    break;

                case SortMethods.Genre:
                    songs = list.OrderBy(s => s.Genres);
                    break;

                case SortMethods.Year:
                    songs = list.OrderBy(s => s.Year);
                    break;

                case SortMethods.Random:
                    Random rng = new Random();
                    songs = list.OrderBy(s => rng.Next());
                    break;

                default:
                    songs = list.OrderBy(s => s.Disc).ThenBy(s => s.Track);
                    break;
            }

            if (descending)
            {
                songs = songs.Reverse();
            }

            return songs;
        }

        /// <summary>
        /// Sorts a list of albums.
        /// </summary>
        /// <param name="list">List to sort.</param>
        /// <param name="method">Preferred sorting method.</param>
        /// <returns>An IOrderedEnumerable with the sorted albums.</returns>
        public IEnumerable<AlbumViewModel> SortAlbums(IEnumerable<AlbumViewModel> list,
            SortMethods method, bool merge, bool descending)
        {
            Debug.WriteLine("Sorting...");
            IEnumerable<AlbumViewModel> albums;

            if (merge)
            {
                albums = list.GroupBy(a => a.Title).Select(a => a.First());
            }
            else
            {
                albums = list;
            }

            switch (method)
            {
                case SortMethods.Artist:
                    albums = albums.OrderBy(a => a.Artist);
                    break;

                case SortMethods.Genre:
                    albums = albums.OrderBy(a => a.Genres);
                    break;

                case SortMethods.Random:
                    Random rng = new Random();
                    albums = albums.OrderBy(a => rng.Next());
                    break;

                default:
                    albums = albums.OrderBy(a => a.Title);
                    break;
            }

            if (descending)
            {
                albums = albums.Reverse();
            }

            return albums;
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
