using Rise.Common.Enums;
using System.Collections.ObjectModel;

namespace Rise.Data.ViewModels
{
    /// <summary>
    /// Represents a destination on a NavigationView control.
    /// </summary>
    public sealed class NavViewItemViewModel : ViewModel
    {
        /// <summary>
        /// Gets a unique identifier for this item.
        /// </summary>
        public string Id { get; init; }

        /// <summary>
        /// Indicates the type of item.
        /// </summary>
        public NavViewItemType ItemType { get; init; }

        /// <summary>
        /// Icon to use for this item by default.
        /// </summary>
        public string DefaultIcon { get; init; }

        private string _label;
        /// <summary>
        /// Label for this item.
        /// </summary>
        public string Label
        {
            get => _label;
            set => Set(ref _label, value);
        }

        /// <summary>
        /// Gets an access key for this item.
        /// </summary>
        public string AccessKey { get; init; }

        /// <summary>
        /// An identifier for the flyout this item requests.
        /// </summary>
        public string FlyoutId { get; init; }

        /// <summary>
        /// Header group this item belongs to.
        /// </summary>
        public string HeaderGroup { get; init; }

        /// <summary>
        /// Whether the item should be in the footer.
        /// </summary>
        public bool IsFooter { get; init; }

        private bool _isVisible = true;
        /// <summary>
        /// Whether the item should be visible.
        /// </summary>
        public bool IsVisible
        {
            get => _isVisible;
            set => Set(ref _isVisible, value);
        }

        /// <summary>
        /// The parent item's Id. Only applies if <see cref="ItemType"/> is
        /// <see cref="NavViewItemType.SubItem"/>.
        /// </summary>
        public string ParentId { get; init; }

        /// <summary>
        /// A set of items contained within this item.
        /// </summary>
        public ObservableCollection<NavViewItemViewModel> SubItems { get; init; }
    }
}
