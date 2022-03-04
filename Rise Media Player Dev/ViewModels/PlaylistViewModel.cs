using Rise.App.Views;
using Rise.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.UI.ViewManagement;

namespace Rise.App.ViewModels
{
    public class PlaylistViewModel : ViewModel<Playlist>
    {
        #region Constructor
        /// <summary>
        /// Initializes a new instance of the PlaylistViewModel class that wraps a Playlist object.
        /// </summary>
        public PlaylistViewModel(Playlist model = null)
        {
            Model = model ?? new Playlist();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the playlist title.
        /// </summary>
        public string Title
        {
            get => Model.Title;
            set
            {
                if (value != Model.Title)
                {
                    Model.Title = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the playlist description.
        /// </summary>
        public string Description
        {
            get => Model.Description;
            set
            {
                if (value != Model.Description)
                {
                    Model.Description = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the playlist icon.
        /// </summary>
        public string Icon
        {
            get => Model.Icon;
            set
            {
                if (value != Model.Icon)
                {
                    Model.Icon = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the playlist duration.
        /// </summary>
        public string Duration
        {
            get => Model.Duration;
            set
            {
                if (value != Model.Duration)
                {
                    Model.Duration = value;
                    OnPropertyChanged(nameof(Duration));
                }
            }
        }

        private ObservableCollection<SongViewModel> _songs = new();
        public ObservableCollection<SongViewModel> Songs
        {
            get => _songs;
            set
            {
                if (value != _songs)
                {
                    _songs = value;
                    OnPropertyChanged();
                }
            }
        }

        public int SongsCount
        {
            get
            {
                if (Songs != null)
                {
                    return Songs.Count;
                }
                else
                {
                    return 0;
                }
            }
        }
        #endregion

        #region Backend
        /// <summary>
        /// Saves item data to the backend.
        /// </summary>
        public async Task SaveAsync()
        {
            App.MViewModel.Playlists.Add(this);
            await App.PBackendController.UpsertAsync(this);
        }

        /// <summary>
        /// Deletes item data from the backend.
        /// </summary>
        public async Task DeleteAsync()
        {
            App.MViewModel.Playlists.Remove(this);
            await App.PBackendController.DeleteAsync(this);
        }

        /// <summary>
        /// Checks whether or not the item is available. If it's not,
        /// delete it.
        /// </summary>
        /*public async Task CheckAvailabilityAsync()
        {
            if (TrackCount == 0)
            {
                await DeleteAsync();
                return;
            }
        }*/
        #endregion

        #region Item management
        /// <summary>
        /// Adds a song to the playlist.
        /// </summary>
        public async Task AddSongAsync(SongViewModel song)
        {
            Songs.Add(song);
            await SaveEditsAsync();
        }

        /// <summary>
        /// Removes a song to the playlist.
        /// </summary>
        public async Task RemoveSongAsync(SongViewModel song)
        {
            Songs.Remove(song);
            await SaveEditsAsync();
        }

        /// <summary>
        /// Adds multiple songs to the playlist.
        /// </summary>
        public async Task AddSongsAsync(IEnumerable<SongViewModel> songs)
        {
            try
            {
                foreach (SongViewModel song in songs)
                {
                    Songs.Add(song);
                }
            }
            finally
            {
                await SaveEditsAsync();
            }
        }

        /// <summary>
        /// Removes multiple songs from the playlist.
        /// </summary>
        public async Task RemoveSongsAsync(List<SongViewModel> songs)
        {
            try
            {
                foreach (SongViewModel song in songs)
                {
                    Songs.Remove(song);
                }
            }
            finally
            {
                await SaveEditsAsync();
            }
        }
        #endregion

        #region Editing
        public async Task StartEditAsync()
        {
            _ = await typeof(PlaylistPropertiesPage).
                PlaceInWindowAsync(ApplicationViewMode.Default, 380, 550, true, this);
        }

        /// <summary>
        /// Saves any edits that have been made.
        /// </summary>
        public async Task SaveEditsAsync()
        {
            await App.PBackendController.DeleteAsync(this);
            await App.PBackendController.UpsertAsync(this);
        }

        /// <summary>
        /// Discards any edits that have been made, restoring the original values.
        /// </summary>
        public async Task CancelEditsAsync()
        {
            Model = (await App.PBackendController.GetAsync(Model.Id)).Model;
        }
        #endregion
    }
}
