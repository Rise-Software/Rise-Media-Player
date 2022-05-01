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
            Loaded += LocalVideosPage_Loaded;
        }

        private void LocalVideosPage_Loaded(object sender, RoutedEventArgs e)
        {
            AddTo.Items.Clear();

            MenuFlyoutItem newPlaylistItem = new()
            {
                Text = "New playlist",
                Icon = new FontIcon
                {
                    Glyph = "\uE93F",
                    FontFamily = new Windows.UI.Xaml.Media.FontFamily("ms-appx:///Assets/MediaPlayerIcons.ttf#Media Player Fluent Icons")
                }
            };

            newPlaylistItem.Click += NewPlaylistItem_Click;

            AddTo.Items.Add(newPlaylistItem);

            if (App.MViewModel.Playlists.Count > 0)
            {
                AddTo.Items.Add(new MenuFlyoutSeparator());
            }

            foreach (PlaylistViewModel playlist in App.MViewModel.Playlists)
            {
                MenuFlyoutItem item = new()
                {
                    Text = playlist.Title,
                    Icon = new FontIcon
                    {
                        Glyph = "\uE93F",
                        FontFamily = new Windows.UI.Xaml.Media.FontFamily("ms-appx:///Assets/MediaPlayerIcons.ttf#Media Player Fluent Icons")
                    },
                    Tag = playlist
                };

                item.Click += Item_Click;

                AddTo.Items.Add(item);
            }
        }

        private async void NewPlaylistItem_Click(object sender, RoutedEventArgs e)
        {
            PlaylistViewModel playlist = new()
            {
                Title = $"Untitled Playlist #{App.MViewModel.Playlists.Count + 1}",
                Description = "",
                Icon = "ms-appx:///Assets/NavigationView/PlaylistsPage/blankplaylist.png",
                Duration = "0"
            };

            await playlist.AddVideoAsync(SelectedVideo, true);
        }

        private async void Item_Click(object sender, RoutedEventArgs e)
        {
            PlaylistViewModel playlist = (sender as MenuFlyoutItem).Tag as PlaylistViewModel;
            await playlist.AddVideoAsync(SelectedVideo);
        }

        private async void GridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (!KeyboardHelpers.IsCtrlPressed())
            {
                if (e.ClickedItem is VideoViewModel video)
                {
                    await PViewModel.PlayVideoAsync(video);
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
            ContentDialog dialog = new()
            {
                Title = "Manage local media folders",
                CloseButtonText = "Close",
                Content = new Settings.MediaSourcesPage()
            };

            _ = await dialog.ShowAsync();
        }

        private async void NewPlaylistMenu_Click(object sender, RoutedEventArgs e)
        {
            PlaylistViewModel playlist = new()
            {
                Title = $"Untitled Playlist #{App.MViewModel.Playlists.Count + 1}",
                Description = "",
                Icon = "ms-appx:///Assets/NavigationView/PlaylistsPage/blankplaylist.png",
                Duration = "0"
            };

            await playlist.AddVideoAsync(SelectedVideo, true);
        }
    }
}
