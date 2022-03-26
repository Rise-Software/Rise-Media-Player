using Rise.Common;
using Rise.Common.Interfaces;
using Rise.Data.ViewModels;
using Rise.Models;
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
        #endregion

        #region Backend
        /// <summary>
        /// Saves item data to the backend.
        /// </summary>
        public async Task SaveAsync()
        {
            if (!App.MViewModel.Artists.Contains(this))
            {
                App.MViewModel.Artists.Add(this);
            }
            await NewRepository.Repository.UpsertAsync(Model);
        }

        /// <summary>
        /// Deletes item data from the backend.
        /// </summary>
        public async Task DeleteAsync()
        {
            if (App.MViewModel.Artists.Contains(this))
            {
                App.MViewModel.Artists.Remove(this);
                await NewRepository.Repository.DeleteAsync(Model);
            }
        }

        /// <summary>
        /// Checks whether or not the item is available. If it's not,
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
        /// Discards any edits that have been made, restoring the original values.
        /// </summary>
        public async Task CancelEditsAsync()
        {
            Model = await NewRepository.Repository.GetArtistAsync(Model.Id);
        }
        #endregion
    }
}
