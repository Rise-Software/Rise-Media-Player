using Rise.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RMP.App.ViewModels
{
    public class GenreViewModel : BaseViewModel
    {
        /// <summary>
        /// Initializes a new instance of the AlbumViewModel class that wraps an Album object.
        /// </summary>
        public GenreViewModel(Genre model = null)
        {
            Model = model ?? new Genre();
            IsNewGenre = true;
        }

        private Genre _model;

        /// <summary>
        /// Gets or sets the underlying Album object.
        /// </summary>
        public Genre Model
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
        /// Gets or sets a value that indicates whether the item has to be deleted.
        /// </summary>
        public bool WillRemove { get; set; }

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

        private bool _isNewGenre;

        /// <summary>
        /// Gets or sets a value that indicates whether this is a new genre.
        /// </summary>
        public bool IsNewGenre
        {
            get => _isNewGenre;
            set => Set(ref _isNewGenre, value);
        }

        /// <summary>
        /// Saves genre data that has been edited.
        /// </summary>
        public async Task SaveAsync()
        {
            IsModified = false;
            if (IsNewGenre)
            {
                IsNewGenre = false;
                App.MViewModel.Genres.Add(this);
            }

            await App.Repository.Genres.UpsertAsync(Model).ConfigureAwait(false);
        }
    }
}
