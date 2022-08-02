using Microsoft.Toolkit.Mvvm.Input;
using Rise.App.ViewModels;
using Rise.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace Rise.App.Helpers
{
    /// <summary>
    /// A helper class for the Add to Playlist functionality.
    /// </summary>
    public partial class AddToPlaylistHelper
    {
        private bool _separatorAdded = false;

        private readonly IList<PlaylistViewModel> _items;
        private readonly List<MenuFlyout> _flyouts = new();

        /// <summary>
        /// Initializes the helper with the provided collection.
        /// </summary>
        /// <param name="addToDelegate">A delegate that runs whenever
        /// saving is requested.</param>
        public AddToPlaylistHelper(IList<PlaylistViewModel> playlists,
            Func<PlaylistViewModel, Task> addToDelegate)
        {
            _items = playlists;
            _separatorAdded = _items.Count > 0;

            AddToPlaylistCommand = new(addToDelegate);
        }

        /// <summary>
        /// Adds the current playlists to the flyout subitem.
        /// </summary>
        public void AddPlaylistsToSubItem(MenuFlyoutSubItem subItem)
        {
            var itms = subItem.Items;
            if (_separatorAdded)
                itms.Add(new MenuFlyoutSeparator());

            foreach (var itm in _items)
                itms.Add(CreatePlaylistFlyoutItem(itm));
        }

        /// <summary>
        /// Adds the current playlists to the flyout, and allows this
        /// class to add new playlist items to it.
        /// </summary>
        public void WatchFlyout(MenuFlyout flyout)
        {
            _flyouts.Add(flyout);
            var itms = flyout.Items;

            if (_separatorAdded)
                itms.Add(new MenuFlyoutSeparator());

            foreach (var itm in _items)
                itms.Add(CreatePlaylistFlyoutItem(itm));
        }

        private MenuFlyoutItem CreatePlaylistFlyoutItem(PlaylistViewModel playlist)
        {
            return new MenuFlyoutItem()
            {
                Text = playlist.Title,
                Icon = new FontIcon
                {
                    Glyph = "\uE93F",
                    FontFamily = new("ms-appx:///Assets/MediaPlayerIcons.ttf#Media Player Fluent Icons")
                },
                Command = AddToPlaylistCommand,
                CommandParameter = playlist
            };
        }
    }

    // Commands
    public partial class AddToPlaylistHelper
    {
        /// <summary>
        /// A command to add an item to a playlist.
        /// </summary>
        public AsyncRelayCommand<PlaylistViewModel> AddToPlaylistCommand { get; private set; }

        /// <summary>
        /// Creates a new playlist, adds the specified item to it,
        /// saves the playlist, and adds the flyout items to the
        /// observed objects.
        /// </summary>
        /// <returns>The new playlist.</returns>
        public Task<PlaylistViewModel> CreateNewPlaylistAsync(IMediaItem item)
        {
            return CreateNewPlaylistAsync(new IMediaItem[1] { item });
        }

        /// <summary>
        /// Creates a new playlist, adds the specified items to it,
        /// saves the playlist, and adds the flyout items to the
        /// observed objects.
        /// </summary>
        /// <returns>The new playlist.</returns>
        public async Task<PlaylistViewModel> CreateNewPlaylistAsync(IEnumerable<IMediaItem> items)
        {
            PlaylistViewModel playlist;
            lock (_items)
            {
                playlist = new()
                {
                    Title = $"Untitled Playlist #{_items.Count + 1}",
                    Description = "",
                    Icon = "ms-appx:///Assets/NavigationView/PlaylistsPage/blankplaylist.png",
                    Duration = "0"
                };

                foreach (var flyout in _flyouts)
                {
                    if (!_separatorAdded)
                        flyout.Items.Add(new MenuFlyoutSeparator());
                    flyout.Items.Add(CreatePlaylistFlyoutItem(playlist));
                }

                _separatorAdded = true;
            }

            foreach (var item in items)
            {
                if (item is SongViewModel svm)
                    playlist.Songs.Add(svm);
                else
                    playlist.Videos.Add(item as VideoViewModel);
            }

            await playlist.SaveAsync();
            return playlist;
        }
    }
}
