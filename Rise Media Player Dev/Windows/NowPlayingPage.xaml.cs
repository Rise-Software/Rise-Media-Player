using Rise.App.ViewModels;
using Rise.Common.Extensions;
using Rise.Common.Helpers;
using Rise.Common.Interfaces;
using System;
using Windows.Storage;
using Windows.System;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

namespace Rise.App.Views
{
    // Constructor and variables
    public sealed partial class NowPlayingPage : Page
    {
        // This page might be either in the main
        // window compact overlay mode or in a
        // separate window (user can choose in settings).
        private bool _isInMainOverlay;

        private SafeObservableCollection<SongViewModel> _queuedSongs = (SafeObservableCollection<SongViewModel>)App.MPViewModel.QueuedItems.CloneList<IMediaItem, SongViewModel>();

        private SongViewModel _selectedItem;

        public NowPlayingPage()
        {
            InitializeComponent();

            TitleBar.SetupTitleBar();
            MainPlayer.SetMediaPlayer(App.MPViewModel.Player);

            Loaded += (s, e) =>
            {
                if (!_isInMainOverlay)
                {
                    OverlayButton.Visibility = Visibility.Collapsed;
                    QueueButton.Margin = new(8, 0, 0, 0);
                }

                QueueButton.Checked += QueueButtonCheckStateChanged;
                QueueButton.Unchecked += QueueButtonCheckStateChanged;

                ApplyPlaylistItems(AddTo);
            };
        }

    }

    // Events and functions
    public partial class NowPlayingPage
    {
        private void ApplyPlaylistItems(MenuFlyoutSubItem addTo)
        {
            addTo.Items.Clear();

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

            addTo.Items.Add(newPlaylistItem);

            if (App.MViewModel.Playlists.Count > 0)
            {
                addTo.Items.Add(new MenuFlyoutSeparator());
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

                addTo.Items.Add(item);
            }
        }

        private void QueueButtonCheckStateChanged(object sender, RoutedEventArgs e)
        {
            if (QueueButton.IsChecked ?? false)
            {
                QueueList.Visibility = Visibility.Visible;
                InfoText.Visibility = Visibility.Collapsed;
            }
            else
            {
                QueueList.Visibility = Visibility.Collapsed;
                InfoText.Visibility = Visibility.Visible;
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            _isInMainOverlay = (bool)e.Parameter;
        }

        private async void OverlayButton_Click(object sender, RoutedEventArgs e)
        {
            if (ApplicationView.GetForCurrentView().ViewMode == ApplicationViewMode.CompactOverlay)
            {
                var preferences = ViewModePreferences.CreateDefault(ApplicationViewMode.Default);
                _ = await ApplicationView.GetForCurrentView().TryEnterViewModeAsync(ApplicationViewMode.Default, preferences);
            }
            else
            {
                var preferences = ViewModePreferences.CreateDefault(ApplicationViewMode.CompactOverlay);
                preferences.CustomSize = new(400, 420);
                _ = await ApplicationView.GetForCurrentView().TryEnterViewModeAsync(ApplicationViewMode.CompactOverlay, preferences);
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

            // This will automatically save the playlist to the db
            await playlist.AddSongAsync(_selectedItem);
        }

        private async void Item_Click(object sender, RoutedEventArgs e)
        {
            PlaylistViewModel playlist = (sender as MenuFlyoutItem).Tag as PlaylistViewModel;

            await playlist.AddSongAsync(_selectedItem);
        }

        private void ShowArtist_Click(object sender, RoutedEventArgs e)
        {
            if (!_selectedItem.IsOnline && MainPage.Current != null)
            {
                MainPage.Current.ContentFrame.Navigate(typeof(ArtistSongsPage), _selectedItem.Artist);
            }
        }

        private void ShowAlbum_Click(object sender, RoutedEventArgs e)
        {
            if (!_selectedItem.IsOnline && MainPage.Current != null)
            {
                MainPage.Current.ContentFrame.Navigate(typeof(AlbumSongsPage), _selectedItem.Album);
            }
        }

        private async void ShowinFE_Click(object sender, RoutedEventArgs e)
        {
            string folderlocation = _selectedItem.Location;
            string filename = _selectedItem.Filename;

            try
            {
                StorageFolder folder = await StorageFolder.GetFolderFromPathAsync(folderlocation.Replace(filename, ""));

                var t = new FolderLauncherOptions();
                foreach (var file in await folder.GetFilesAsync())
                {
                    t.ItemsToSelect.Add(file);
                }

                await Launcher.LaunchFolderAsync(folder, t);
            }
            catch
            {

            }
        }

        private void MainList_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            if ((e.OriginalSource as FrameworkElement).DataContext is SongViewModel song)
            {
                _selectedItem = song;
                SongFlyout.ShowAt(QueueList, e.GetPosition(QueueList));
            }
        }

        private async void PropsHover_Click(object sender, RoutedEventArgs e)
        {
            if ((e.OriginalSource as FrameworkElement).DataContext is SongViewModel song)
            {
                await song.StartEditAsync();
            }
        }

        private void PlayHover_Click(object sender, RoutedEventArgs e)
        {
            if ((e.OriginalSource as FrameworkElement).DataContext is SongViewModel song)
            {
                App.MPViewModel.PlaybackList.MoveTo((uint)App.MPViewModel.QueuedItems.IndexOf(song));
            }
        }

        private async void Remove1_Click(object sender, RoutedEventArgs e)
        {
            App.MPViewModel.QueuedItems.Remove(_selectedItem);
            App.MPViewModel.PlaybackList.Items.Remove(await _selectedItem.AsPlaybackItemAsync());
        }

        private void Album_Click(Hyperlink sender, HyperlinkClickEventArgs args)
            => EventsLogic.GoToAlbum(sender);

        private void Artist_Click(Hyperlink sender, HyperlinkClickEventArgs args)
            => EventsLogic.GoToArtist(sender);

        private void Grid_PointerEntered(object sender, PointerRoutedEventArgs e)
            => EventsLogic.FocusSong(ref _selectedItem, e);

        private void Grid_PointerExited(object sender, PointerRoutedEventArgs e)
            => EventsLogic.UnfocusSong(ref _selectedItem, e);

        private void MainList_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            int itemIndex = App.MPViewModel.QueuedItems.IndexOf(_selectedItem);

            if (itemIndex < 0)
            {
                return;
            }

            App.MPViewModel.PlaybackList.MoveTo((uint)itemIndex);
        }

        private void PlayerControls_ShufflingChanged(object sender, bool e)
            => App.MPViewModel.ShuffleEnabled = e;
    }
}