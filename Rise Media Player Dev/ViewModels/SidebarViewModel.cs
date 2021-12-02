using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.Storage;

namespace Rise.App.ViewModels
{
    public class SidebarViewModel : ViewModel
    {
        private const string Filename = "_itemData.json";

        public ObservableCollection<NavViewItemViewModel> Items { get; set; } =
            new ObservableCollection<NavViewItemViewModel>();

        public ObservableCollection<NavViewItemViewModel> FooterItems { get; set; } =
            new ObservableCollection<NavViewItemViewModel>();

        private readonly Dictionary<string, string> _defaultIcons =
            new Dictionary<string, string>();

        public SidebarViewModel()
        {
            // Populate the default icon dictionary.
            _defaultIcons.Add("HomePage", "\uECA5");
            _defaultIcons.Add("NowPlayingPage", "\uEA37");
            _defaultIcons.Add("PlaylistsPage", "\uE8FD");
            _defaultIcons.Add("SongsPage", "\uEC4F");
            _defaultIcons.Add("ArtistsPage", "\uE125");
            _defaultIcons.Add("AlbumsPage", "\uE93C");
            _defaultIcons.Add("GenresPage", "\uE138");
            _defaultIcons.Add("LocalVideosPage", "\uE8B2");
            _defaultIcons.Add("VideoPlaybackPage", "\uE1D9");
            _defaultIcons.Add("DiscyPage", "\uE9CE");
            _defaultIcons.Add("SettingsPage", "\uE115");
        }

        /// <summary>
        /// Loads sidebar items inside the JSON file. If the file's
        /// not present, load the placeholder JSON.
        /// </summary>
        public async Task LoadItemsAsync()
        {
            Items.Clear();
            FooterItems.Clear();

            string path = Path.Combine(ApplicationData.
                Current.LocalFolder.Path, Filename);

            StorageFile file;

            // In case the file was deleted, or just isn't there yet.
            if (!File.Exists(path))
            {
                Uri uriPath = new Uri("ms-appx:///Assets/SampleItemData.json");

                StorageFile placeholder =
                    await StorageFile.GetFileFromApplicationUriAsync(uriPath);

                file = await placeholder.CopyAsync(ApplicationData.
                    Current.LocalFolder, Filename, NameCollisionOption.ReplaceExisting);
            }
            else
            {
                file = await ApplicationData.Current.LocalFolder.
                    GetFileAsync(Filename);
            }

            string jsonText = await FileIO.ReadTextAsync(file);
            _ = JsonArray.
                TryParse(jsonText, out JsonArray jsonArray);

            foreach (JsonValue groupValue in jsonArray)
            {
                JsonObject groupObject = groupValue.GetObject();

                string tag = groupObject["Tag"].GetString();
                bool footer = groupObject["IsFooter"].GetBoolean();
                bool visible = groupObject["Visible"].GetBoolean();

                NavViewItemViewModel item = new NavViewItemViewModel
                {
                    Tag = tag,
                    IsFooter = footer,
                    Visible = visible
                };

                if (!tag.Equals("Separator"))
                {
                    string label = groupObject["LabelResource"].GetString();
                    string group = groupObject["HeaderGroup"].GetString();
                    item.LabelResource = label;
                    item.HeaderGroup = group;

                    if (!tag.Equals("Header"))
                    {
                        string iconStr = groupObject["Icon"].GetString();
                        string accKey = groupObject["AccKey"].GetString();
                        item.Icon = iconStr;
                        item.AccKey = accKey;
                    }
                }

                if (footer)
                {
                    FooterItems.Add(item);
                }
                else
                {
                    Items.Add(item);
                }
            }
        }

