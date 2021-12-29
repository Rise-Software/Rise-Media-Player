using Microsoft.Toolkit.Uwp.UI;
using Rise.App.Common;
using Rise.App.Dialogs;
using Rise.App.ViewModels;
using System;
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
            if ((e.OriginalSource as FrameworkElement).DataContext is PlaylistViewModel playlist)
            {
                _ = Frame.Navigate(typeof(PlaylistDetailsPage), playlist);
            }

            SelectedPlaylist = null;
        }
    }
}
