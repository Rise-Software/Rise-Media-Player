using Microsoft.Toolkit.Uwp;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.System;

namespace RMP.App.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private readonly DispatcherQueue dispatcherQueue = DispatcherQueue.GetForCurrentThread();

        /// <summary>
        /// Creates a new MainViewModel.
        /// </summary>
        public MainViewModel() => Task.Run(GetListsAsync);

        /// <summary>
        /// The collection of songs in the list. 
        /// </summary>
        public ObservableCollection<SongViewModel> Songs { get; set; }
            = new ObservableCollection<SongViewModel>();

        private SongViewModel _selectedSong;

        /// <summary>
        /// Gets or sets the selected song, or null if no song is selected. 
        /// </summary>
        public SongViewModel SelectedSong
        {
            get => _selectedSong;
            set => Set(ref _selectedSong, value);
        }

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
            _ = await dispatcherQueue.EnqueueAsync(() => IsLoading = true);

            System.Collections.Generic.IEnumerable<Rise.Models.Song> songs = await App.Repository.Songs.GetAsync();
            if (songs == null)
            {
                return;
            }

            System.Collections.Generic.IEnumerable<Rise.Models.Album> albums = await App.Repository.Albums.GetAsync();
            if (albums == null)
            {
                return;
            }

            await dispatcherQueue.EnqueueAsync(() =>
            {
                Songs.Clear();
                foreach (Rise.Models.Song s in songs)
                {
                    Songs.Add(new SongViewModel(s));
                }

                Albums.Clear();
                foreach (Rise.Models.Album a in albums)
                {
                    Albums.Add(new AlbumViewModel(a));
                }

                IsLoading = false;
            });
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

                await GetListsAsync();
                IsLoading = false;
            });
        }
    }
}
