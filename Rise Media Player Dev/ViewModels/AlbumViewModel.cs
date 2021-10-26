using Rise.Models;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace RMP.App.ViewModels
{
    public class AlbumViewModel : BaseViewModel, IEditableObject
    {
        // private readonly DispatcherQueue dispatcherQueue = DispatcherQueue.GetForCurrentThread();

        /// <summary>
        /// Initializes a new instance of the AlbumViewModel class that wraps an Album object.
        /// </summary>
        public AlbumViewModel(Album model = null)
        {
            Model = model ?? new Album();
            IsNewAlbum = true;
            OnPropertyChanged(nameof(ArtistViewModel.AlbumCount));
        }

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
            get
            {
                if (Model.Title == "UnknownAlbumResource")
                {
                    return ResourceLoaders.MediaDataLoader.GetString("UnknownAlbumResource");
                }

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
        /// Gets or sets the album artist.
        /// </summary>
        public string Artist
        {
            get
            {
                if (Model.Artist == "UnknownArtistResource")
                {
                    return ResourceLoaders.MediaDataLoader.GetString("UnknownArtistResource");
                }

                return Model.Artist;
            }
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
        public string Genres
        {
            get
            {
                if (Model.Genres == "UnknownGenreResource")
                {
                    return ResourceLoaders.MediaDataLoader.GetString("UnknownGenreResource");
                }

                return Model.Genres;
            }
            set
            {
                if (value != Model.Genres)
                {
                    Model.Genres = value;
                    IsModified = true;
                    OnPropertyChanged(nameof(Genres));
                }
            }
        }

        /// <summary>
        /// Gets or sets the album song count.
        /// </summary>
        public uint TrackCount
        {
            get
            {
                int count = App.MViewModel.Songs.Count(s => s.Album == Model.Title && !s.WillRemove);

                if (count == 0)
                {
                    Delete();
                }
                return (uint)count;
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
                App.MViewModel.Albums.Add(this);
            }

            await App.Repository.Albums.UpsertAsync(Model).ConfigureAwait(false);
        }

        /// <summary>
        /// Delete album from repository and MViewModel.
        /// </summary>
        public async void Delete()
        {
            IsModified = true;
            WillRemove = true;

            App.MViewModel.Albums.Remove(this);
            await App.Repository.Albums.DeleteAsync(Model).ConfigureAwait(false);
            OnPropertyChanged(nameof(ArtistViewModel.AlbumCount));
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
