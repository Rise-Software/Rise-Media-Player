using CommunityToolkit.Mvvm.Input;
using Rise.Common.Extensions;
using Rise.Data.Navigation;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Windows.Foundation;
using Windows.Storage;

namespace Rise.Data.Sources
{
    public sealed partial class NavViewDataSource
    {
        private const string _fileName = "NavigationItemData.json";

        /// <summary>
        /// Contains all the items in the data source.
        /// </summary>
        public NavigationItemCollection AllItems { get; private set; }
    }

    // Saving/restoring item state
    public sealed partial class NavViewDataSource
    {
        private bool _populated = false;
        /// <summary>
        /// Populates the <see cref="AllItems"/> collection.
        /// </summary>
        public void PopulateGroups()
        {
            // No need to populate groups more than once
            if (_populated)
                return;
            _populated = true;

            var localFolder = ApplicationData.Current.LocalFolder;
            var file = localFolder.TryGetItemAsync(_fileName).Get() as StorageFile;

            // If the file doesn't exist, use the default items, and create
            // the JSON file
            if (file == null)
            {
                _ = localFolder.CreateFileAsync(_fileName).Get();
                AllItems = new(_defaultItems);

                return;
            }

            string jsonText = FileIO.ReadTextAsync(file).Get();
            var items = JsonSerializer.Deserialize<List<NavigationItemBase>>(jsonText);

            // Remove items that shouldn't be there
            items.RemoveAll(i => !_defaultItems.Contains(i));

            // Add new items
            for (int i = 0; i < _defaultItems.Length; i++)
            {
                var item = _defaultItems[i];
                if (!items.Contains(item))
                {
                    bool isHeader = item.ItemType == NavigationItemType.Header;

                    int index;
                    if (isHeader)
                        index = items.FindIndex(i => i.Group == item.Group && i.IsFooter == item.IsFooter);
                    else
                        index = items.FindLastIndex(i => i.Group == item.Group && i.IsFooter == item.IsFooter);

                    // If there's no group yet, add the item at
                    // the end of the previous group
                    if (index == -1)
                    {
                        items.Insert(i, item);
                        continue;
                    }

                    if (isHeader)
                        items.Insert(index, item);
                    else
                        items.Insert(index + 1, item);
                }
            }

            AllItems = new(items);
        }

        /// <summary>
        /// Serializes the <see cref="AllItems"/> collection to a JSON file.
        /// </summary>
        public IAsyncAction SerializeGroupsAsync()
            => PathIO.WriteTextAsync($"ms-appdata:///local/{_fileName}", JsonSerializer.Serialize(AllItems));
    }

    // Showing/hiding items
    public sealed partial class NavViewDataSource
    {
        /// <summary>
        /// Toggles the visibility of the provided item and updates the
        /// visibility of its group header if necessary.
        /// </summary>
        [RelayCommand]
        public void ToggleItemVisibility(NavigationItemBase item)
            => ChangeItemVisibility(item, !item.IsVisible);

        /// <summary>
        /// Changes the visibility of the provided item and updates the
        /// visibility of its group header if necessary.
        /// </summary>
        /// <param name="vis">Whether the item should be visible.</param>
        public void ChangeItemVisibility(NavigationItemBase item, bool vis)
        {
            item.IsVisible = vis;
            if (GetItem(item.Group) is NavigationItemHeader header)
            {
                if (vis)
                    header.IsGroupVisible = true;
                else if (item.ItemType != NavigationItemType.Header)
                    HideIfNeeded(header);
            }
        }

        private void HideIfNeeded(NavigationItemHeader header)
        {
            string group = header.Group;
            foreach (NavigationItemBase item in AllItems)
            {
                if (item.ItemType == NavigationItemType.Destination &&
                    item.Group == group &&
                    item.IsVisible)
                {
                    // An item is visible, no need to hide header
                    return;
                }
            }

            header.IsGroupVisible = false;
            header.IsVisible = false;
        }

