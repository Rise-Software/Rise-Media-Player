using Rise.Models;

namespace Rise.App.ViewModels
{
    public class VideoViewModel : ViewModel<Video>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VideoViewModel"/>
        /// class that wraps a <see cref="Video"/> object.
        /// </summary>
        public VideoViewModel(Video model = null)
        {
            Model = model ?? new Video();
            IsNew = true;
        }

        /// <summary>
        /// Gets or sets the video title.
        /// </summary>
        public string Title
        {
            get => Model.Title;
            set
            {
                if (Model.Title != value)
                {
                    Model.Title = value;
                    OnPropertyChanged(nameof(Title));
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

        private bool _isInEdit;
        /// <summary>
        /// Gets or sets a value that indicates whether the item data is being edited.
        /// </summary>
        public bool IsInEdit
        {
            get => _isInEdit;
            set => Set(ref _isInEdit, value);
        }
    }
}
