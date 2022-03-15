using Microsoft.Toolkit.Uwp.UI;
using Rise.App.Dialogs;
using Rise.App.ViewModels;
using Rise.Common.Helpers;
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

        private static readonly DependencyProperty SelectedVideoProperty =
            DependencyProperty.Register("SelectedVideo", typeof(VideoViewModel), typeof(LocalVideosPage), null);

        private VideoViewModel SelectedVideo
        {
            get => (VideoViewModel)GetValue(SelectedVideoProperty);
            set => SetValue(SelectedVideoProperty, value);
        }

        /// <summary>
        /// Gets the <see cref="NavigationHelper"/> associated with this <see cref="Page"/>.
        /// </summary>
        private readonly NavigationHelper _navigationHelper;

        private readonly string Label = "LocalVideos";

        private SortDirection CurrentSort = SortDirection.Ascending;
        private string CurrentSortProperty = "Title";

        private bool IsCtrlPressed;

        public LocalVideosPage()
        {
            InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;

            _navigationHelper = new NavigationHelper(this);
        }

        private async void GridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (!KeyboardHelpers.IsCtrlPressed())
            {
                if (e.ClickedItem is VideoViewModel video)
                {
                    await PViewModel.PlayVideoAsync(e.ClickedItem as VideoViewModel);
                    if (Window.Current.Content is Frame rootFrame)
                    {
                        rootFrame.Navigate(typeof(VideoPlaybackPage));
                    }
                    SelectedVideo = null;
                }
            }
            else
            {
                if (e.ClickedItem is VideoViewModel video)
                {
                    SelectedVideo = video;
                }
            }
        }

        private async void Play_Click(object sender, RoutedEventArgs e)
        {
            if (MainGrid.Items.Count > 0)
            {
                if (SelectedVideo != null)
                {
                    await EventsLogic.StartVideoPlaybackAsync(MainGrid.Items.IndexOf(SelectedVideo));
                }
                else
                {
                    await EventsLogic.StartVideoPlaybackAsync(0);
                }
                if (Window.Current.Content is Frame rootFrame)
                {
                    _ = rootFrame.Navigate(typeof(VideoPlaybackPage));
                }
            }
        }

        private void AskDiscy_Click(object sender, RoutedEventArgs e)
        {
            DiscyOnVideo.IsOpen = true;
        }

        private void MainGrid_RightTapped_1(object sender, Windows.UI.Xaml.Input.RightTappedRoutedEventArgs e)
        {
            if ((e.OriginalSource as FrameworkElement).DataContext is VideoViewModel video)
            {
                SelectedVideo = video;
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
                default:
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

        private void SortByYear_Click(object sender, RoutedEventArgs e)
        {
            Videos.SortDescriptions.Clear();
            Videos.SortDescriptions.Add(new SortDescription("Year", CurrentSort));
            CurrentSortProperty = "Year";
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

        private void Page_KeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            IsCtrlPressed = e.Key == Windows.System.VirtualKey.Control;
        }

        private async void PlayFromUrl_Click(object sender, RoutedEventArgs e)
        {
            _ = await new VideoStreamingDialog().ShowAsync();
        }

        private async void AddFolders_Click(object sender, RoutedEventArgs e)
        {
            ContentDialog dialog = new ContentDialog();
            dialog.Title = "Manage local media folders";
            dialog.CloseButtonText = "Close";
            dialog.Content = new Settings.MediaSourcesPage();
            var result = await dialog.ShowAsync();
        }
    }
}