        /// <summary>
        /// Serializes the items to a JSON file to save their state.
        /// </summary>
        public async Task SerializeItemsAsync()
        {
            List<NavViewItemViewModel> items = new List<NavViewItemViewModel>();
            foreach (NavViewItemViewModel item in Items)
            {
                items.Add(item);
            }

            foreach (NavViewItemViewModel item in FooterItems)
            {
                items.Add(item);
            }

            StorageFile file = await ApplicationData.Current.LocalFolder.
                CreateFileAsync(Filename, CreationCollisionOption.ReplaceExisting);

            string json = JsonConvert.SerializeObject(items);
            await FileIO.WriteTextAsync(file, json);
        }

        /// <summary>
        /// Changes the visibility of NavigationView headers based
        /// on the visibility of its items.
        /// </summary>
        /// <param name="group">Header group to check.</param>
        /// <param name="inFooter">Whether or not the header and its
        /// items are in the footer.</param>
        public void CheckHeaderVisibility(string group, bool inFooter = false)
        {
            if (group.Equals("General"))
            {
                return;
            }

            if (!inFooter)
            {
                NavViewItemViewModel header = Items.
                    First(i => i.Tag == "Header" && i.HeaderGroup == group);

                if (header != null)
                {
                    foreach (NavViewItemViewModel item in Items)
                    {
                        // Make sure the item isn't a header.
                        if (!item.Tag.Equals("Header") && !item.Tag.Equals("Separator"))
                        {
                            if (item.HeaderGroup.Equals(group))
                            {
                                if (item.Visible)
                                {
                                    // An item is visible, no need to hide header.
                                    header.Visible = true;
                                    return;
                                }
                            }
                        }
                    }

                    header.Visible = false;
                }
            }
            else
            {
                NavViewItemViewModel header = FooterItems.
                    First(i => i.Tag == "Header" && i.HeaderGroup == group);

                if (header != null)
                {
                    foreach (NavViewItemViewModel item in FooterItems)
                    {
                        // Make sure the item isn't a header.
                        if (!item.Tag.Equals("Header") && !item.Tag.Equals("Separator"))
                        {
                            if (item.HeaderGroup.Equals(group))
                            {
                                if (item.Visible)
                                {
                                    // An item is visible, no need to hide header.
                                    header.Visible = true;
                                    return;
                                }
                            }
                        }
                    }

                    header.Visible = false;
                }
            }
        }

        /// <summary>
        /// Changes the currently applied icon pack.
        /// </summary>
        /// <param name="newName">Name of the new icon pack. If null or "Default",
        /// go back to the default icons.</param>
        public void ChangeIconPack(string newName = null)
        {
            if (newName != null && !newName.Equals("Default"))
            {
                string startingUri = "ms-appx:///Assets/NavigationView/";
                foreach (NavViewItemViewModel item in Items)
                {
                    if (!item.Tag.Equals("Header") && !item.Tag.Equals("Separator"))
                    {
                        string newUri = startingUri + item.Tag + "/" + newName + ".png";
                        item.Icon = newUri;
                    }
                }

                foreach (NavViewItemViewModel item in FooterItems)
                {
                    if (!item.Tag.Equals("Header") && !item.Tag.Equals("Separator"))
                    {
                        string newUri = startingUri + item.Tag + "/" + newName + ".png";
                        item.Icon = newUri;
                    }
                }
            }
            else
            {
                foreach (NavViewItemViewModel item in Items)
                {
                    if (!item.Tag.Equals("Header") && !item.Tag.Equals("Separator"))
                    {
                        item.Icon = _defaultIcons[item.Tag];
                    }
                }

                foreach (NavViewItemViewModel item in FooterItems)
                {
                    if (!item.Tag.Equals("Header") && !item.Tag.Equals("Separator"))
                    {
                        item.Icon = _defaultIcons[item.Tag];
                    }
                }
            }
        }

        /// <summary>
        /// Hides a group of NavigationView items and their header.
        /// </summary>
        /// <param name="group">Group to hide.</param>
        public void HideGroup(string group)
        {
            foreach (NavViewItemViewModel item in Items)
            {
                if (item.HeaderGroup.Equals(group))
                {
                    item.Visible = false;
                }
            }

            foreach (NavViewItemViewModel item in FooterItems)
            {
                if (item.HeaderGroup.Equals(group))
                {
                    item.Visible = false;
                }
            }
        }

