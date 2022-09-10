using CommunityToolkit.Mvvm.Input;
using Microsoft.Toolkit.Uwp.UI.Animations;
using Rise.App.Helpers;
using Rise.App.UserControls;
using Rise.App.ViewModels;
using Rise.Common.Enums;
using Rise.Common.Extensions;
using Rise.Common.Helpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Rise.App.Views
{
    public sealed partial class AlbumsPage : MediaPageBase
    {
        private MainViewModel MViewModel => App.MViewModel;
        private SettingsViewModel SViewModel => App.SViewModel;

        private readonly AddToPlaylistHelper PlaylistHelper;

        private AlbumViewModel SelectedItem
        {
            get => (AlbumViewModel)GetValue(SelectedItemProperty);
            set => SetValue(SelectedItemProperty, value);
        }

        private readonly string Label = "Albums";
        private double? _offset = null;

        public AlbumsPage()
            : base("Title", App.MViewModel.Albums)
        {
            InitializeComponent();

            NavigationHelper.LoadState += NavigationHelper_LoadState;
            NavigationHelper.SaveState += NavigationHelper_SaveState;

            PlaylistHelper = new(App.MViewModel.Playlists, AddToPlaylistAsync);
            PlaylistHelper.AddPlaylistsToSubItem(AddTo);
            PlaylistHelper.AddPlaylistsToFlyout(AddToBar);
        }

        private void OnPageLoaded(object sender, RoutedEventArgs e)
        {
            if (_offset != null)
                MainGrid.FindVisualChild<ScrollViewer>().ChangeView(null, _offset, null);
        }

        private void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            if (e.PageState != null)
            {
                bool result = e.PageState.TryGetValue("Offset", out var offset);
                if (result)
                    _offset = (double)offset;
            }
        }

        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
            var scr = MainGrid.FindVisualChild<ScrollViewer>();
            if (scr != null)
                e.PageState["Offset"] = scr.VerticalOffset;
        }
    }

    // Playlists
    public sealed partial class AlbumsPage
    {
        private Task AddToPlaylistAsync(PlaylistViewModel playlist)
        {
            var name = SelectedItem.Title;
            var items = new List<SongViewModel>();

            foreach (var itm in MViewModel.Songs)
                if (itm.Album == name)
                    items.Add(itm);

            if (playlist == null)
                return PlaylistHelper.CreateNewPlaylistAsync(items);
            else
                return playlist.AddSongsAsync(items);
        }
    }

    // Event handlers
    public sealed partial class AlbumsPage
    {
        [RelayCommand]
        private void UpdateViewMode(AlbumViewMode viewMode)
            => SViewModel.AlbumViewMode = viewMode;

        private void MainGrid_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem is AlbumViewModel album && !KeyboardHelpers.IsCtrlPressed())
            {
                Frame.SetListDataItemForNextConnectedAnimation(album);
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
                Title = "Manage local media folders",
                CloseButtonText = "Close",
                Content = new Settings.MediaSourcesPage()
            };
            _ = await dialog.ShowAsync();
        }
    }
}
