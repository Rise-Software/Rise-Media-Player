using System;

namespace Rise.App.ViewModels
{
    /// <summary>
    /// Data for NavView items.
    /// </summary>
    public class NavViewItemViewModel : ViewModel, IEquatable<NavViewItemViewModel>
    {
        private string _label;
        public string LabelResource
        {
            get => _label;
            set => Set(ref _label, value);
        }

        private string _tag;
        public string Tag
        {
            get => _tag;
            set => Set(ref _tag, value);
        }

        private string _icon;
        public string Icon
        {
            get => _icon;
            set => Set(ref _icon, value);
        }

        private string _headerGroup;
        public string HeaderGroup
        {
            get => _headerGroup;
            set => Set(ref _headerGroup, value);
        }

        private bool _visible;
        public bool Visible
        {
            get => _visible;
            set => Set(ref _visible, value);
        }

        private bool _isFooter;
        public bool IsFooter
        {
            get => _isFooter;
            set => Set(ref _isFooter, value);
        }

        private string _accKey;
        public string AccKey
        {
            get => _accKey;
            set => Set(ref _accKey, value);
        }

        public bool Equals(NavViewItemViewModel other)
        {
            return other.Tag == Tag;
        }
    }
}
