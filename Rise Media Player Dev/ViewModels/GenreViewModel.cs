using Rise.App.Common;
using Rise.Common.Interfaces;
using Rise.Data.ViewModels;
using Rise.Models;
using Rise.Repository.SQL;
using System.Threading.Tasks;

namespace Rise.App.ViewModels
{
    public class GenreViewModel : ViewModel<Genre>
    {
        private ISQLRepository<Genre> Repository => SQLRepository.Repository.Genres;

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the AlbumViewModel class that wraps an Album object.
        /// </summary>
        public GenreViewModel(Genre model = null)
        {
            Model = model ?? new Genre();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the genre name.
        /// </summary>
        public string Name
        {
            get => Model.Name == "UnknownGenreResource" ? ResourceLoaders.MediaDataLoader.GetString("UnknownGenreResource") : Model.Name;
            set
            {
                if (value != Model.Name)
                {
                    Model.Name = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        #region Backend
        /// <summary>
        /// Saves item data to the backend.
        /// </summary>
        public async Task SaveAsync()
        {
            bool hasMatch = await Repository.CheckForMatchAsync(Model);
            if (!hasMatch)
            {
                App.MViewModel.Genres.Add(this);
                await Repository.QueueUpsertAsync(Model);
            }
            else
            {
                await Repository.UpdateAsync(Model);
            }
        }
        #endregion
    }
}
