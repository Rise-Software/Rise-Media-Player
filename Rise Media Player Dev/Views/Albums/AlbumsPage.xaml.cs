using CommunityToolkit.Mvvm.Input;
using Rise.App.UserControls;
using Rise.App.ViewModels;
using Rise.Common.Enums;
using Rise.Common.Extensions.Markup;
using Rise.Common.Helpers;
using Rise.Data.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Rise.App.Views
{
    public sealed partial class AlbumsPage : MediaPageBase
    {
        private JsonBackendController<PlaylistViewModel> PBackend
            => App.MViewModel.PBackend;
        private MainViewModel MViewModel => App.MViewModel;
        private SettingsViewModel SViewModel => App.SViewModel;

        private AlbumViewModel SelectedItem
        {
            get => (AlbumViewModel)GetValue(SelectedItemProperty);
            set => SetValue(SelectedItemProperty, value);
        }

        public AlbumsPage()
            : base("AlbumTitle", App.MViewModel.Albums, App.MViewModel.Playlists)
        {
            InitializeComponent();

            UpdateViewWithViewMode();

            PlaylistHelper.AddPlaylistsToSubItem(AddTo, AddToPlaylistCommand);
            PlaylistHelper.AddPlaylistsToFlyout(AddToBar, AddToPlaylistCommand);
        }
    }

    // Playlists
    public sealed partial class AlbumsPage
    {
        [RelayCommand]
        private Task AddToPlaylistAsync(PlaylistViewModel playlist)
        {
            var name = SelectedItem.Title;
            var items = new List<SongViewModel>();

            foreach (var itm in MViewModel.Songs)
                if (itm.Album == name)
                    items.Add(itm);

            if (playlist == null)
            {
                return PlaylistHelper.CreateNewPlaylistAsync(items);
            }
            else
            {
                playlist.AddItems(items);
                return PBackend.SaveAsync();
            }
        }
    }

    // Event handlers
    public sealed partial class AlbumsPage
    {
        [RelayCommand]
        private void UpdateViewMode(AlbumViewMode viewMode)
        {
            SViewModel.AlbumViewMode = viewMode;
            UpdateViewWithViewMode();
        }

        private void UpdateViewWithViewMode()
        {
            if (App.SViewModel.AlbumViewMode == AlbumViewMode.HorizontalTile)
                MainGrid.DesiredWidth = 256;
            else
                MainGrid.DesiredWidth = 158;
        }

        private void MainGrid_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem is AlbumViewModel album && !KeyboardHelpers.IsCtrlPressed())
            {
                _ = Frame.Navigate(typeof(AlbumSongsPage), album.Model.Id);
            }
        }

        private void MenuFlyout_Opening(object sender, object e)
        {
            var fl = sender as MenuFlyout;
            var cont = MainGrid.ItemFromContainer(fl.Target);

            if (cont == null)
                fl.Hide();
            else
                SelectedItem = (AlbumViewModel)cont;
        }

        private void AskDiscy_Click(object sender, RoutedEventArgs e)
        {
            DiscyOnAlbum.IsOpen = true;
        }

        private async void AddFolders_Click(object sender, RoutedEventArgs e)
        {
            ContentDialog dialog = new()
            {
                Title = ResourceHelper.GetString("/Settings/MediaLibraryManageFoldersTitle"),
                CloseButtonText = ResourceHelper.GetString("Close"),
                Content = new Settings.MediaSourcesPage()
            };
            _ = await dialog.ShowAsync();
        }
    }
}
