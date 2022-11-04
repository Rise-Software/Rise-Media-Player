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

        private string _icon;
        /// <summary>
        /// Icon for this item. Can be a glyph (\uGLPH format) or
        /// a resource Uri.
        /// </summary>
        public string Icon
        {
            get => _icon;
            set => Set(ref _icon, value);
        }

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
        /// A set of items contained within this item.
        /// </summary>
        public ObservableCollection<NavViewItemViewModel> SubItems { get; init; }
    }
}
