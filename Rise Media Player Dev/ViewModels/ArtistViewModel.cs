using Rise.Common;
using Rise.Common.Extensions.Markup;
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
                    return ResourceHelper.GetString("UnknownArtistResource");
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
        /// Gets the artist's song count.
        /// </summary>
        public int SongCount
            => App.MViewModel.Songs.Count(s => s.Model.Artist == Model.Name);
        public string LocalizedSongCount
            => ResourceHelper.GetLocalizedCount("Song", SongCount);

        /// <summary>
        /// Gets the artist's album count.
        /// </summary>
        public int AlbumCount
            => App.MViewModel.Albums.Count(a => a.Model.Artist == Model.Name);
        public string LocalizedAlbumCount
            => ResourceHelper.GetLocalizedCount("Album", AlbumCount);

        public string LocalizedSongsAndAlbums
            => $"{LocalizedSongCount}, {LocalizedAlbumCount}";
        #endregion

        #region Backend
        /// <summary>
        /// Saves item data to the backend.
        /// </summary>
        public async Task SaveAsync(bool queue = false)
        {
            if (!App.MViewModel.Artists.Contains(this))
            {
                App.MViewModel.Artists.Add(this);
            }

            if (queue)
            {
                NewRepository.Repository.QueueUpsert(Model);
            }
            else
            {
                await NewRepository.Repository.UpsertAsync(Model);
            }
        }

        /// <summary>
        /// Deletes item data from the backend.
        /// </summary>
        public async Task DeleteAsync(bool queue)
        {
            if (App.MViewModel.Artists.Contains(this))
            {
                App.MViewModel.Artists.Remove(this);

                if (queue)
                    NewRepository.Repository.QueueRemove(Model);
                else
                    await NewRepository.Repository.DeleteAsync(Model);
            }
        }

        /// <summary>
        /// Checks whether or not the item is available. If it's not,
        /// delete it.
        /// </summary>
        public async Task CheckAvailabilityAsync(bool queue)
        {
            var songCount = (await NewRepository.Repository.GetItemsAsync<Song>()).Count(s => s.Artist == Model.Name);
            var albumCount = (await NewRepository.Repository.GetItemsAsync<Album>()).Count(a => a.Artist == Model.Name);

            if (songCount == 0 && albumCount == 0)
                await DeleteAsync(queue);
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
            Model = await NewRepository.Repository.GetItemAsync<Artist>(Model.Id);
        }
        #endregion
    }
}