        /// <summary>
        /// Shows a group of NavigationView items and their header.
        /// </summary>
        /// <param name="group">Group to show.</param>
        public void ShowGroup(string group)
        {
            foreach (NavViewItemViewModel item in Items)
            {
                if (item.HeaderGroup.Equals(group))
                {
                    item.Visible = true;
                }
            }

            foreach (NavViewItemViewModel item in FooterItems)
            {
                if (item.HeaderGroup.Equals(group))
                {
                    item.Visible = true;
                }
            }
        }

        /// <summary>
        /// Changes the visibility of a NavigationView item.
        /// </summary>
        /// <param name="tag">Tag of the item to change.</param>
        /// <param name="vis">Whether or not the item should be visible.</param>
        public void ChangeItemVisibility(string tag, bool vis)
        {
            NavViewItemViewModel item = ItemFromTag(tag);

            item.Visible = vis;
            CheckHeaderVisibility(item.HeaderGroup);
        }

        /// <summary>
        /// Changes the visibility of a NavigationView header.
        /// </summary>
        /// <param name="groupName">Group name of the header to change.</param>
        /// <param name="vis">Whether or not the header should be visible.</param>
        public void ChangeHeaderVisibility(string groupName, bool vis)
            => HeaderFromGroupName(groupName).Visible = vis;

        /// <summary>
        /// Whether or not is an item visible.
        /// </summary>
        /// <param name="tag">Tag of the item to check.</param>
        /// <returns>Whether or not is the item visible.</returns>
        public bool IsItemVisible(string tag)
            => ItemFromTag(tag).Visible;

        /// <summary>
        /// Whether or not is a header visible.
        /// </summary>
        /// <param name="groupName">Group name of the header to check.</param>
        /// <returns>Whether or not is the item visible.</returns>
        public bool IsHeaderVisible(string groupName)
            => HeaderFromGroupName(groupName).Visible;

