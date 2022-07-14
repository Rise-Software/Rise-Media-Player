using Microsoft.Toolkit.Uwp.UI.Animations;
using Rise.App.Dialogs;
using Rise.App.UserControls;
using Rise.App.ViewModels;
using Rise.Common.Enums;
using Rise.Common.Extensions;
using Rise.Common.Helpers;
using System;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace Rise.App.Views
{
    public sealed partial class PlaylistsPage : MediaPageBase
    {
        private PlaylistViewModel SelectedItem
        {
            get => (PlaylistViewModel)GetValue(SelectedItemProperty);
            set => SetValue(SelectedItemProperty, value);
        }

        private readonly string Label = "Playlists";
        private double? _offset = null;

        public PlaylistsPage()
            : base(MediaItemType.Playlist, App.MViewModel.Playlists)
        {
            InitializeComponent();

            NavigationHelper.LoadState += NavigationHelper_LoadState;
            NavigationHelper.SaveState += NavigationHelper_SaveState;
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

    // Event handlers
    public sealed partial class PlaylistsPage
    {
        private async void CreatePlaylistButton_Click(object sender, RoutedEventArgs e)
        {
            await new CreatePlaylistDialog().ShowAsync();
        }

        private async void MenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            await SelectedItem.DeleteAsync();
        }

        private void AskDiscy_Click(object sender, RoutedEventArgs e)
        {
            DiscyOnPlaylist.IsOpen = true;
        }

        private void MainGrid_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            if ((e.OriginalSource as FrameworkElement).DataContext is PlaylistViewModel playlist)
            {
                SelectedItem = playlist;
                PlaylistFlyout.ShowAt(MainGrid, e.GetPosition(MainGrid));
            }
        }

        private void GridView_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if ((e.OriginalSource as FrameworkElement).DataContext is PlaylistViewModel playlist)
            {
                if (!KeyboardHelpers.IsCtrlPressed())
                {
                    Frame.SetListDataItemForNextConnectedAnimation(playlist);
                    _ = Frame.Navigate(typeof(PlaylistDetailsPage), playlist.Model.Id);
                }
                else
                {
                    SelectedItem = playlist;
                }
            }
        }

        private async void ImportPlaylist_Click(object sender, RoutedEventArgs e)
        {
            FileOpenPicker picker = new();
            picker.FileTypeFilter.Add(".m3u");

            StorageFile file = await picker.PickSingleFileAsync();

            if (file != null)
            {
                var playlist = await PlaylistViewModel.GetFromFileAsync(file);
                await playlist.SaveAsync();
            }
        }
    }
}
