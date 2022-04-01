using Newtonsoft.Json;
using Rise.App.Views;
using Rise.Common;
using Rise.Common.Extensions;
using Rise.Data.ViewModels;
using Rise.Models;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Media;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Storage;
using Windows.Storage.Streams;

namespace Rise.App.ViewModels
{
    public class SongViewModel : ViewModel<Song>
    {

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the SongViewModel class that wraps a Song object.
        /// </summary>
        public SongViewModel(Song model = null)
        {
            Model = model ?? new Song();

            OnPropertyChanged(nameof(AlbumViewModel.TrackCount));
            OnPropertyChanged(nameof(ArtistViewModel.SongCount));
        }
        #endregion

        #region Properties
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
                    OnPropertyChanged();
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
                    OnPropertyChanged();
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
                    OnPropertyChanged();
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
                    OnPropertyChanged();
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
                    OnPropertyChanged();
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
                    OnPropertyChanged();
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
                    OnPropertyChanged();
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
                    OnPropertyChanged();
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
                    OnPropertyChanged();
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
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets the song filename.
        /// </summary>
        [JsonIgnore]
        public string Filename => Path.GetFileName(Location);

        /// <summary>
        /// Gets the song extension.
        /// </summary>
        [JsonIgnore]
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
                    OnPropertyChanged();
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
                    OnPropertyChanged();
                }
            }
        }

        private bool _isFocused;
        /// <summary>
        /// Gets or sets a value that indicates whether the item is focused.
        /// </summary>
        [JsonIgnore]
        public bool IsFocused
        {
            get => _isFocused;
            set => Set(ref _isFocused, value);
        }

        private bool _isTrackNumberVisible = true;
        /// <summary>
        /// Gets or sets a value that indicates whether the songs' track number is visible.
        /// </summary>
        [JsonIgnore]
        public bool IsTrackNumberVisible
        {
            get => _isTrackNumberVisible;
            set => Set(ref _isTrackNumberVisible, value);
        }

        private bool _isDurationVisible = true;

        /// <summary>
        /// Gets or sets a value that indicates whether the songs' track number is visible.
        /// </summary>
        [JsonIgnore]
        public bool IsDurationVisible
        {
            get => _isDurationVisible;
            set => Set(ref _isDurationVisible, value);
        }
        #endregion

        #region Backend
        /// <summary>
        /// Saves item data to the backend.
        /// </summary>
        public async Task SaveAsync(bool addToList = true)
        {
            if (addToList)
            {
                if (!App.MViewModel.Songs.Contains(this))
                {
                    App.MViewModel.Songs.Add(this);
                }
            }
            await NewRepository.Repository.UpsertAsync(Model);
        }

        /// <summary>
        /// Deletes item data from the backend.
        /// </summary>
        public async Task DeleteAsync()
        {
            App.MViewModel.Songs.Remove(this);

            await NewRepository.Repository.DeleteAsync(Model);

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
        #endregion

        #region Editing
        /// <summary>
        /// Enables edit mode.
        /// </summary>
        public async Task StartEditAsync()
        {
            if (!IsOnline)
            {
                try
                {
                    StorageFile file = await StorageFile.GetFileFromPathAsync(Location);

                    if (file != null)
                    {
                        SongPropertiesViewModel props = new(this, file.DateCreated)
                        {
                            FileProps = await file.GetBasicPropertiesAsync()
                        };

                        _ = await typeof(SongPropertiesPage).
                            PlaceInApplicationViewAsync(props, 380, 550, true);
                    }
                }
                catch
                {

                }
            }
        }

        /// <summary>
        /// Discards any edits that have been made, restoring the original values.
        /// </summary>
        public async Task CancelEditsAsync()
        {
            Model = await NewRepository.Repository.GetItemAsync<Song>(Model.Id);
        }
        #endregion

        #region Playback
        /// <summary>
        /// Creates a <see cref="MediaPlaybackItem"/> from this <see cref="SongViewModel"/>.
        /// </summary>
        /// <returns>A <see cref="MediaPlaybackItem"/> based on the song.</returns>
        public async Task<MediaPlaybackItem> AsPlaybackItemAsync()
        {
            try
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
            catch
            {

            }
            return null;
        }

        /// <summary>
        /// Creates a <see cref="MediaPlaybackItem"/> from this <see cref="SongViewModel"/>.
        /// </summary>
        /// <returns>A <see cref="MediaPlaybackItem"/> based on the song.</returns>
        public MediaPlaybackItem AsPlaybackItem(Uri url)
        {
            try
            {
                MediaSource source = MediaSource.CreateFromUri(url);
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
            catch
            {

            }
            return null;
        }
        #endregion
    }
}