        /// <summary>
        /// Shows a group of NavigationView items and their header.
        /// </summary>
        /// <param name="group">Group to show.</param>
        public void ShowGroup(string group)
        {
            foreach (NavigationItemBase item in AllItems)
            {
                if (item.Group == group)
                {
                    item.IsVisible = true;
                    if (item is NavigationItemHeader header)
                        header.IsGroupVisible = true;
                }
            }
        }

        /// <summary>
        /// Hides a group of NavigationView items and their header.
        /// </summary>
        /// <param name="group">Group to hide.</param>
        [RelayCommand]
        public void HideGroup(string group)
        {
            foreach (NavigationItemBase item in AllItems)
            {
                if (item.Group == group)
                {
                    item.IsVisible = false;
                    if (item is NavigationItemHeader header)
                        header.IsGroupVisible = false;
                }
            }
        }

        /// <summary>
        /// Checks if any items in a header group are shown.
        /// </summary>
        /// <param name="groupName">Group name to check for.</param>
        /// <returns>true if any of the items in the group are shown;
        /// false otherwise.</returns>
        public bool IsGroupShown(string groupName)
        {
            foreach (var item in AllItems)
            {
                if (item.Group == groupName && item.IsVisible)
                    return true;
            }

            return false;
        }
    }

    // Moving items
    public sealed partial class NavViewDataSource
    {
        /// <summary>
        /// Checks whether an item can be moved up without going under another
        /// group's header or out of bounds.
        /// </summary>
        /// <returns>true if the item can be moved up, false otherwise.</returns>
        public bool CanMoveUp(NavigationItemBase item)
        {
            int index = AllItems.IndexOf(item);
            if (index == 0)
                return false;

            var elm = AllItems.ElementAt(index - 1);
            bool sameGroup = elm.Group == item.Group;
            bool directlyBelowHeader = sameGroup && elm.ItemType == NavigationItemType.Header;

            return sameGroup && !directlyBelowHeader;
        }

        /// <summary>
        /// Checks whether an item can be moved down without going under another
        /// group's header or out of bounds.
        /// </summary>
        /// <returns>true if the item can be moved down, false otherwise.</returns>
        public bool CanMoveDown(NavigationItemBase item)
        {
            int index = AllItems.IndexOf(item) + 1;
            if (index == AllItems.Count)
                return false;

            var elm = AllItems.ElementAt(index);
            return elm.Group == item.Group;
        }

        /// <summary>
        /// Moves the provided item up.
        /// </summary>
        [RelayCommand]
        public void MoveUp(NavigationItemBase item)
            => MoveItem(item, -1);

        /// <summary>
        /// Moves the provided item down.
        /// </summary>
        [RelayCommand]
        public void MoveDown(NavigationItemBase item)
            => MoveItem(item, 1);

        /// <summary>
        /// Moves the provided item to the top of its group.
        /// </summary>
        [RelayCommand]
        public void MoveToTop(NavigationItemBase item)
        {
            var header = GetItem(item.Group);
            var items = AllItems.GetView(item.IsFooter);

            // If the header is null, the index will be -1, but thanks to the
            // addition, the item will get inserted at the beginning
            AllItems.Move(item, items.IndexOf(header) + 1);
        }

        /// <summary>
        /// Moves the provided item to the bottom of its group.
        /// </summary>
        [RelayCommand]
        public void MoveToBottom(NavigationItemBase item)
        {
            var items = AllItems.GetView(item.IsFooter);
            var lastInGroup = items.LastOrDefault(i => i.Group == item.Group);

            AllItems.Move(item, items.IndexOf(lastInGroup));
        }

        private void MoveItem(NavigationItemBase item, int offset)
        {
            int index = AllItems.IndexOf(item);
            AllItems.Move(index, index + offset);
        }
    }

    // Getting items
    public sealed partial class NavViewDataSource
    {
        /// <summary>
        /// Gets the item with the specified ID.
        /// </summary>
        /// <param name="id">ID of the item.</param>
        public NavigationItemBase GetItem(string id)
            => AllItems.FirstOrDefault(i => i.Id.Equals(id));
    }
}