        #region Moving
        /// <summary>
        /// Checks if an item can be moved down.
        /// </summary>
        /// <param name="tag">Item's tag.</param>
        /// <returns>True if the item can be moved down,
        /// false otherwise.</returns>
        public bool CanMoveDown(string tag)
        {
            NavViewItemViewModel item = ItemFromTag(tag);
            int index;

            if (!item.IsFooter)
            {
                index = Items.IndexOf(item);
                if (index + 1 == Items.Count)
                {
                    return false;
                }

                if (Items.ElementAt(index + 1).HeaderGroup != item.HeaderGroup)
                {
                    return false;
                }
            }
            else
            {
                index = FooterItems.IndexOf(item);
                if (index + 1 == FooterItems.Count)
                {
                    return false;
                }

                if (FooterItems.ElementAt(index + 1).HeaderGroup !=
                    item.HeaderGroup)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Moves an item down.
        /// </summary>
        /// <param name="tag">Item's tag.</param>
        public void MoveDown(string tag)
        {
            NavViewItemViewModel item = ItemFromTag(tag);
            int index;

            if (!item.IsFooter)
            {
                index = Items.IndexOf(item);
                Items.Move(index, index + 1);
            }
            else
            {
                index = FooterItems.IndexOf(item);
                FooterItems.Move(index, index + 1);
            }
        }

        /// <summary>
        /// Moves an item to the bottom.
        /// </summary>
        /// <param name="tag">Item's tag.</param>
        public void MoveToBottom(string tag)
        {
            NavViewItemViewModel item = ItemFromTag(tag);
            int index;

            if (!item.IsFooter)
            {
                index = Items.IndexOf(item);
                while (index < Items.Count &&
                    !Items.ElementAt(index + 1).Tag.Equals("Header"))
                {
                    Items.Move(index, index + 1);
                    index++;
                }
            }
            else
            {
                index = FooterItems.IndexOf(item);
                while (index < FooterItems.Count &&
                    !FooterItems.ElementAt(index + 1).Tag.Equals("Header"))
                {
                    FooterItems.Move(index, index + 1);
                    index++;
                }
            }
        }

        /// <summary>
        /// Checks if an item can be moved up.
        /// </summary>
        /// <param name="tag">Item's tag.</param>
        /// <returns>True if the item can be moved up,
        /// false otherwise.</returns>
        public bool CanMoveUp(string tag)
        {
            NavViewItemViewModel item = ItemFromTag(tag);
            int index;

            if (!item.IsFooter)
            {
                index = Items.IndexOf(item);
                if (index - 1 == -1)
                {
                    return false;
                }

                if (Items.ElementAt(index - 1).HeaderGroup != item.HeaderGroup ||
                    (Items.ElementAt(index - 1).HeaderGroup == item.HeaderGroup &&
                    Items.ElementAt(index - 1).Tag.Equals("Header")))
                {
                    return false;
                }
            }
            else
            {
                index = FooterItems.IndexOf(item);
                if (index - 1 == -1)
                {
                    return false;
                }

                if (FooterItems.ElementAt(index - 1).HeaderGroup != item.HeaderGroup ||
                    (FooterItems.ElementAt(index - 1).HeaderGroup == item.HeaderGroup &&
                    FooterItems.ElementAt(index - 1).Tag.Equals("Header")))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Moves an item up.
        /// </summary>
        /// <param name="tag">Item's tag.</param>
        public void MoveUp(string tag)
        {
            NavViewItemViewModel item = ItemFromTag(tag);
            int index;

            if (!item.IsFooter)
            {
                index = Items.IndexOf(item);
                Items.Move(index, index - 1);
            }
            else
            {
                index = FooterItems.IndexOf(item);
                FooterItems.Move(index, index - 1);
            }
        }

        /// <summary>
        /// Moves an item to the top.
        /// </summary>
        /// <param name="tag">Item's tag.</param>
        public void MoveToTop(string tag)
        {
            NavViewItemViewModel item = ItemFromTag(tag);
            int index;

            if (item.HeaderGroup == "General")
            {
                index = Items.IndexOf(item);
                Items.Move(index, 0);
            }
            else
            {
                NavViewItemViewModel header = HeaderFromGroupName(item.HeaderGroup);
                if (!item.IsFooter)
                {
                    index = Items.IndexOf(item);
                    Items.Move(index, Items.IndexOf(header) + 1);
                }
                else
                {
                    index = FooterItems.IndexOf(item);
                    FooterItems.Move(index, FooterItems.IndexOf(header) + 1);
                }
            }
        }
        #endregion

        #region Finding items
        /// <summary>
        /// Gets an item based on its tag.
        /// </summary>
        /// <param name="tag">The item's tag.</param>
        /// <returns>The item with the specified tag.</returns>
        public NavViewItemViewModel ItemFromTag(string tag)
        {
            NavViewItemViewModel item = Items.FirstOrDefault(i => i.Tag == tag);

            if (item == null)
            {
                item = FooterItems.FirstOrDefault(i => i.Tag == tag);
            }

            if (item == null)
            {
                throw new ArgumentException("Provided tag (" + tag + ") was not found.");
            }

            return item;
        }

        /// <summary>
        /// Gets a header based on its group name.
        /// </summary>
        /// <param name="group">The header's group name.</param>
        /// <returns>The header with the specified group name.</returns>
        public NavViewItemViewModel HeaderFromGroupName(string group)
        {
            NavViewItemViewModel item = Items.
                FirstOrDefault(i => i.HeaderGroup == group && i.Tag.Equals("Header"));

            if (item == null)
            {
                item = FooterItems.
                    FirstOrDefault(i => i.HeaderGroup == group && i.Tag.Equals("Header"));
            }

            if (item == null)
            {
                throw new ArgumentException("Provided group name (" + group + ") was not found.");
            }

            return item;
        }
        #endregion
    }
}
