using Rise.Models;
using RMP.App.Views;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.ViewManagement;

namespace RMP.App.ViewModels
{
    public class SongViewModel : BaseViewModel
    {
        // private readonly DispatcherQueue dispatcherQueue = DispatcherQueue.GetForCurrentThread();

        /// <summary>
        /// Initializes a new instance of the SongViewModel class that wraps a Song object.
        /// </summary>
        public SongViewModel(Song model = null)
        {
            Model = model ?? new Song();
            IsNewSong = true;

            OnPropertyChanged(nameof(AlbumViewModel.TrackCount));
            OnPropertyChanged(nameof(ArtistViewModel.SongCount));
        }

        private Song _model;

        /// <summary>
        /// Gets or sets the underlying Song object.
        /// </summary>
        public Song Model
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
        /// Gets or sets the song title.
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
        /// Gets or sets the song artist.
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
        /// Gets or sets the song track.
        /// </summary>
        public uint Track
        {
            get => Model.Track;
            set
            {
                if (value != Model.Track)
                {
                    Model.Track = value;
                    IsModified = true;
                    OnPropertyChanged(nameof(Track));
                }
            }
        }

        /// <summary>
        /// Gets or sets the song disc.
        /// </summary>
        public int Disc
        {
            get => Model.Disc;
            set
            {
                if (value != Model.Disc)
                {
                    Model.Disc = value;
                    IsModified = true;
                    OnPropertyChanged(nameof(Disc));
                }
            }
        }

        /// <summary>
        /// Gets or sets the song album. 
        /// </summary>
        public string Album
        {
            get
            {
                if (Model.Album == "UnknownAlbumResource")
                {
                    return ResourceLoaders.MediaDataLoader.GetString("UnknownAlbumResource");
                }

                return Model.Album;
            }
            set
            {
                if (value != Model.Album)
                {
                    Model.Album = value;
                    IsModified = true;
                    OnPropertyChanged(nameof(Album));
                }
            }
        }

        /// <summary>
        /// Gets or sets the song album artist. 
        /// </summary>
        public string AlbumArtist
        {
            get
            {
                if (Model.AlbumArtist == "UnknownArtistResource")
                {
                    return ResourceLoaders.MediaDataLoader.GetString("UnknownArtistResource");
                }

                return Model.AlbumArtist;
            }
            set
            {
                if (value != Model.AlbumArtist)
                {
                    Model.AlbumArtist = value;
                    IsModified = true;
                    OnPropertyChanged(nameof(AlbumArtist));
                }
            }
        }

        /// <summary>
        /// Gets or sets the song genres.
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
        /// Gets or sets the song length. 
        /// </summary>
        public string Length
        {
            get => Model.Length;
            set
            {
                if (value != Model.Length)
                {
                    Model.Length = value;
                    IsModified = true;
                    OnPropertyChanged(nameof(Length));
                }
            }
        }

        /// <summary>
        /// Gets or sets the song year.
        /// </summary>
        public uint Year
        {
            get => Model.Year;
            set
            {
                if (value != Model.Year)
                {
                    Model.Year = value;
                    IsModified = true;
                    OnPropertyChanged(nameof(Year));
                }
            }
        }

        /// <summary>
        /// Gets or sets the song location. 
        /// </summary>
        public string Location
        {
            get => Model.Location;
            set
            {
                if (value != Model.Location)
                {
                    Model.Location = value;
                    IsModified = true;
                    OnPropertyChanged(nameof(Location));
                }
            }
        }

        /// <summary>
        /// Gets the song filename.
        /// </summary>
        public string Filename => Path.GetFileName(Location);

        /// <summary>
        /// Gets the song extension.
        /// </summary>
        public string Extension => Path.GetExtension(Location);

        /// <summary>
        /// Gets or sets the song rating.
        /// </summary>
        public uint Rating
        {
            get => Model.Rating;
            set
            {
                if (value != Model.Rating)
                {
                    Model.Rating = value;
                    IsModified = true;
                    OnPropertyChanged(nameof(Rating));
                }
            }
        }

