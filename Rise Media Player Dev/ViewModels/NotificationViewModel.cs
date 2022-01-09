using Rise.Models;
using System;
using System.Threading.Tasks;
using Windows.Storage;

namespace Rise.App.ViewModels
{
    public class NotificationViewModel : ViewModel<Notification>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationViewModel"/> class that wraps a <see cref="Notification"/> object.
        /// </summary>
        public NotificationViewModel(Notification model = null)
        {
            if (model != null)
            {
                Model = model;
            }
            else
            {
                Model = new Notification();
                IsNew = true;
            }
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
                    IsModified = true;
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
                    IsModified = true;
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
                    IsModified = true;
                    OnPropertyChanged(nameof(Icon));
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

        private bool _isLoading;
        /// <summary>
        /// Gets or sets a value that indicates whether to show a progress bar. 
        /// </summary>
        public bool IsLoading
        {
            get => _isLoading;
            set => Set(ref _isLoading, value);
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

        private bool _isInEdit = false;

        /// <summary>
        /// Gets or sets a value that indicates whether the notification data is being edited.
        /// </summary>
        public bool IsInEdit
        {
            get => _isInEdit;
            set => Set(ref _isInEdit, value);
        }

        /// <summary>
        /// Saves notification data that has been edited.
        /// </summary>
        public async Task SaveAsync()
        {
            IsInEdit = false;
            IsModified = false;

            if (IsNew)
            {
                IsNew = false;
                App.MViewModel.Notifications.Add(this);
            }

            await App.NBackendController.InsertAsync(this);
        }

        /// <summary>
        /// Delete notification from repository and MViewModel.
        /// </summary>
        public async Task DeleteAsync()
        {
            IsModified = true;

            App.MViewModel.Notifications.Remove(this);
            await App.NBackendController.DeleteAsync(this);
        }
    }
}
