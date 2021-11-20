using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
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

        private readonly string[] _defaultIcons = new string[10]
        {
            "\uECA5", "\uEA37", "\uE8FD", "\uEC4F", "\uE125",
            "\uE93C", "\uE138", "\uE8B2", "\uE9CE", "\uE115"
        };

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

            object jsonText;
            using (Stream stream = await file.OpenStreamForWriteAsync())
            {
                jsonText = await JsonSerializer.DeserializeAsync<object>(stream);
            }

            _ = JsonArray.
                TryParse(jsonText.ToString(), out JsonArray jsonArray);

            foreach (JsonValue groupValue in jsonArray)
            {
                JsonObject groupObject = groupValue.GetObject();

                string label = groupObject["LabelResource"].GetString();
                string tag = groupObject["Tag"].GetString();
                string iconStr = groupObject["Icon"].GetString();
                string group = groupObject["HeaderGroup"].GetString();

                bool visible = groupObject["Visible"].GetBoolean();
                bool footer = groupObject["IsFooter"].GetBoolean();

                NavViewItemViewModel item = new NavViewItemViewModel
                {
                    LabelResource = label,
                    Tag = tag,
                    Icon = iconStr,
                    HeaderGroup = group,
                    Visible = visible,
                    IsFooter = footer
                };

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

            using (Stream stream = await file.OpenStreamForWriteAsync())
            {
                JsonSerializerOptions options = new JsonSerializerOptions
                {
                    WriteIndented = true
                };

                await JsonSerializer.SerializeAsync(stream, items, options);
            }
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
                        if (!item.Tag.Equals("Header"))
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
                        if (!item.Tag.Equals("Header"))
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
                    if (!item.Tag.Equals("Header"))
                    {
                        string newUri = startingUri + item.Tag + "/" + newName + ".png";
                        item.Icon = newUri;
                    }
                }

                foreach (NavViewItemViewModel item in FooterItems)
                {
                    if (!item.Tag.Equals("Header"))
                    {
                        string newUri = startingUri + item.Tag + "/" + newName + ".png";
                        item.Icon = newUri;
                    }
                }
            }
            else
            {
                int counter = 0;
                foreach (NavViewItemViewModel item in Items)
                {
                    if (!item.Tag.Equals("Header"))
                    {
                        item.Icon = _defaultIcons[counter];
                        counter++;
                    }
                }

                foreach (NavViewItemViewModel item in FooterItems)
                {
                    if (!item.Tag.Equals("Header"))
                    {
                        item.Icon = _defaultIcons[counter];
                        counter++;
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
                Items.Move(index, Items.Count - 1);
            }
            else
            {
                index = FooterItems.IndexOf(item);
                FooterItems.Move(index, FooterItems.Count - 1);
            }
        }

        /// <summary>
        /// Moves an item up.
        /// </summary>
        /// <param name="tag">Item's tag.</param>
        /// <param name="inFooter">Whether or not the item is
        /// in the footer.</param>
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
        /// <param name="inFooter">Whether or not the item is
        /// in the footer.</param>
        public void MoveToTop(string tag)
        {
            NavViewItemViewModel item = ItemFromTag(tag);
            int index;

            if (!item.IsFooter)
            {
                index = Items.IndexOf(item);
                Items.Move(index, 0);
            }
            else
            {
                index = FooterItems.IndexOf(item);
                FooterItems.Move(index, 0);
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
