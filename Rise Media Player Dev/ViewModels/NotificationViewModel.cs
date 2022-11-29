using Rise.Data.ViewModels;
using Rise.Models;

namespace Rise.App.ViewModels
{
    public class NotificationViewModel : ViewModel<Notification>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationViewModel"/> class that wraps a <see cref="Notification"/> object.
        /// </summary>
        public NotificationViewModel(Notification model = null)
        {
            Model = model ?? new Notification();
        }

        /// <summary>
        /// Gets or sets the notification title.
        /// </summary>
        public string Title
        {
            get
            {
                return Model.Title;
            }
            set
            {
                if (value != Model.Title)
                {
                    Model.Title = value;
                    OnPropertyChanged(nameof(Title));
                }
            }
        }

        /// <summary>
        /// Gets or sets the notification description.
        /// </summary>
        public string Description
        {
            get
            {
                return Model.Description;
            }
            set
            {
                if (value != Model.Description)
                {
                    Model.Description = value;
                    OnPropertyChanged(nameof(Description));
                }
            }
        }

        /// <summary>
        /// Gets or sets the notification icon.
        /// </summary>
        public string Icon
        {
            get
            {
                return Model.Icon;
            }
            set
            {
                if (value != Model.Icon)
                {
                    Model.Icon = value;
                    OnPropertyChanged(nameof(Icon));
                }
            }
        }
    }
}
