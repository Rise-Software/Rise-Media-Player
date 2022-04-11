using Microsoft.Toolkit.Uwp.UI;
using Rise.App.UserControls;
using Rise.App.ViewModels;
using System;
using System.Diagnostics;
using Windows.Foundation;
using Windows.Media;
using Windows.Media.Playback;
using Windows.Storage;
using Windows.System;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

namespace Rise.App.Views
{
    public sealed partial class QueuePage : Page
    {
        /// <summary>
        /// Gets the app-wide NPViewModel instance.
        /// </summary>
        private static PlaybackViewModel ViewModel => App.PViewModel;
        private SongViewModel _selectedSong;
        private readonly AdvancedCollectionView Songs = new(ViewModel.PlayingSongs);
        private MainViewModel MViewModel => App.MViewModel;

        private SongViewModel queuesong;
        private SongViewModel SelectedSong
        {
            get => MViewModel.SelectedSong;
            set => MViewModel.SelectedSong = value;
        }



        public QueuePage()
        {
            InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;

            Loaded += (s, e) =>
            {
                Queue.Checked += ToggleButton_Checked;
                AlbumQueue.Checked += ToggleButton_Checked;
                ApplyPlaylistItems(AddTo);
            };
        }

        private void PlaybackList_CurrentItemChanged(MediaPlaybackList sender, CurrentMediaPlaybackItemChangedEventArgs args)
        {
            if (args.Reason == MediaPlaybackItemChangedReason.Error)
            {
                Debug.WriteLine("Error when going to next item.");
                return;
            }

            if (sender.CurrentItem != null)
            {
                if (sender.CurrentItem.GetDisplayProperties().Type == MediaPlaybackType.Music)
                {
                    using (Songs.DeferRefresh())
                    {
                        var props = sender.CurrentItem.GetDisplayProperties();
                        Songs.Filter = s =>
                            ((SongViewModel)s).Album == props.MusicProperties.AlbumTitle;
                    }

                    Songs.Refresh();
                }
            }

        }

        private async void ShowArtist_Click(object sender, RoutedEventArgs e)
        {
            if (!_selectedSong.IsOnline)
            {
                var preferences = ViewModePreferences.CreateDefault(ApplicationViewMode.CompactOverlay);
                preferences.CustomSize = new Size(600, 700);
                _ = await ApplicationView.GetForCurrentView().TryEnterViewModeAsync(ApplicationViewMode.Default, preferences);

                MainPage.Current.AppTitleBar.Visibility = Visibility.Visible;
                MainPage.Current.AppTitleBar.SetupTitleBar();

                MainPage.Current.AppTitleBar.IsHitTestVisible = true;
                MainPage.Current.ContentFrame.Navigate(typeof(ArtistSongsPage), _selectedSong.Artist);
            }
        }

        private async void ShowAlbum_Click(object sender, RoutedEventArgs e)
        {
            if (!_selectedSong.IsOnline)
            {
                var preferences = ViewModePreferences.CreateDefault(ApplicationViewMode.CompactOverlay);
                preferences.CustomSize = new Size(600, 700);
                _ = await ApplicationView.GetForCurrentView().TryEnterViewModeAsync(ApplicationViewMode.Default, preferences);

                MainPage.Current.AppTitleBar.Visibility = Visibility.Visible;
                MainPage.Current.AppTitleBar.SetupTitleBar();

                MainPage.Current.AppTitleBar.IsHitTestVisible = true;
                MainPage.Current.ContentFrame.Navigate(typeof(AlbumSongsPage), _selectedSong.Album);
            }
        }

        private async void ShowinFE_Click(object sender, RoutedEventArgs e)
        {
            string folderlocation = _selectedSong.Location;
            string filename = _selectedSong.Filename;
            string result = folderlocation.Replace(filename, "");
            Debug.WriteLine(result);

            try
            {
                StorageFolder folder = await StorageFolder.GetFolderFromPathAsync(result);
                await Launcher.LaunchFolderAsync(folder);
                var t = new FolderLauncherOptions();
                foreach (var SelectedSong in await folder.GetFilesAsync())
                {
                    t.ItemsToSelect.Add(SelectedSong);
                }
            }
            catch
            {

            }
        }

        private async void PropsHover_Click(object sender, RoutedEventArgs e)
        {
            if ((e.OriginalSource as FrameworkElement).DataContext is SongViewModel song)
            {
                SelectedSong = song;
                await SelectedSong.StartEditAsync();
            }
        }

