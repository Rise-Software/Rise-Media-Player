using Rise.App.Common;
using Rise.Data.ViewModels;
using Rise.Models;
using Rise.Repository.SQL;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Rise.App.ViewModels
{
    public class AlbumViewModel : ViewModel<Album>
    {
        #region Constructor
        /// <summary>
        /// Initializes a new instance of the AlbumViewModel class that wraps an Album object.
        /// </summary>
        public AlbumViewModel(Album model = null)
        {
            Model = model ?? new Album();

            OnPropertyChanged(nameof(ArtistViewModel.AlbumCount));
        }
        #endregion

        #region Properties
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
                    OnPropertyChanged();
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
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets the album title + artist together.
        /// </summary>
        public string TitleWithArtist
        {
            get
            {
                return $"{Title} - {Artist}";
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
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the album song count.
        /// </summary>
        public int TrackCount =>
            App.MViewModel.Songs.Count(s => s.Album == Model.Title);

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
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or setss the album release year.
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

        private bool _isArtistVisible = true;
        /// <summary>
        /// Gets or sets a value that indicates whether the album title is displayed or not.
        /// </summary>
        public bool IsArtistVisible
        {
            get => _isArtistVisible;
            set => Set(ref _isArtistVisible, value);
        }

        private bool _isThumbnailVisible = true;
        /// <summary>
        /// Gets or sets a value that indicates whether the album art is displayed or not.
        /// </summary>
        public bool IsThumbnailVisible
        {
            get => _isThumbnailVisible;
            set => Set(ref _isThumbnailVisible, value);
        }

        private bool _isGenresVisible = false;
        /// <summary>
        /// Gets or sets a value that indicates whether the album genre is displayed or not.
        /// </summary>
        public bool IsGenresVisible
        {
            get => _isGenresVisible;
            set => Set(ref _isGenresVisible, value);
        }

        private bool _isTitleVisible = true;
        /// <summary>
        /// Gets or sets a value that indicates whether the album title is displayed or not.
        /// </summary>
        public bool IsTitleVisible
        {
            get => _isTitleVisible;
            set => Set(ref _isTitleVisible, value);
        }

        private bool _hasRoundedAlbumArt = true;
        /// <summary>
        /// Gets or sets a value that indicates whether the album art is rounded or not.
        /// </summary>
        public bool HasRoundedAlbumArt
        {
            get => _hasRoundedAlbumArt;
            set => Set(ref _hasRoundedAlbumArt, value);
        }

        private bool _isReleaseYearVisible = false;
        /// <summary>
        /// Gets or sets a value that indicates whether the album release year is rounded or not.
        /// </summary>
        public bool IsReleaseYearVisible
        {
            get => _isReleaseYearVisible;
            set => Set(ref _isReleaseYearVisible, value);
        }
        #endregion

        #region Backend
        /// <summary>
        /// Saves item data to the backend.
        /// </summary>
        public async Task SaveAsync()
        {
            App.MViewModel.Albums.Add(this);
            await SQLRepository.Repository.Albums.QueueUpsertAsync(Model);
        }

        /// <summary>
        /// Deletes item data from the backend.
        /// </summary>
        public async Task DeleteAsync()
        {
            App.MViewModel.Albums.Remove(this);
            await SQLRepository.Repository.Albums.QueueDeletionAsync(Model);

            ArtistViewModel artist = App.MViewModel.Artists.
                FirstOrDefault(a => a.Model.Name == Model.Artist);

            if (artist != null)
            {
                await artist.CheckAvailabilityAsync();
            }
        }

        /// <summary>
        /// Checks whether or not the item is available. If it's not,
        /// delete it.
        /// </summary>
        public async Task CheckAvailabilityAsync()
        {
            if (TrackCount == 0)
            {
                await DeleteAsync();
                return;
            }
        }
        #endregion

        #region Editing
        /// <summary>
        /// Enables edit mode.
        /// </summary>
        /*public async Task StartEditAsync()
        {
            _ = await typeof(PropertiesPage).
                PlaceInWindowAsync(ApplicationViewMode.Default, 380, 550, true, props);
        }*/

        /// <summary>
        /// Saves any edits that have been made.
        /// </summary>
        public async Task SaveEditsAsync()
        {
            if (await SQLRepository.Repository.Songs.GetAsync(Model.Id) != null)
            {
                await SQLRepository.Repository.Albums.UpdateAsync(Model);
            }
            else await SaveAsync();
        }

        /// <summary>
        /// Discards any edits that have been made, restoring the original values.
        /// </summary>
        public async Task CancelEditsAsync()
        {
            Model = await SQLRepository.Repository.Albums.GetAsync(Model.Id);
        }
        #endregion
    }
}
