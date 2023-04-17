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

    // Hiding/showing items
    public sealed partial class NavViewDataSource
    {
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
        /// Toggles the visibility of a NavigationView item.
        /// </summary>
        /// <param name="id">Id of the item to change.</param>
        [RelayCommand]
        public void ToggleItemVisibility(string id)
        {
            var item = GetItem(id);

            item.IsVisible = !item.IsVisible;
            CheckHeaderVisibility(item.Group);
        }

        /// <summary>
        /// Changes the visibility of a NavigationView item.
        /// </summary>
        /// <param name="id">Id of the item to change.</param>
        /// <param name="vis">Whether or not the item should be visible.</param>
        public void ChangeItemVisibility(string id, bool vis)
        {
            var item = GetItem(id);

            item.IsVisible = vis;
            CheckHeaderVisibility(item.Group);
        }

        /// <summary>
        /// Changes the visibility of NavigationView headers based
        /// on the visibility of the items in its group.
        /// </summary>
        /// <param name="group">Header group to check.</param>
        public void CheckHeaderVisibility(string group)
        {
            var header = HeaderFromGroupName(group);
            if (header != null)
            {
                foreach (NavigationItemBase item in AllItems)
                {
                    if (item.ItemType == NavigationItemType.Destination &&
                        item.Group == group &&
                        item.IsVisible)
                    {
                        // An item is visible, no need to hide header
                        header.IsGroupVisible = true;
                        return;
                    }
                }

                header.IsGroupVisible = false;
                header.IsVisible = false;
            }
        }

        /// <summary>
        /// Changes the visibility of a NavigationView header.
        /// </summary>
        /// <param name="groupName">Group name of the header to change.</param>
        /// <param name="vis">Whether or not the header should be visible.</param>
        public void ChangeHeaderVisibility(string groupName, bool vis)
            => HeaderFromGroupName(groupName).IsVisible = vis;

        /// <summary>
        /// Whether or not is an item visible.
        /// </summary>
        /// <param name="id">Id of the item to check.</param>
        /// <returns>Whether or not is the item visible.</returns>
        public bool IsItemVisible(string id)
        {
            var item = GetItem(id);
            return item.IsVisible;
        }

        /// <summary>
        /// Whether or not is a header visible.
        /// </summary>
        /// <param name="groupName">Group name of the header to check.</param>
        /// <returns>Whether or not is the item visible.</returns>
        public bool IsHeaderVisible(string groupName)
            => HeaderFromGroupName(groupName).IsVisible;

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
        private void MoveItem(string id, int offset)
        {
            var item = GetItem(id);

            int index = AllItems.IndexOf(item);
            AllItems.Move(index, index + offset);
        }

        /// <summary>
        /// Checks if an item can be moved up.
        /// </summary>
        /// <param name="id">Item's Id.</param>
        /// <returns>True if the item can be moved up,
        /// false otherwise.</returns>
        public bool CanMoveUp(string id)
        {
            var item = GetItem(id);

            int index = AllItems.IndexOf(item);
            if (index == 0)
                return false;

            var elm = AllItems.ElementAt(index - 1);
            bool sameGroup = elm.Group == item.Group;
            bool directlyBelowHeader = sameGroup && elm.ItemType == NavigationItemType.Header;

            return sameGroup && !directlyBelowHeader;
        }

        /// <summary>
        /// Checks if an item can be moved down.
        /// </summary>
        /// <param name="id">Item's Id.</param>
        /// <returns>True if the item can be moved down,
        /// false otherwise.</returns>
        public bool CanMoveDown(string id)
        {
            var item = GetItem(id);

            int index = AllItems.IndexOf(item) + 1;
            if (index == AllItems.Count)
                return false;

            var elm = AllItems.ElementAt(index);
            return elm.Group == item.Group;
        }

        /// <summary>
        /// Moves an item up.
        /// </summary>
        /// <param name="id">Item's Id.</param>
        [RelayCommand]
        public void MoveUp(string id)
            => MoveItem(id, -1);

        /// <summary>
        /// Moves an item down.
        /// </summary>
        /// <param name="id">Item's Id.</param>
        [RelayCommand]
        public void MoveDown(string id)
            => MoveItem(id, 1);

        /// <summary>
        /// Moves an item to the top.
        /// </summary>
        /// <param name="id">Item's Id.</param>
        [RelayCommand]
        public void MoveToTop(string id)
        {
            var item = GetItem(id);

            int index = AllItems.IndexOf(item);
            if (item.Group == "General")
            {
                AllItems.Move(index, 0);
            }
            else
            {
                var header = HeaderFromGroupName(item.Group);
                AllItems.Move(index, AllItems.IndexOf(header) + 1);
            }
        }

        /// <summary>
        /// Moves an item to the bottom.
        /// </summary>
        /// <param name="id">Item's Id.</param>
        [RelayCommand]
        public void MoveToBottom(string id)
        {
            var item = GetItem(id);

            int index = AllItems.IndexOf(item);

            var lastInGroup = AllItems.LastOrDefault(i => i.Group == item.Group);
            AllItems.Move(index, AllItems.IndexOf(lastInGroup));
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

        /// <summary>
        /// Tries to get an item with the specified ID.
        /// </summary>
        /// <param name="id">ID of the item.</param>
        /// <param name="item">The item if found.</param>
        /// <returns>true if the item is found, false otherwise.</returns>
        public bool TryGetItem(string id, out NavigationItemBase item)
        {
            item = AllItems.FirstOrDefault(i => i.Id.Equals(id));
            return item != null;
        }

        /// <summary>
        /// Gets a header based on its group name.
        /// </summary>
        /// <param name="group">The header's group name.</param>
        /// <returns>The header with the specified group name.</returns>
        public NavigationItemHeader HeaderFromGroupName(string group)
            => (NavigationItemHeader)AllItems.FirstOrDefault(i => i.ItemType == NavigationItemType.Header && i.Group == group);
    }
}