        private async void PlayHover_Click(object sender, RoutedEventArgs e)
        {
            if ((e.OriginalSource as FrameworkElement).DataContext is SongViewModel song)
            {
                SelectedSong = song;
                App.PViewModel.PlaybackList.MoveTo((uint)Songs.IndexOf(SelectedSong));
            }
        }

        private void Album_Click(Hyperlink sender, HyperlinkClickEventArgs args)
            => EventsLogic.GoToAlbum(sender);

        private void Artist_Click(Hyperlink sender, HyperlinkClickEventArgs args)
            => EventsLogic.GoToArtist(sender);

        private void Grid_PointerEntered(object sender, PointerRoutedEventArgs e)
            => EventsLogic.FocusSong(ref queuesong, e);

        private void Grid_PointerExited(object sender, PointerRoutedEventArgs e)
            => EventsLogic.UnfocusSong(ref queuesong, e);
        private void MainList_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            int itemIndex = ViewModel.PlayingSongs.
                IndexOf(MainList.SelectedItem as SongViewModel);

            if (itemIndex < 0)
            {
                return;
            }

            ViewModel.PlaybackList.MoveTo((uint)itemIndex);
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (Frame.CanGoBack)
            {
                Frame.GoBack();
            }
        }

        private void ToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            var btn = sender as ToggleButton;
            switch (btn.Tag.ToString())
            {
                case "QueueItem":
                    AlbumQueue.IsChecked = false;

                    using (Songs.DeferRefresh())
                    {
                        Songs.Filter = null;
                    }

                    Songs.Refresh();
                    ViewModel.PlaybackList.CurrentItemChanged -= PlaybackList_CurrentItemChanged;
                    break;

                default:
                    Queue.IsChecked = false;
                    if (ViewModel.PlaybackList.CurrentItem != null)
                    {
                        if (ViewModel.PlaybackList.CurrentItem.GetDisplayProperties().Type == MediaPlaybackType.Music)
                        {
                            var props = ViewModel.PlaybackList.CurrentItem.GetDisplayProperties();
                            Songs.Filter = s =>
                                ((SongViewModel)s).Album == props.MusicProperties.AlbumTitle;
                        }
                    }

                    ViewModel.PlaybackList.CurrentItemChanged += PlaybackList_CurrentItemChanged;
                    break;
            }
        }

        private void MainList_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            if ((e.OriginalSource as FrameworkElement).DataContext is SongViewModel song)
            {
                _selectedSong = song;
                SongFlyout.ShowAt(MainList, e.GetPosition(MainList));
            }
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            App.PViewModel.PlaybackList.MoveTo((uint)Songs.IndexOf(_selectedSong));
        }

        private async void Remove1_Click(object sender, RoutedEventArgs e)
        {
            App.PViewModel.PlayingSongs.Remove(_selectedSong);
            App.PViewModel.PlaybackList.Items.Remove(await _selectedSong.AsPlaybackItemAsync());
        }

        private void ApplyPlaylistItems(MenuFlyout addTo)
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
            await playlist.AddSongAsync(_selectedSong);
        }

        private async void Item_Click(object sender, RoutedEventArgs e)
        {
            PlaylistViewModel playlist = (sender as MenuFlyoutItem).Tag as PlaylistViewModel;

            await playlist.AddSongAsync(_selectedSong);
        }

        private void QueueNav_SelectionChanged(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewSelectionChangedEventArgs args)
        {
            var selectedItem = (Microsoft.UI.Xaml.Controls.NavigationViewItem)args.SelectedItem;
            string selectedItemTag = selectedItem.Tag as string;

            switch (selectedItemTag)
            {
                case "QueueItem":

                    using (Songs.DeferRefresh())
                    {
                        Songs.Filter = null;
                    }

                    Songs.Refresh();
                    ViewModel.PlaybackList.CurrentItemChanged -= PlaybackList_CurrentItemChanged;
                    break;

                default:
                    if (ViewModel.PlaybackList.CurrentItem != null)
                    {
                        if (ViewModel.PlaybackList.CurrentItem.GetDisplayProperties().Type == MediaPlaybackType.Music)
                        {
                            var props = ViewModel.PlaybackList.CurrentItem.GetDisplayProperties();
                            Songs.Filter = s =>
                                ((SongViewModel)s).Album == props.MusicProperties.AlbumTitle;
                        }
                    }

                    ViewModel.PlaybackList.CurrentItemChanged += PlaybackList_CurrentItemChanged;
                    break;
            }
        }

        private async void Props_Click(object sender, RoutedEventArgs e)
        {
            if (!_selectedSong.IsOnline)
            {
                await _selectedSong.StartEditAsync();
            }
        }

    }
}
