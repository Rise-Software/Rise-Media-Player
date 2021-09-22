using Rise.Models;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;

namespace RMP.App.ViewModels
{
    public class AlbumViewModel : BaseViewModel, IEditableObject
    {
        // private readonly DispatcherQueue dispatcherQueue = DispatcherQueue.GetForCurrentThread();

        /// <summary>
        /// Initializes a new instance of the AlbumViewModel class that wraps an Album object.
        /// </summary>
        public AlbumViewModel(Album model = null) => Model = model ?? new Album();

        private Album _model;

        /// <summary>
        /// Gets or sets the underlying Album object.
        /// </summary>
        public Album Model
        {
            get => _model;
            set
            {
                if (_model != value)
                {
                    _model = value;

                    // Raise the PropertyChanged event for all properties.
                    OnPropertyChanged(string.Empty);
                }
            }
        }

        /// <summary>
        /// Gets or sets the album title.
        /// </summary>
        public string Title
        {
            get => Model.Title;
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
        /// Gets or sets the album artist.
        /// </summary>
        public string Artist
        {
            get => Model.Artist;
            set
            {
                if (value != Model.Artist)
                {
                    Model.Artist = value;
                    IsModified = true;
                    OnPropertyChanged(nameof(Artist));
                }
            }
        }

        /// <summary>
        /// Gets or sets the album genre.
        /// </summary>
        public string Genre
        {
            get => Model.Genre;
            set
            {
                if (value != Model.Genre)
                {
                    Model.Genre = value;
                    IsModified = true;
                    OnPropertyChanged(nameof(Genre));
                }
            }
        }

        /// <summary>
        /// Gets or sets the album song count.
        /// </summary>
        public int SongCount
        {
            get => Model.SongCount;
            set
            {
                if (value != Model.SongCount)
                {
                    if (value < 1)
                    {
                        Delete();
                    }
                    else
                    {
                        Model.SongCount = value;
                        IsModified = true;
                        OnPropertyChanged(nameof(SongCount));
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the album thumbnail.
        /// </summary>
        public string Thumbnail
        {
            get => Model.Thumbnail;
            set
            {
                if (value != Model.Thumbnail)
                {
                    Model.Thumbnail = value;
                    IsModified = true;
                    OnPropertyChanged(nameof(Thumbnail));
                }
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether the item has to be deleted.
        /// </summary>
        public bool WillRemove { get; set; }

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

        private bool _isNewAlbum;

        /// <summary>
        /// Gets or sets a value that indicates whether this is a new album.
        /// </summary>
        public bool IsNewAlbum
        {
            get => _isNewAlbum;
            set => Set(ref _isNewAlbum, value);
        }

        private bool _isInEdit = false;

        /// <summary>
        /// Gets or sets a value that indicates whether the album data is being edited.
        /// </summary>
        public bool IsInEdit
        {
            get => _isInEdit;
            set => Set(ref _isInEdit, value);
        }

        /// <summary>
        /// Saves album data that has been edited.
        /// </summary>
        public async Task SaveAsync()
        {
            IsInEdit = false;
            IsModified = false;
            if (IsNewAlbum)
            {
                IsNewAlbum = false;
                App.ViewModel.Albums.Add(this);
            }

            await App.Repository.Albums.UpsertAsync(Model).ConfigureAwait(false);
        }

        /// <summary>
        /// Delete album from repository and ViewModel.
        /// </summary>
        public void Delete()
        {
            IsModified = true;
            WillRemove = true;
            Debug.WriteLine("Album removed!");
        }

        /// <summary>
        /// Raised when the user cancels the changes they've made to the album data.
        /// </summary>
        public event EventHandler AddNewAlbumCanceled;

        /// <summary>
        /// Cancels any in progress edits.
        /// </summary>
        public async Task CancelEditsAsync()
        {
            if (IsNewAlbum)
            {
                AddNewAlbumCanceled?.Invoke(this, EventArgs.Empty);
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
                await RefreshAlbumsAsync();
                IsModified = false;
            }
        }

        /// <summary>
        /// Enables edit mode.
        /// </summary>
        public void StartEdit() => IsInEdit = true;

        /// <summary>
        /// Reloads all of the album data.
        /// </summary>
        public async Task RefreshAlbumsAsync()
        {
            Model = await App.Repository.Albums.GetAsync(Model.Id);
        }

        /// <summary>
        /// Called when a bound DataGrid control causes the album to enter edit mode.
        /// </summary>
        public void BeginEdit()
        {
            // Not used.
        }

        /// <summary>
        /// Called when a bound DataGrid control cancels the edits that have been made to an album.
        /// </summary>
        public async void CancelEdit() => await CancelEditsAsync();

        /// <summary>
        /// Called when a bound DataGrid control commits the edits that have been made to an album.
        /// </summary>
        public async void EndEdit() => await SaveAsync();
    }
}
