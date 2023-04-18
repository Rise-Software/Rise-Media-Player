using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Rise.Data.Navigation
{
    /// <summary>
    /// A collection of items to be displayed in a NavigationView. For binding
    /// purposes, it exposes observable views for main and footer items, and
    /// handles external modifications as needed.
    /// </summary>
    /// <remarks>This collection isn't observable on its own - the menu and
    /// footer views are, despite being read-only.</remarks>
    public sealed class NavigationItemCollection : Collection<NavigationItemBase>
    {
        private readonly ObservableCollection<NavigationItemBase> _menuItems;
        /// <summary>
        /// Gets an observable view of the items in the main section.
        /// </summary>
        public ReadOnlyObservableCollection<NavigationItemBase> MenuItems { get; }

        private readonly ObservableCollection<NavigationItemBase> _footerItems;
        /// <summary>
        /// Gets an observable view of the items in the footer section.
        /// </summary>
        public ReadOnlyObservableCollection<NavigationItemBase> FooterItems { get; }

        public NavigationItemCollection()
            : base()
        {
            _menuItems = new();
            _footerItems = new();

            MenuItems = new(_menuItems);
            FooterItems = new(_footerItems);
        }

        public NavigationItemCollection(IList<NavigationItemBase> list)
            : base(list)
        {
            _menuItems = new(list.Where(i => !i.IsFooter));
            _footerItems = new(list.Where(i => i.IsFooter));

            MenuItems = new(_menuItems);
            FooterItems = new(_footerItems);
        }

        protected override void InsertItem(int index, NavigationItemBase item)
        {
            base.InsertItem(index, item);

            int menuCount = _menuItems.Count;
            if (index >= menuCount)
                _footerItems.Insert(index - menuCount, item);
            else
                _menuItems.Insert(index, item);
        }

        protected override void RemoveItem(int index)
        {
            base.RemoveItem(index);

            int menuCount = _menuItems.Count;
            if (index >= menuCount)
                _footerItems.RemoveAt(index - menuCount);
            else
                _menuItems.RemoveAt(index);
        }

        protected override void SetItem(int index, NavigationItemBase item)
        {
            base.SetItem(index, item);

            int menuCount = _menuItems.Count;
            if (index >= menuCount)
                _footerItems[index - menuCount] = item;
            else
                _menuItems[index] = item;
        }

        protected override void ClearItems()
        {
            base.ClearItems();

            _menuItems.Clear();
            _footerItems.Clear();
        }
    }
}
