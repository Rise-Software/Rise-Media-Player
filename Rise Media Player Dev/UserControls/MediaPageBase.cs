using CommunityToolkit.Mvvm.Input;
using Microsoft.Toolkit.Uwp.UI;
using Rise.App.Helpers;
using Rise.App.ViewModels;
using Rise.App.Views;
using Rise.App.Views.Albums.Properties;
using Rise.Common.Enums;
using Rise.Common.Extensions;
using Rise.Common.Helpers;
using Rise.Common.Interfaces;
using Rise.Data.Json;
using Rise.Data.Sources;
using Rise.Data.ViewModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Rise.App.UserControls
{
    /// <summary>
    /// A base class for pages that present media content.
    /// </summary>
    public partial class MediaPageBase : Page
    {
        private NavViewDataSource NavDataSource => App.NavDataSource;

        /// <summary>
        /// A property that stores the page's selected item.
        /// </summary>
        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register("SelectedItem", typeof(object),
                typeof(MediaPageBase), new PropertyMetadata(null));

        /// <summary>
        /// A helper to save session state during navigation.
        /// </summary>
        public NavigationHelper NavigationHelper { get; private set; }

        /// <summary>
        /// The ViewModel for this page. Contains the collection of media and
        /// commands responsible for sorting and starting playback.
        /// </summary>
        public MediaCollectionViewModel MediaViewModel { get; private set; }

        /// <summary>
        /// A helper to handle adding items to playlists.
        /// </summary>
        public AddToPlaylistHelper PlaylistHelper { get; private set; }

        /// <summary>
        /// Initializes a new instance of this class without initializing
        /// <see cref="MediaViewModel"/>.
        /// </summary>
        public MediaPageBase()
        {
            NavigationHelper = new(this);
            NavigationHelper.SaveState += NavigationHelper_SaveState;
        }

        /// <summary>
        /// Initializes a new instance of this class with the specified
        /// property for sorting and ViewModel data source.
        /// </summary>
        public MediaPageBase(string defaultProperty, IList viewModelSource)
            : this()
        {
            CreateViewModel(defaultProperty, viewModelSource);
        }

        /// <summary>
        /// Initializes a new instance of this class with a data
        /// source for <see cref="PlaylistHelper"/>.
        /// </summary>
        public MediaPageBase(IList<PlaylistViewModel> playlists)
            : this()
        {
            PlaylistHelper = new(playlists);
        }

        /// <summary>
        /// Initializes a new instance of this class with the specified
        /// property for sorting, a ViewModel data source, and a data
        /// source for <see cref="PlaylistHelper"/>.
        /// </summary>
        public MediaPageBase(string defaultProperty, IList viewModelSource, IList<PlaylistViewModel> playlists)
            : this(defaultProperty, viewModelSource)
        {
            PlaylistHelper = new(playlists);
        }

        /// <summary>
        /// Initializes <see cref="MediaViewModel"/> with the specified
        /// property for sorting and data source.
        /// </summary>
        public void CreateViewModel(string defaultProperty, IList dataSource)
        {
            MediaViewModel ??= new(defaultProperty, dataSource,
                App.MViewModel.Songs, App.MPViewModel);
        }
    }

    // Editing
    public partial class MediaPageBase
    {
        [RelayCommand]
        private Task EditItemAsync(object parameter)
        {
            if (parameter is SongViewModel song)
                return EditSongAsync(song);
            else if (parameter is AlbumViewModel album)
                return EditAlbumAsync(album);
            else if (parameter is PlaylistViewModel playlist)
                return EditPlaylistAsync(playlist);
            else
                throw new NotImplementedException("No other item type is supported at the moment.");
        }

        /// <summary>
        /// Opens the properties page for the provided song.
        /// </summary>
        public async Task EditSongAsync(SongViewModel song)
        {
            try
            {
                StorageFile file = await StorageFile.GetFileFromPathAsync(song.Location);
                if (file != null)
                {
                    SongPropertiesViewModel props = new(song, file.DateCreated)
                    {
                        FileProps = await file.GetBasicPropertiesAsync()
                    };

                    _ = await typeof(SongPropertiesPage).
                        PlaceInApplicationViewAsync(props, 380, 550, true);
                }
            }
            catch
            {

            }
        }

        /// <summary>
        /// Opens the properties page for the provided album.
        /// </summary>
        public async Task EditAlbumAsync(AlbumViewModel album)
        {
            _ = await typeof(AlbumPropertiesPage).
                ShowInApplicationViewAsync(album, 380, 550, true);
        }

        /// <summary>
        /// Opens the properties page for the provided playlist.
        /// </summary>
        public async Task EditPlaylistAsync(PlaylistViewModel playlist)
        {
            _ = await typeof(PlaylistPropertiesPage).
                ShowInApplicationViewAsync(playlist, 380, 550, true);
        }
    }

    // Navigation
    public partial class MediaPageBase
    {
        /// <summary>
        /// Navigates to the album with the specified name.
        /// </summary>
        [RelayCommand]
        protected void GoToAlbum(string name)
            => _ = Frame.Navigate(typeof(AlbumSongsPage), name);

        /// <summary>
        /// Navigates to the artist with the specified name.
        /// </summary>
        [RelayCommand]
        protected void GoToArtist(string name)
            => _ = Frame.Navigate(typeof(ArtistSongsPage), name);

        [RelayCommand]
        private Task OpenInExplorerAsync(object parameter)
        {
            if (parameter is IMediaItem item)
                return OpenItemInExplorerAsync(item);
            else
                throw new NotImplementedException("No other item type is supported at the moment.");
        }

        /// <summary>
        /// Opens the explorer to the item's location and highlights
        /// the file.
        /// </summary>
        public async Task OpenItemInExplorerAsync(IMediaItem item)
        {
            string location = item.Location;
            string path = Path.GetDirectoryName(location);
            string filename = Path.GetFileName(location);

            try
            {
                var folder = await StorageFolder.GetFolderFromPathAsync(path);
                var options = new FolderLauncherOptions();

                var stItem = await folder.TryGetItemAsync(filename);
                if (stItem != null && stItem is StorageFile file)
                    options.ItemsToSelect.Add(file);

                _ = await Launcher.LaunchFolderAsync(folder, options);
            }
            catch { }
        }
    }

    // Playlists
    public partial class MediaPageBase
    {
        private JsonBackendController<PlaylistViewModel> PBackend
            => App.MViewModel.PBackend;

        [RelayCommand]
        private Task AddSelectedItemToPlaylistAsync(PlaylistViewModel playlist)
        {
            var itm = GetValue(SelectedItemProperty);
            if (itm is IMediaItem media)
            {
                if (playlist != null)
                {
                    playlist.AddItem(itm as IMediaItem);
                    return PBackend.SaveAsync();
                }

                return PlaylistHelper.CreateNewPlaylistAsync(media);
            }

            return Task.CompletedTask;
        }

        [RelayCommand]
        private Task AddMediaItemsToPlaylistAsync(PlaylistViewModel playlist)
        {
            var first = MediaViewModel.Items.FirstOrDefault();
            if (playlist != null)
            {
                playlist.AddItems(MediaViewModel.Items.Cast<IMediaItem>());
                return PBackend.SaveAsync();
            }
            else if (first is IMediaItem)
            {
                var items = MediaViewModel.Items.Cast<IMediaItem>();
                return PlaylistHelper.CreateNewPlaylistAsync(items);
            }

            return Task.CompletedTask;
        }

        [RelayCommand]
        private Task SwitchPlaylistPinningState(PlaylistViewModel playlist)
        {
            bool hasItem = NavDataSource.TryGetItem("PlaylistsPage", out var item);
            if (hasItem)
            {
                if (playlist.IsPinned)
                {
                    var itm = item.SubItems.FirstOrDefault(i => i.Id == playlist.Id.ToString());
                    if (itm != null)
                    {
                        item.SubItems.Remove(itm);
                        playlist.IsPinned = false;
                    }
                }
                else
                {
                    var itm = new NavViewItemViewModel
                    {
                        Id = playlist.Id.ToString(),
                        ItemType = NavViewItemType.SubItem,
                        Icon = playlist.Icon,
                        Label = playlist.Title,
                        ParentId = item.Id,
                        FlyoutId = "RemoveItemFlyout"
                    };

                    item.SubItems.Add(itm);
                    playlist.IsPinned = true;
                }
            }

            return App.MViewModel.PBackend.SaveAsync();
        }
    }

    // Session state
    public partial class MediaPageBase
    {
        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
            MediaViewModel?.Dispose();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
            => NavigationHelper.OnNavigatedTo(e);

        protected override void OnNavigatedFrom(NavigationEventArgs e)
            => NavigationHelper.OnNavigatedFrom(e);
    }
}
