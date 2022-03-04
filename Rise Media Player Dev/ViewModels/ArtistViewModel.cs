using Rise.App.Common;
using Rise.Models;
using Rise.Repository.SQL;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Rise.App.ViewModels
{
    public class ArtistViewModel : ViewModel<Artist>
    {
        #region Constructor
        /// <summary>
        /// Initializes a new instance of the ArtistViewModel class that wraps an Artist object.
        /// </summary>
        public ArtistViewModel(Artist model = null)
        {
            Model = model ?? new Artist();
            IsNew = true;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the artist name.
        /// </summary>
        public string Name
        {
            get
            {
                if (Model.Name == "UnknownArtistResource")
                {
                    return ResourceLoaders.MediaDataLoader.GetString("UnknownArtistResource");
                }

                return Model.Name;
            }
            set
            {
                if (value != Model.Name)
                {
                    Model.Name = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the artist picture.
        /// </summary>
        public string Picture
        {
            get => Model.Picture;
            set
            {
                if (value != Model.Picture)
                {
                    Model.Picture = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the artist's song count.
        /// </summary>
        public int SongCount => App.MViewModel.Songs.Count(s => s.Model.Artist == Model.Name);
        public string Songs => SongCount.ToString() + " " + ResourceLoaders.MediaDataLoader.GetString("Songs");

        /// <summary>
        /// Gets or sets the artist's album count.
        /// </summary>
        public int AlbumCount => App.MViewModel.Albums.Count(a => a.Model.Artist == Model.Name);
        public string Albums => AlbumCount.ToString() + " " + ResourceLoaders.MediaDataLoader.GetString("Albums");

        /// <summary>
        /// Combination of artist's song count and album count.
        /// </summary>
        public string SongsNAlbums => Albums + ", " + Songs;

        private bool _isNew;
        /// <summary>
        /// Gets or sets a value that indicates whether this is a new item.
        /// </summary>
        public bool IsNew
        {
            get => _isNew;
            set => Set(ref _isNew, value);
        }
        #endregion

        #region Backend
        /// <summary>
        /// Saves artist data that has been edited.
        /// </summary>
        public async Task SaveAsync()
        {
            if (IsNew)
            {
                IsNew = false;
                App.MViewModel.Artists.Add(this);
            }

            await SQLRepository.Repository.Artists.QueueUpsertAsync(Model);
        }

        /// <summary>
        /// Delete artist from repository and MViewModel.
        /// </summary>
        public async Task DeleteAsync()
        {
            App.MViewModel.Artists.Remove(this);
            await SQLRepository.Repository.Artists.QueueDeletionAsync(Model);
        }

        /// <summary>
        /// Checks whether or not the artist is available. If it's not,
        /// delete it.
        /// </summary>
        public async Task CheckAvailabilityAsync()
        {
            if (SongCount == 0 && AlbumCount == 0)
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
        public async Task SaveEditAsync()
        {
            await SQLRepository.Repository.Artists.UpdateAsync(Model);
        }

        /// <summary>
        /// Discards any edits that have been made, restoring the original values.
        /// </summary>
        public async Task CancelEditAsync()
        {
            Model = await SQLRepository.Repository.Artists.GetAsync(Model.Id);
        }
        #endregion
    }
}
