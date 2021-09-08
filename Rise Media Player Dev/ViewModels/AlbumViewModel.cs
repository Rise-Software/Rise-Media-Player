using Rise.Models;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.System;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace RMP.App.ViewModels
{
    public class AlbumViewModel : BaseViewModel, IEditableObject
    {
        private DispatcherQueue dispatcherQueue = DispatcherQueue.GetForCurrentThread();

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

        private ImageSource PrivThumbnailBitmap { get; set; }
        public ImageSource ThumbnailBitmap
        {
            get => PrivThumbnailBitmap;
            set
            {
                IsModified = true;
                PrivThumbnailBitmap = value;
                OnPropertyChanged(nameof(ThumbnailBitmap));
            }
        }

        /// <summary>
        /// Load thumbnail image from LocalCache.
        /// </summary>
        /// <param name="imgFilename">Image filename.</param>
        /// <returns>Generated image from Uri.</returns>
        /// <remarks>In case the file doesn't exist, a default image will be loaded.</remarks>
        public async Task LoadThumbnailsAsync(string imgFilename)
        {
            BitmapImage bmp = new BitmapImage();
            StorageFolder localCache = ApplicationData.Current.LocalCacheFolder;

            IStorageItem resultingItem = await localCache.TryGetItemAsync(imgFilename);

            // If the file doesn't exist, use default thumbnail
            if (resultingItem != null)
            {
                StorageFile file = resultingItem as StorageFile;
                IRandomAccessStreamWithContentType stream = await file.OpenReadAsync();
                bmp.SetSource(stream);

                ThumbnailBitmap = bmp;
            }

            return;
        }

        /// <summary>
        /// Gets or sets a value that indicates whether the underlying model has been modified. 
        /// </summary>
        /// <remarks>
        /// Used when sync'ing with the server to reduce load and only upload the models that have changed.
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

            await App.Repository.Albums.UpsertAsync(Model);
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
