using Microsoft.Toolkit.Uwp.UI;
using Microsoft.Toolkit.Uwp.UI.Animations;
using Rise.App.Dialogs;
using Rise.App.ViewModels;
using Rise.Common.Helpers;
using System;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

namespace Rise.App.Views
{
    // Constructor, Lifecycle management
    public sealed partial class PlaylistsPage : Page
    {
        /// <summary>
        /// Gets the <see cref="NavigationHelper"/> associated with this <see cref="Page"/>.
        /// </summary>
        private readonly NavigationHelper _navigationHelper;

        public PlaylistsPage()
        {
            InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;

            _navigationHelper = new NavigationHelper(this);
        }

        #region NavigationHelper registration
        /// <summary>
        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// Page specific logic should be placed in event handlers for the  
        /// <see cref="NavigationHelper.LoadState"/>
        /// and <see cref="NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method 
        /// in addition to page state preserved during an earlier session.
        /// </summary>
        protected override void OnNavigatedTo(NavigationEventArgs e)
            => _navigationHelper.OnNavigatedTo(e);

        protected override void OnNavigatedFrom(NavigationEventArgs e)
            => _navigationHelper.OnNavigatedFrom(e);
        #endregion
    }

    // Fields, properties
    public sealed partial class PlaylistsPage : Page
    {
        private AdvancedCollectionView Playlists => App.MViewModel.FilteredPlaylists;

        private static readonly DependencyProperty SelectedPlaylistProperty =
                DependencyProperty.Register("SelectedPlaylist", typeof(PlaylistViewModel), typeof(PlaylistsPage), null);

        private PlaylistViewModel SelectedPlaylist
        {
            get => (PlaylistViewModel)GetValue(SelectedPlaylistProperty);
            set => SetValue(SelectedPlaylistProperty, value);
        }
    }

    // Event handlers
    public sealed partial class PlaylistsPage : Page
    {
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
                if (!KeyboardHelpers.IsCtrlPressed())
                {
                    Frame.SetListDataItemForNextConnectedAnimation(playlist);
                    _ = Frame.Navigate(typeof(PlaylistDetailsPage), playlist.Model.Id);
                    SelectedPlaylist = null;
                }
                else
                {
                    SelectedPlaylist = playlist;
                }
            }
        }

        private void MainGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                PlaylistViewModel playlist = MainGrid.Items[MainGrid.SelectedIndex] as PlaylistViewModel;
                SelectedPlaylist = playlist;
                System.Diagnostics.Debug.WriteLine($"Playlist info:\n   Title: {SelectedPlaylist.Title}\n   Description: {SelectedPlaylist.Description}");
            }
            catch
            {
                try
                {
                    SelectedPlaylist = MainGrid.Items[0] as PlaylistViewModel;
                }
                catch
                {

                }

            }
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
