﻿using Rise.App.Views;
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
        /// <summary>
        /// Initializes a new instance of the PlaylistViewModel class that wraps a Playlist object.
        /// </summary>
        public PlaylistViewModel(Playlist model = null)
        {
            if (model != null)
            {
                Model = model;
            }
            else
            {
                Model = new Playlist();
                IsNew = true;
            }
        }

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
                    OnPropertyChanged(nameof(Title));
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
                    OnPropertyChanged(nameof(Description));
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
                    OnPropertyChanged(nameof(Icon));
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
                    OnPropertyChanged(nameof(Songs));
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

        private bool _isNew;
        /// <summary>
        /// Gets or sets a value that indicates whether this is a new item.
        /// </summary>
        public bool IsNew
        {
            get => _isNew;
            set => Set(ref _isNew, value);
        }

        /// <summary>
        /// Saves playlist to the backend.
        /// </summary>
        public async Task SaveAsync()
        {
            if (IsNew)
            {
                IsNew = false;
                App.MViewModel.Playlists.Add(this);
                await App.PBackendController.UpsertAsync(this);
            }
            else
            {
                await App.PBackendController.DeleteAsync(this);
                await App.PBackendController.UpsertAsync(this);
            }
        }

        /// <summary>
        /// Adds a song to the playlist.
        /// </summary>
        public async Task AddSongAsync(SongViewModel song)
        {
            Songs.Add(song);
            await SaveAsync();
        }

        /// <summary>
        /// Removes a song to the playlist.
        /// </summary>
        public async Task RemoveSongAsync(SongViewModel song)
        {
            Songs.Remove(song);
            await SaveAsync();
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
                await SaveAsync();
            }
        }

        /// <summary>
        /// Checks whether or not the playlist is available. If it's not,
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

        /// <summary>
        /// Delete playlist from repository and MViewModel.
        /// </summary>
        public async Task DeleteAsync()
        {
            App.MViewModel.Playlists.Remove(this);
            await App.PBackendController.DeleteAsync(this);
        }

        public async Task StartEditAsync()
        {
            _ = await typeof(PlaylistPropertiesPage).
                PlaceInWindowAsync(ApplicationViewMode.Default, 380, 550, true, this);
        }

        /// <summary>
        /// Saves any edits that have been made.
        /// </summary>
        public async Task EndEditAsync()
        {
            await App.PBackendController.DeleteAsync(this);
            await App.PBackendController.UpsertAsync(this);
        }

        /// <summary>
        /// Discards any edits that have been made, restoring the original values.
        /// </summary>
        public async Task RevertChangesAsync()
        {
            Model = (await App.PBackendController.GetAsync(Model.Id)).Model;
        }

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
                await SaveAsync();
            }
        }
    }
}
