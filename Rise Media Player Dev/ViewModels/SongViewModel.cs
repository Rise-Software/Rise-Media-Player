using Microsoft.Toolkit.Uwp;
using Rise.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.System;

namespace RMP.App.ViewModels
{
    public class SongViewModel : BaseViewModel, IEditableObject
    {
        private DispatcherQueue dispatcherQueue = DispatcherQueue.GetForCurrentThread();

        /// <summary>
        /// Initializes a new instance of the SongViewModel class that wraps a Song object.
        /// </summary>
        public SongViewModel(Song model = null) => Model = model ?? new Song();

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
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(Title));
                }
            }
        }

        /// <summary>
        /// Gets or sets the song artist.
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
            get => Model.Album;
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
            get => Model.AlbumArtist;
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
        /// Gets or sets the song genre. 
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

        private bool _isNewSong;

        /// <summary>
        /// Gets or sets a value that indicates whether this is a new song.
        /// </summary>
        public bool IsNewSong
        {
            get => _isNewSong;
            set => Set(ref _isNewSong, value);
        }

        private bool _isInEdit = false;

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
        public async Task SaveAsync()
        {
            IsInEdit = false;
            IsModified = false;
            if (IsNewSong)
            {
                IsNewSong = false;
                App.ViewModel.Songs.Add(this);
            }

            await App.Repository.Songs.UpsertAsync(Model);
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
        public void StartEdit() => IsInEdit = true;

        /// <summary>
        /// Reloads all of the song data.
        /// </summary>
        public async Task RefreshSongsAsync()
        {
            Model = await App.Repository.Songs.GetAsync(Model.Id);
        }

        /// <summary>
        /// Called when a bound DataGrid control causes the song to enter edit mode.
        /// </summary>
        public void BeginEdit()
        {
            // Not used.
        }

        /// <summary>
        /// Called when a bound DataGrid control cancels the edits that have been made to a song.
        /// </summary>
        public async void CancelEdit() => await CancelEditsAsync();

        /// <summary>
        /// Called when a bound DataGrid control commits the edits that have been made to a song.
        /// </summary>
        public async void EndEdit() => await SaveAsync();
    }
}
