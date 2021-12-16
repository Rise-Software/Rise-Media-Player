using Rise.App.Common;
using Rise.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Rise.App.ViewModels
{
    public class PlaylistViewModel : ViewModel<Playlist>
    {
        // private readonly DispatcherQueue dispatcherQueue = DispatcherQueue.GetForCurrentThread();

        /// <summary>
        /// Initializes a new instance of the AlbumViewModel class that wraps an Playlist object.
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

            OnPropertyChanged(nameof(ArtistViewModel.AlbumCount));
        }

        /// <summary>
        /// Gets or sets the playlist title.
        /// </summary>
        public string Title
        {
            get
            {
                return Model.Title;
            }
            set
            {
                if (value != Model.Title)
                {
                    Model.Title = value;
                    IsModified = true;
                    OnPropertyChanged(nameof(Title));
                }
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether the underlying model has been modified. 
        /// </summary>
        /// <remarks>
        /// Used to reduce load and only upsert the models that have changed.
        /// </remarks>
        public bool IsModified { get; set; }

        private bool _isLoading;
        /// <summary>
        /// Gets or sets a value that indicates whether to show a progress bar. 
        /// </summary>
        public bool IsLoading
        {
            get => _isLoading;
            set => Set(ref _isLoading, value);
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

        private bool _isInEdit = false;

        /// <summary>
        /// Gets or sets a value that indicates whether the playlist data is being edited.
        /// </summary>
        public bool IsInEdit
        {
            get => _isInEdit;
            set => Set(ref _isInEdit, value);
        }

        /// <summary>
        /// Saves playlist data that has been edited.
        /// </summary>
        public async Task SaveAsync()
        {
            IsInEdit = false;
            IsModified = false;

            if (IsNew)
            {
                IsNew = false;
                App.MViewModel.Playlists.Add(this);
            }

            await App.Repository.Playlists.QueueUpsertAsync(Model);
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
            IsModified = true;

            App.MViewModel.Playlists.Remove(this);
            await App.Repository.Playlists.QueueDeletionAsync(Model);
        }

        /// <summary>
        /// Raised when the user cancels the changes they've made to the playlist data.
        /// </summary>
        public event EventHandler AddNewPlaylistCanceled;

        /// <summary>
        /// Cancels any in progress edits.
        /// </summary>
        public async Task CancelEditsAsync()
        {
            if (IsNew)
            {
                AddNewPlaylistCanceled?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                await RevertChangesAsync();
            }
        }

        /// <summary>
        /// Discards any edits that have been made, restoring the original values.
        /// </summary>
        public async Task RevertChangesAsync()
        {
            IsInEdit = false;
            if (IsModified)
            {
                await RefreshPlaylistsAsync();
                IsModified = false;
            }
        }

        /// <summary>
        /// Reloads all of the playlist data.
        /// </summary>
        public async Task RefreshPlaylistsAsync()
        {
            Model = await App.Repository.Playlists.GetAsync(Model.Id);
        }
    }
}
