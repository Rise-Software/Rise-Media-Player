using Rise.Common.Enums;
using Rise.Common.Extensions;
using Rise.Models;
using System;
using Windows.Data.Json;

namespace Rise.Data.ViewModels
{
    public class NavViewItemViewModel : ViewModel<NavViewItem>
    {
        #region Constructors
        public NavViewItemViewModel()
        {
            Model = new NavViewItem();
        }

        public NavViewItemViewModel(NavViewItem item)
        {
            Model = item;
        }

        /// <summary>
        /// Creates an item from a <see cref="JsonObject"/>.
        /// </summary>
        public NavViewItemViewModel(JsonObject jsonObject)
        {
            var item = jsonObject.Deserialize<NavViewItem>();
            Model = item;
        }
        #endregion

        #region Public properties
        /// <summary>
        /// Gets or sets the ID of this item.
        /// </summary>
        public string Id
        {
            get => Model.Id;
            set
            {
                if (Model.Id != value)
                {
                    Model.Id = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the access key of this item.
        /// </summary>
        public string AccessKey
        {
            get => Model.AccessKey;
            set
            {
                if (Model.AccessKey != value)
                {
                    Model.AccessKey = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Header group this item belongs to.
        /// </summary>
        public string HeaderGroup
        {
            get => Model.HeaderGroup;
            set
            {
                if (Model.HeaderGroup != value)
                {
                    Model.HeaderGroup = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Icon for this item. Can be a glyph (\uGLPH format) or
        /// a resource Uri.
        /// </summary>
        public string Icon
        {
            get => Model.Icon;
            set
            {
                if (Model.Icon != value)
                {
                    Model.Icon = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Label for this item.
        /// </summary>
        public string Label
        {
            get => Model.Label;
            set
            {
                if (Model.Label != value)
                {
                    Model.Label = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Whether or not the item's visible.
        /// </summary>
        public bool IsVisible
        {
            get => Model.IsVisible;
            set
            {
                if (Model.IsVisible != value)
                {
                    Model.IsVisible = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Type of item.
        /// </summary>
        public NavViewItemType ItemType
        {
            get
            {
                var result = Enum.TryParse<NavViewItemType>
                    (Model.ItemType.ToString(), out var type);

                if (result)
                {
                    return type;
                }

                return NavViewItemType.Item;
            }
            set
            {
                if (Model.ItemType != (int)value)
                {
                    Model.ItemType = (int)value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Whether or not the item's in the footer.
        /// </summary>
        public bool IsFooter { get; set; }
        #endregion
    }
}
