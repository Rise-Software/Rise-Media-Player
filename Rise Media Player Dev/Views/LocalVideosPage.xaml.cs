using Microsoft.Toolkit.Uwp.UI;
using Rise.App.Common;
using Rise.App.ViewModels;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Rise.App.Views
{
    public sealed partial class LocalVideosPage : Page
    {
        /// <summary>
        /// Gets the app-wide MViewModel instance.
        /// </summary>
        private MainViewModel MViewModel => App.MViewModel;

        private AdvancedCollectionView Videos => App.MViewModel.FilteredVideos;

        /// <summary>
        /// Gets the app-wide PViewModel instance.
        /// </summary>
        private PlaybackViewModel PViewModel => App.PViewModel;

        /// <summary>
        /// Gets the <see cref="NavigationHelper"/> associated with this <see cref="Page"/>.
        /// </summary>
        private readonly NavigationHelper _navigationHelper;

        private SortDirection CurrentSort = SortDirection.Ascending;
        private string CurrentSortProperty = "Title";

        public LocalVideosPage()
        {
            InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;

            _navigationHelper = new NavigationHelper(this);
        }

        private async void GridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            await PViewModel.PlayVideoAsync(e.ClickedItem as VideoViewModel);
            if (Window.Current.Content is Frame rootFrame)
            {
                rootFrame.Navigate(typeof(VideoPlaybackPage));
            }
        }

        private async void Play_Click(object sender, RoutedEventArgs e)
        {
            if ((e.OriginalSource as FrameworkElement).DataContext is VideoViewModel video)
            {
                int index = MainGrid.Items.IndexOf(video);
                await EventsLogic.StartVideoPlaybackAsync(index);

                if (Window.Current.Content is Frame rootFrame)
                {
                    rootFrame.Navigate(typeof(VideoPlaybackPage));
                }
            }
        }

        private void GridView_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {

        }

        private void AskDiscy_Click(object sender, RoutedEventArgs e)
        {
            DiscyOnVideo.IsOpen = true;
        }

        private void MainGrid_RightTapped_1(object sender, Windows.UI.Xaml.Input.RightTappedRoutedEventArgs e)
        {
            if ((e.OriginalSource as FrameworkElement).DataContext is VideoViewModel video)
            {
                VideosFlyout.ShowAt(MainGrid, e.GetPosition(MainGrid));
            }
        }

        private void SortByName_Click(object sender, RoutedEventArgs e)
        {
            Videos.SortDescriptions.Clear();
            Videos.SortDescriptions.Add(new SortDescription("Title", CurrentSort));
            CurrentSortProperty = "Title";
            Videos.Refresh();
        }

        private void AscendingOrDescending_Click(object sender, RoutedEventArgs e)
        {
            MenuFlyoutItem item = sender as MenuFlyoutItem;
            Videos.SortDescriptions.Clear();

            string tag = item.Tag.ToString();
            switch (tag)
            {
                case "Ascending":
                    CurrentSort = SortDirection.Ascending;
                    break;
                case "Descending":
                    CurrentSort = SortDirection.Descending;
                    break;
            }
            Videos.SortDescriptions.Add(new SortDescription(CurrentSortProperty, CurrentSort));
            Videos.Refresh();
        }

        private void SortByLength_Click(object sender, RoutedEventArgs e)
        {
            Videos.SortDescriptions.Clear();
            Videos.SortDescriptions.Add(new SortDescription("Length", CurrentSort));
            CurrentSortProperty = "Length";
            Videos.Refresh();
        }

        private async void ShuffleItem_Click(object sender, RoutedEventArgs e)
        {
            await EventsLogic.StartVideoPlaybackAsync(new Random().Next(0, Videos.Count), true);
            if (Window.Current.Content is Frame rootFrame)
            {
                _ = rootFrame.Navigate(typeof(VideoPlaybackPage));
            }
        }
    }
}