        private string _thumbnail;

        /// <summary>
        /// Gets the song album's thumbnail.
        /// </summary>
        public string Thumbnail
        {
            get
            {
                if (_thumbnail == null)
                {
                    _thumbnail = "ms-appx:///Assets/Default.png";
                    try
                    {
                        _thumbnail = App.MViewModel.Albums.First(a => a.Model.Title == Model.Album
                            && a.Model.Artist == Model.AlbumArtist).Thumbnail;
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                    }
                }

                return _thumbnail;
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether or not the
        /// item has to be removed.
        /// </summary>
        public bool Removed
        {
            get => Model.Removed;
            private set
            {
                if (value != Model.Removed)
                {
                    Model.Removed = value;
                    IsModified = true;
                    OnPropertyChanged(string.Empty);
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

        private bool _isNewSong;

        /// <summary>
        /// Gets or sets a value that indicates whether this is a new song.
        /// </summary>
        public bool IsNewSong
        {
            get => _isNewSong;
            set => Set(ref _isNewSong, value);
        }

        private bool _isInEdit;

        /// <summary>
        /// Gets or sets a value that indicates whether the song data is being edited.
        /// </summary>
        public bool IsInEdit
        {
            get => _isInEdit;
            set => Set(ref _isInEdit, value);
        }

        /// <summary>
        /// Saves song data that has been edited.
        /// </summary>
        public void Save()
        {
            IsInEdit = false;
            IsModified = false;
            Removed = false;

            if (IsNewSong)
            {
                IsNewSong = false;
                App.MViewModel.Songs.Add(this);

                OnPropertyChanged(nameof(AlbumViewModel.TrackCount));
                OnPropertyChanged(nameof(ArtistViewModel.SongCount));
            }

            App.Repository.Songs.QueueUpsert(Model);
        }

        /// <summary>
        /// Delete song from MViewModel.
        /// </summary>
        public async Task DeleteAsync()
        {
            await CancelEditsAsync();
            IsModified = true;
            Removed = true;

            if (!IsNewSong)
            {
                App.MViewModel.Songs.Remove(this);
            }

            App.Repository.Songs.QueueUpsert(Model);
            AlbumViewModel album = App.MViewModel.Albums.
                FirstOrDefault(a => a.Model.Title == Model.Album &&
                           a.Model.Artist == Model.AlbumArtist);

            if (album != null)
            {
                album.CheckAvailability();
            }

            ArtistViewModel artist = App.MViewModel.Artists.
                FirstOrDefault(a => a.Model.Name == Model.Artist);

            if (artist != null)
            {
                artist.CheckAvailability();
            }
        }

        /// <summary>
        /// Raised when the user cancels the changes they've made to the song data.
        /// </summary>
        public event EventHandler AddNewSongCanceled;

        /// <summary>
        /// Cancels any in progress edits.
        /// </summary>
        public async Task CancelEditsAsync()
        {
            if (IsNewSong)
            {
                AddNewSongCanceled?.Invoke(this, EventArgs.Empty);
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
                await RefreshSongsAsync();
                IsModified = false;
            }
        }

        /// <summary>
        /// Enables edit mode.
        /// </summary>
        public async Task StartEdit()
        {
            IsInEdit = true;
            StorageFile file = await StorageFile.GetFileFromPathAsync(Location);

            if (file != null)
            {
                SongPropertiesViewModel props = new SongPropertiesViewModel(this, file.DateCreated)
                {
                    FileProps = await file.GetBasicPropertiesAsync()
                };

                _ = await typeof(PropertiesPage).
                    OpenInWindowAsync(ApplicationViewMode.Default, 380, 550, props);
            }
        }

        /// <summary>
        /// Reloads all of the song data.
        /// </summary>
        public async Task RefreshSongsAsync()
        {
            Model = await App.Repository.Songs.GetAsync(Model.Id);
        }
    }
}
