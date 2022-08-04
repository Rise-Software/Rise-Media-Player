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
        private bool _addSeparator = false;
        private readonly IList<PlaylistViewModel> _items;

        /// <summary>
        /// Initializes the helper with the provided collection.
        /// </summary>
        /// <param name="addToDelegate">A delegate that runs whenever
        /// saving is requested.</param>
        public AddToPlaylistHelper(IList<PlaylistViewModel> playlists,
            Func<PlaylistViewModel, Task> addToDelegate)
        {
            _items = playlists;
            _addSeparator = _items.Count > 0;

            AddToPlaylistCommand = new(addToDelegate);
        }

        /// <summary>
        /// Adds the current playlists to the flyout subitem.
        /// </summary>
        public void AddPlaylistsToSubItem(MenuFlyoutSubItem subItem)
        {
            var itms = subItem.Items;
            if (_addSeparator)
                itms.Add(new MenuFlyoutSeparator());

            foreach (var itm in _items)
                itms.Add(CreatePlaylistFlyoutItem(itm));
        }

        /// <summary>
        /// Adds the current playlists to the flyout.
        /// </summary>
        public void AddPlaylistsToFlyout(MenuFlyout flyout)
        {
            var itms = flyout.Items;
            if (_addSeparator)
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

                _addSeparator = true;
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
