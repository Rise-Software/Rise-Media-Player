using Rise.App.Common;
using Rise.Models;
using System.Threading.Tasks;

namespace Rise.App.ViewModels
{
    public class GenreViewModel : ViewModel<Genre>
    {
        /// <summary>
        /// Initializes a new instance of the AlbumViewModel class that wraps an Album object.
        /// </summary>
        public GenreViewModel(Genre model = null)
        {
            Model = model ?? new Genre();
            IsNew = true;
        }

        /// <summary>
        /// Gets or sets the genre name.
        /// </summary>
        public string Name
        {
            get
            {
                if (Model.Name == "UnknownGenreResource")
                {
                    return ResourceLoaders.MediaDataLoader.GetString("UnknownGenreResource");
                }

                return Model.Name;
            }
            set
            {
                if (value != Model.Name)
                {
                    Model.Name = value;
                    IsModified = true;
                    OnPropertyChanged(nameof(Name));
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

        private bool _isNew;
        /// <summary>
        /// Gets or sets a value that indicates whether this is a new item.
        /// </summary>
        public bool IsNew
        {
            get => _isNew;
            set => Set(ref _isNew, value);
        }

        /// <summary>
        /// Saves genre data that has been edited.
        /// </summary>
        public async Task SaveAsync()
        {
            IsModified = false;
            if (IsNew)
            {
                IsNew = false;
                App.MViewModel.Genres.Add(this);
            }

            await App.Repository.Genres.QueueUpsertAsync(Model);
        }
    }
}
