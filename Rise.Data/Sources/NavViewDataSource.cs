using CommunityToolkit.Mvvm.Input;
using Rise.Common.Enums;
using Rise.Common.Extensions;
using Rise.Data.Navigation;
using Rise.Data.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Windows.Foundation;
using Windows.Storage;

namespace Rise.Data.Sources
{
    public sealed partial class NavViewDataSource
    {
        private const string _fileName = "NavViewItemData.json";

        /// <summary>
        /// Contains all the items in the data source.
        /// </summary>
        public NavigationItemCollection AllItems { get; private set; }
    }

    // Saving/restoring item state
    public partial class NavViewDataSource
    {
        /// <summary>
        /// Populates the <see cref="AllItems"/> collection with
        /// data from a JSON file.
        /// </summary>
        public void PopulateGroups()
        {
            // No need to populate groups more than once
            if (AllItems != null)
                return;

            var localFolder = ApplicationData.Current.LocalFolder;
            var file = localFolder.TryGetItemAsync(_fileName).Get() as StorageFile;

            // If the file doesn't exist, get data from the placeholder
            if (file == null)
            {
                var baseFile = StorageFile.GetFileFromApplicationUriAsync(new($"ms-appx:///Assets/{_fileName}")).Get();
                file = baseFile.CopyAsync(localFolder).Get();
            }

            string jsonText = FileIO.ReadTextAsync(file).Get();
            var items = JsonSerializer.Deserialize<List<NavViewItemViewModel>>(jsonText);

            AllItems = new(items);
        }

        /// <summary>
        /// Serializes the <see cref="AllItems"/> collection to a JSON file.
        /// </summary>
        public IAsyncAction SerializeGroupsAsync()
            => PathIO.WriteTextAsync($"ms-appdata:///local/{_fileName}", JsonSerializer.Serialize(AllItems));
    }

    // Hiding/showing items
    public partial class NavViewDataSource
    {
        /// <summary>
        /// Hides a group of NavigationView items and their header.
        /// </summary>
        /// <param name="group">Group to hide.</param>
        [RelayCommand]
        public void HideGroup(string group)
        {
            foreach (NavViewItemViewModel item in AllItems)
            {
                if (item.HeaderGroup == group)
                {
                    item.IsVisible = false;
                    if (item.ItemType == NavViewItemType.Header)
                        item.HasVisibleChildren = false;
                }
            }
        }

        /// <summary>
        /// Shows a group of NavigationView items and their header.
        /// </summary>
        /// <param name="group">Group to show.</param>
        public void ShowGroup(string group)
        {
            foreach (NavViewItemViewModel item in AllItems)
            {
                if (item.HeaderGroup == group)
                {
                    item.IsVisible = true;
                    if (item.ItemType == NavViewItemType.Header)
                        item.HasVisibleChildren = true;
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
            _ = TryGetItem(id, out var item);

            item.IsVisible = !item.IsVisible;
            CheckHeaderVisibility(item.HeaderGroup);
        }

        /// <summary>
        /// Changes the visibility of a NavigationView item.
        /// </summary>
        /// <param name="id">Id of the item to change.</param>
        /// <param name="vis">Whether or not the item should be visible.</param>
        public void ChangeItemVisibility(string id, bool vis)
        {
            _ = TryGetItem(id, out var item);

            item.IsVisible = vis;
            CheckHeaderVisibility(item.HeaderGroup);
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
                foreach (NavViewItemViewModel item in AllItems)
                {
                    if (item.ItemType == NavViewItemType.Item &&
                        item.HeaderGroup == group &&
                        item.IsVisible)
                    {
                        // An item is visible, no need to hide header
                        header.HasVisibleChildren = true;
                        return;
                    }
                }

                header.HasVisibleChildren = false;
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
            _ = TryGetItem(id, out var item);
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
                if (item.HeaderGroup == groupName && item.IsVisible)
                    return true;
            }

            return false;
        }
    }

    // Moving items
    public partial class NavViewDataSource
    {
        private void MoveItem(string id, int offset)
        {
            _ = TryGetItem(id, out var item);

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
            _ = TryGetItem(id, out var item);

            int index = AllItems.IndexOf(item);
            if (index == 0)
                return false;

            var elm = AllItems.ElementAt(index - 1);
            bool sameGroup = elm.HeaderGroup == item.HeaderGroup;
            bool directlyBelowHeader = sameGroup && elm.ItemType == NavViewItemType.Header;

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
            _ = TryGetItem(id, out var item);

            int index = AllItems.IndexOf(item) + 1;
            if (index == AllItems.Count)
                return false;

            var elm = AllItems.ElementAt(index);
            return elm.HeaderGroup == item.HeaderGroup;
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
            _ = TryGetItem(id, out var item);

            int index = AllItems.IndexOf(item);
            if (item.HeaderGroup == "General")
            {
                AllItems.Move(index, 0);
            }
            else
            {
                var header = HeaderFromGroupName(item.HeaderGroup);
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
            _ = TryGetItem(id, out var item);

            int index = AllItems.IndexOf(item);

            var lastInGroup = AllItems.LastOrDefault(i => i.HeaderGroup == item.HeaderGroup);
            AllItems.Move(index, AllItems.IndexOf(lastInGroup));
        }
    }

    // Getting items
    public partial class NavViewDataSource
    {
        /// <summary>
        /// Tries to get an item with the specified ID.
        /// </summary>
        /// <param name="id">ID of the item.</param>
        /// <param name="item">The item if found.</param>
        /// <returns>true if the item is found, false otherwise.</returns>
        public bool TryGetItem(string id, out NavViewItemViewModel item)
        {
            item = AllItems.FirstOrDefault(i => i.Id.Equals(id));
            return item != null;
        }

        /// <summary>
        /// Gets a header based on its group name.
        /// </summary>
        /// <param name="group">The header's group name.</param>
        /// <returns>The header with the specified group name.</returns>
        public NavViewItemViewModel HeaderFromGroupName(string group)
            => AllItems.FirstOrDefault(i => i.ItemType == NavViewItemType.Header && i.HeaderGroup == group);
    }
}
