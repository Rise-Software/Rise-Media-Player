using Rise.App.Common;
using Rise.Models;
using Rise.Repository.SQL;
using System.Threading.Tasks;

namespace Rise.App.ViewModels
{
    public class GenreViewModel : ViewModel<Genre>
    {
        #region Constructor
        /// <summary>
        /// Initializes a new instance of the AlbumViewModel class that wraps an Album object.
        /// </summary>
        public GenreViewModel(Genre model = null)
        {
            Model = model ?? new Genre();
            IsNew = true;
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
        /// Saves genre data that has been edited.
        /// </summary>
        public async Task SaveAsync()
        {
            if (IsNew)
            {
                IsNew = false;
                App.MViewModel.Genres.Add(this);
            }

            await SQLRepository.Repository.Genres.QueueUpsertAsync(Model);
        }
        #endregion
    }
}
