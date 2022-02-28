using Rise.App.Common;
using Rise.App.Views;
using Rise.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Media;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.ViewManagement;

namespace Rise.App.ViewModels
{
    public class SongViewModel : ViewModel<Song>
    {
        // private readonly DispatcherQueue dispatcherQueue = DispatcherQueue.GetForCurrentThread();

        /// <summary>
        /// Initializes a new instance of the SongViewModel class that wraps a Song object.
        /// </summary>
        public SongViewModel(Song model = null)
        {
            Model = model ?? new Song();
            IsNew = true;

            OnPropertyChanged(nameof(AlbumViewModel.TrackCount));
            OnPropertyChanged(nameof(ArtistViewModel.SongCount));
        }

        /// <summary>
        /// Checks if the song is played from an online stream, playlist or song.
        /// </summary>
        public bool IsOnline = false;

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
        public TimeSpan Length
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

        /// <summary>
        /// Gets the song album's thumbnail.
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

        private bool _isInEdit;
        /// <summary>
        /// Gets or sets a value that indicates whether the item data is being edited.
        /// </summary>
        public bool IsInEdit
        {
            get => _isInEdit;
            set => Set(ref _isInEdit, value);
        }

        private bool _isFocused;
        /// <summary>
        /// Gets or sets a value that indicates whether the item is focused.
        /// </summary>
        public bool IsFocused
        {
            get => _isFocused;
            set => Set(ref _isFocused, value);
        }

        private bool _isTrackNumberVisible = true;
        /// <summary>
        /// Gets or sets a value that indicates whether the ite's track number is visible.
        /// </summary>
        public bool IsTrackNumberVisible
        {
            get => _isTrackNumberVisible;
            set => Set(ref _isTrackNumberVisible, value);
        }

        private bool _isDurationVisible = true;
        /// <summary>
        /// Gets or sets a value that indicates whether the ite's track number is visible.
        /// </summary>
        public bool IsDurationVisible
        {
            get => _isDurationVisible;
            set => Set(ref _isDurationVisible, value);
        }

        /// <summary>
        /// Saves song data that has been edited.
        /// </summary>
        public async Task SaveAsync()
        {
            IsInEdit = false;
            IsModified = false;

            if (IsNew)
            {
                IsNew = false;
                App.MViewModel.Songs.Add(this);

                OnPropertyChanged(nameof(AlbumViewModel.TrackCount));
                OnPropertyChanged(nameof(ArtistViewModel.SongCount));
            }

            await App.Repository.Songs.QueueUpsertAsync(Model);
        }

        /// <summary>
        /// Delete song from MViewModel.
        /// </summary>
        public async Task DeleteAsync()
        {
            await CancelEditsAsync();
            IsModified = true;

            if (!IsNew)
            {
                App.MViewModel.Songs.Remove(this);
            }

            await App.Repository.Songs.QueueUpsertAsync(Model);
            AlbumViewModel album = App.MViewModel.Albums.
                FirstOrDefault(a => a.Model.Title == Model.Album &&
                           a.Model.Artist == Model.AlbumArtist);

            if (album != null)
            {
                await album.CheckAvailabilityAsync();
            }

            ArtistViewModel artist = App.MViewModel.Artists.
                FirstOrDefault(a => a.Model.Name == Model.Artist);

            if (artist != null)
            {
                await artist.CheckAvailabilityAsync();
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
            if (IsNew)
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
                    PlaceInWindowAsync(ApplicationViewMode.Default, 380, 550, true, props);
            }
        }

        /// <summary>
        /// Reloads all of the song data.
        /// </summary>
        public async Task RefreshSongsAsync()
        {
            Model = await App.Repository.Songs.GetAsync(Model.Id);
        }

        /// <summary>
        /// Creates a <see cref="MediaPlaybackItem"/> from this <see cref="SongViewModel"/>.
        /// </summary>
        /// <returns>A <see cref="MediaPlaybackItem"/> based on the song.</returns>
        public async Task<MediaPlaybackItem> AsPlaybackItemAsync()
        {
            StorageFile file = await StorageFile.GetFileFromPathAsync(Location);

            MediaSource source = MediaSource.CreateFromStorageFile(file);
            MediaPlaybackItem media = new(source);

            MediaItemDisplayProperties props = media.GetDisplayProperties();
            props.Type = MediaPlaybackType.Music;

            props.MusicProperties.Title = Title;
            props.MusicProperties.Artist = Artist;
            props.MusicProperties.AlbumTitle = Album;
            props.MusicProperties.AlbumArtist = AlbumArtist;
            props.MusicProperties.TrackNumber = Track;

            if (Thumbnail != null)
            {
                props.Thumbnail = RandomAccessStreamReference.
                    CreateFromUri(new Uri(Thumbnail));
            }

            media.ApplyDisplayProperties(props);
            return media;
        }

        /// <summary>
        /// Creates a <see cref="MediaPlaybackItem"/> from this <see cref="SongViewModel"/>.
        /// </summary>
        /// <returns>A <see cref="MediaPlaybackItem"/> based on the song.</returns>
        public async Task<MediaPlaybackItem> AsPlaybackItemAsync(Uri url)
        {
            MediaSource source = MediaSource.CreateFromUri(url);
            MediaPlaybackItem media = new(source);

            MediaItemDisplayProperties props = media.GetDisplayProperties();
            props.Type = MediaPlaybackType.Music;

            /*props.MusicProperties.Title = Title;
            props.MusicProperties.Artist = Artist;
            props.MusicProperties.AlbumTitle = Album;
            props.MusicProperties.AlbumArtist = AlbumArtist;
            props.MusicProperties.TrackNumber = Track;*/
            Title = props.MusicProperties.Title;

            if (Thumbnail != null)
            {
                props.Thumbnail = RandomAccessStreamReference.
                    CreateFromUri(new Uri(Thumbnail));
            }

            media.ApplyDisplayProperties(props);
            return media;
        }

        public readonly static RelayCommand _beginPlayback = new RelayCommand(async () =>
        {
            int index = 0;
            if (App.MViewModel.SelectedSong != null)
            {
                index = App.MViewModel.FilteredSongs.IndexOf(App.MViewModel.SelectedSong);
                App.MViewModel.SelectedSong = null;
            }

            IEnumerator<object> enumerator = App.MViewModel.FilteredSongs.GetEnumerator();
            List<SongViewModel> songs = new List<SongViewModel>();

            while (enumerator.MoveNext())
            {
                songs.Add(enumerator.Current as SongViewModel);
            }

            enumerator.Dispose();
            await App.PViewModel.StartMusicPlaybackAsync(songs.GetEnumerator(), index, songs.Count);
        });
    }
}
