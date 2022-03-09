using Microsoft.Toolkit.Uwp.UI;
using Rise.App.Common;
using Rise.App.Dialogs;
using Rise.App.Helpers;
using Rise.App.ViewModels;
using System;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace Rise.App.Views
{
    public sealed partial class PlaylistsPage : Page
    {
        private AdvancedCollectionView Playlists => App.MViewModel.FilteredPlaylists;

        private static readonly DependencyProperty SelectedPlaylistProperty =
                DependencyProperty.Register("SelectedPlaylist", typeof(PlaylistViewModel), typeof(AlbumsPage), null);

        private PlaylistViewModel SelectedPlaylist
        {
            get => (PlaylistViewModel)GetValue(SelectedPlaylistProperty);
            set => SetValue(SelectedPlaylistProperty, value);
        }

        public PlaylistsPage()
        {
            InitializeComponent();
        }

        private async void CreatePlaylistButton_Click(object sender, RoutedEventArgs e)
        {
            await new CreatePlaylistDialog().ShowAsync();
        }

        private async void MenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            await SelectedPlaylist.DeleteAsync();
        }

        private void AskDiscy_Click(object sender, RoutedEventArgs e)
        {
            DiscyOnPlaylist.IsOpen = true;
        }

        private void MainGrid_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            if ((e.OriginalSource as FrameworkElement).DataContext is PlaylistViewModel playlist)
            {
                SelectedPlaylist = playlist;
                PlaylistFlyout.ShowAt(MainGrid, e.GetPosition(MainGrid));
            }
        }

        private void GridView_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (!KeyboardHelpers.IsCtrlPressed())
            {
                if ((e.OriginalSource as FrameworkElement).DataContext is PlaylistViewModel playlist)
                {
                    _ = Frame.Navigate(typeof(PlaylistDetailsPage), playlist);
                }

                SelectedPlaylist = null;
            }
            else
            {
                if ((e.OriginalSource as FrameworkElement).DataContext is PlaylistViewModel playlist)
                {
                    SelectedPlaylist = playlist;
                }
            }
        }

        private void MenuFlyoutItem_Click_1(object sender, RoutedEventArgs e)
        {
            PlaylistViewModel model = (sender as MenuFlyoutItem).Tag as PlaylistViewModel;
            System.Diagnostics.Debug.WriteLine($"Playlist info:\n   Title: {model.Title}\n   Description: {model.Description}");
        }

        private async void PlaylistProperties_Click(object sender, RoutedEventArgs e)
        {
            await SelectedPlaylist.StartEditAsync();
        }

        private async void ImportPlaylist_Click(object sender, RoutedEventArgs e)
        {
            FileOpenPicker picker = new();
            picker.FileTypeFilter.Add(".m3u");

            Windows.Storage.StorageFile file = await picker.PickSingleFileAsync();

            if (file != null)
            {
                var playlist = await PlaylistViewModel.GetFromFileAsync(file);
                await playlist.SaveAsync();
            }
        }
    }
}
