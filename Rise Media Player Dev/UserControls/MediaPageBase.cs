﻿using CommunityToolkit.Mvvm.Input;
using Microsoft.Toolkit.Uwp.UI;
using Rise.App.ViewModels;
using Rise.App.Views;
using Rise.App.Views.Albums.Properties;
using Rise.Common.Extensions;
using Rise.Common.Helpers;
using Rise.Common.Interfaces;
using System;
using System.Collections;
using System.IO;
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
        /// Initializes a new instance of this class without initializing
        /// <see cref="MediaViewModel"/>.
        /// </summary>
        public MediaPageBase()
        {
            NavigationHelper = new(this);
            NavigationHelper.LoadState += NavigationHelper_LoadState;
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

    // Session state
    public partial class MediaPageBase
    {
        private void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            if (e.PageState != null && MediaViewModel != null)
            {
                bool result = e.PageState.TryGetValue("Ascending", out var asc);
                if (result)
                    MediaViewModel.UpdateSortDirection((bool)asc ?
                        SortDirection.Ascending : SortDirection.Descending);

                result = e.PageState.TryGetValue("Property", out var prop);
                if (result)
                    MediaViewModel.SortBy(prop.ToString());
            }
        }

        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
            if (MediaViewModel != null)
            {
                e.PageState["Ascending"] = MediaViewModel.
                    CurrentSortDirection == SortDirection.Ascending;

                e.PageState["Property"] = MediaViewModel.CurrentSortProperty;

                MediaViewModel.Items.Filter = null;
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
            => NavigationHelper.OnNavigatedTo(e);

        protected override void OnNavigatedFrom(NavigationEventArgs e)
            => NavigationHelper.OnNavigatedFrom(e);
    }
}
