using Rise.App.Dialogs;
using Rise.App.Settings;
using Rise.App.ViewModels;
using Rise.Common.Constants;
using Rise.Common.Enums;
using Rise.Common.Extensions;
using Rise.Common.Helpers;
using Rise.Common.Interfaces;
using Rise.Data.Sources;
using Rise.Data.ViewModels;
using Rise.Effects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Foundation.Collections;
using Windows.Graphics.Imaging;
using Windows.Media;
using Windows.Media.Playback;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using NavigationViewItem = Microsoft.UI.Xaml.Controls.NavigationViewItem;

namespace Rise.App.Views
{
    /// <summary>
    /// Main app page, hosts the NavigationView and ContentFrame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private MainViewModel MViewModel => App.MViewModel;
        private SettingsViewModel SViewModel => App.SViewModel;

        private MediaPlaybackViewModel MPViewModel => App.MPViewModel;
        private LastFMViewModel LMViewModel => App.LMViewModel;

        private NavViewDataSource NavDataSource => App.NavDataSource;

        private NavigationViewItem RightClickedItem { get; set; }

        // This is static to allow it to persist during an
        // individual session. When the user exits the app,
        // state restoration is relegated to SuspensionManager
        private static string _navState;

        private readonly Dictionary<string, Type> Destinations = new()
        {
            { "HomePage", typeof(HomePage) },
            { "PlaylistsPage", typeof(PlaylistsPage) },
            { "SongsPage", typeof(SongsPage) },
            { "ArtistsPage", typeof(ArtistsPage) },
            { "AlbumsPage", typeof(AlbumsPage) },
            { "GenresPage", typeof(GenresPage) },
            { "LocalVideosPage", typeof(LocalVideosPage) },
            { "DiscyPage", typeof(DiscyPage) }
        };

        private readonly Dictionary<string, Type> UnlistedDestinations = new()
        {
            { "PlaylistsPage", typeof(PlaylistDetailsPage) },
            { "ArtistsPage", typeof(ArtistSongsPage) },
            { "AlbumsPage", typeof(AlbumSongsPage) },
            { "GenresPage", typeof(GenreSongsPage) }
        };

        public MainPage()
        {
            InitializeComponent();

            Loaded += MainPage_Loaded;
            Unloaded += MainPage_Unloaded;

            SuspensionManager.RegisterFrame(ContentFrame, "NavViewFrame");

            MViewModel.IndexingStarted += MViewModel_IndexingStarted;
            MViewModel.IndexingFinished += MViewModel_IndexingFinished;

            MPViewModel.PlayingItemChanged += MPViewModel_PlayingItemChanged;

            AppTitleBar.SetTitleBarForCurrentView();

            var coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
            UpdateTitleBarLayout(coreTitleBar);

            coreTitleBar.LayoutMetricsChanged += CoreTitleBar_LayoutMetricsChanged;
        }

        private void MainPage_Unloaded(object sender, RoutedEventArgs e)
        {
            MViewModel.IndexingStarted -= MViewModel_IndexingStarted;
            MViewModel.IndexingFinished -= MViewModel_IndexingFinished;

            MPViewModel.MediaPlayerRecreated -= OnMediaPlayerRecreated;
            MPViewModel.PlayingItemChanged -= MPViewModel_PlayingItemChanged;

            Bindings.StopTracking();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            if (!string.IsNullOrEmpty(_navState))
                ContentFrame.SetNavigationState(_navState);

            if (MPViewModel.PlayerCreated)
                UpdatePlayer(MPViewModel.Player);
            else
                MPViewModel.MediaPlayerRecreated += OnMediaPlayerRecreated;

            await HandleViewModelColorSettingAsync();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            _navState = ContentFrame.GetNavigationState();
        }

        private async void MPViewModel_PlayingItemChanged(object sender, IMediaItem e)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                await HandleViewModelColorSettingAsync();
            });

            if (e?.ItemType == MediaPlaybackType.Music)
            {
                await LMViewModel.TryScrobbleItemAsync(e);
            }
        }

        private void OnContentFrameSizeChanged(object sender, SizeChangedEventArgs e)
        {
            switch (e.NewSize.Width)
            {
                case >= 725:
                    VisualStateManager.GoToState(this, "WideContentAreaLayout", true);
                    break;
                case >= 550:
                    VisualStateManager.GoToState(this, "MediumContentAreaLayout", true);
                    break;
                default:
                    VisualStateManager.GoToState(this, "NarrowContentAreaLayout", true);
                    break;
            }
        }

        private async void OnMediaPlayerRecreated(object sender, MediaPlayer e)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                UpdatePlayer(e);
            });
        }

        private void UpdatePlayer(MediaPlayer player)
        {
            MainPlayer.SetMediaPlayer(player);

            if (!MPViewModel.Effects.Any(e => e.EffectClassType == typeof(EqualizerEffect)))
            {
                var config = new PropertySet { ["Gain"] = SViewModel.EqualizerGain, ["Enabled"] = SViewModel.EqualizerEnabled };
                MPViewModel.AddEffect(new(typeof(EqualizerEffect), false, true, config));
            }
        }

        private void PlayerControls_ShufflingChanged(object sender, bool e)
            => MPViewModel.ShuffleEnabled = e;

        private void OnDisplayItemClick(object sender, RoutedEventArgs e)
            => GoToNowPlaying();

        private void OnDisplayItemRightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            if (MPViewModel.PlayingItem == null) return;
            if (MPViewModel.PlayingItem.ItemType == MediaPlaybackType.Video)
                PlayingItemVideoFlyout.ShowAt(MainPlayer);
            else
                PlayingItemMusicFlyout.ShowAt(MainPlayer);
        }

        private async void OnCompactOverlayButtonClick(object sender, RoutedEventArgs e)
        {
            if (MPViewModel.PlayingItem == null) return;

            await ApplicationView.GetForCurrentView().
                TryEnterViewModeAsync(ApplicationViewMode.CompactOverlay);
            GoToNowPlaying();
        }

        private void OnOverlayButtonClick(object sender, RoutedEventArgs e)
            => GoToNowPlaying();

        private void GoToNowPlaying()
        {
            if (MPViewModel.PlayingItem == null) return;
            if (MPViewModel.PlayingItem.ItemType == MediaPlaybackType.Video)
                Frame.Navigate(typeof(VideoPlaybackPage));
            else
                Frame.Navigate(typeof(NowPlayingPage));
        }

        private async void MainPage_Loaded(object sender, RoutedEventArgs args)
        {
            if (!App.MainPageLoaded)
            {
                // Sidebar icons
                await NavDataSource.PopulateGroupsAsync();

                // Startup setting
                if (ContentFrame.Content == null)
                {
                    ContentFrame.Navigate(Destinations[SViewModel.Open]);
                }

                if (SViewModel.AutoIndexingEnabled)
                {
                    await Task.Run(async () => await App.MViewModel.StartFullCrawlAsync());
                }

                UpdateTitleBarItems(NavView);
                App.MainPageLoaded = true;
            }
        }

        private async void MViewModel_IndexingStarted(object sender, EventArgs e)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                AddedTip.IsOpen = false;
                CheckTip.IsOpen = true;
            });
        }

        private async void MViewModel_IndexingFinished(object sender, IndexingFinishedEventArgs e)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                CheckTip.IsOpen = false;
                AddedTip.IsOpen = true;

                await Task.Delay(3000);
                AddedTip.IsOpen = false;
            });
        }

        private void NavigationViewControl_DisplayModeChanged(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewDisplayModeChangedEventArgs args)
            => UpdateTitleBarItems(sender);

        private void CoreTitleBar_LayoutMetricsChanged(CoreApplicationViewTitleBar sender, object args)
            => UpdateTitleBarLayout(sender);

        /// <summary>
        /// Update the TitleBar layout.
        /// </summary>
        private void UpdateTitleBarLayout(CoreApplicationViewTitleBar coreTitleBar)
        {
            // Ensure the custom title bar does not overlap window caption controls
            Thickness currMargin = AppTitleBar.Margin;
            AppTitleBar.Margin = new Thickness(currMargin.Left, currMargin.Top, coreTitleBar.SystemOverlayRightInset, currMargin.Bottom);

            currMargin = ControlsPanel.Margin;
            ControlsPanel.Margin = new Thickness(currMargin.Left, currMargin.Top, coreTitleBar.SystemOverlayRightInset, currMargin.Bottom);
        }

        /// <summary>
        /// Update the TitleBar content layout depending on NavigationView DisplayMode.
        /// </summary>
        private void UpdateTitleBarItems(Microsoft.UI.Xaml.Controls.NavigationView navView)
        {
            const int expandedIndent = 48;
            int minimalIndent = 104;

            // If the back button is not visible, reduce the TitleBar content indent.
            if (navView.IsBackButtonVisible.Equals(Microsoft.UI.Xaml.Controls.NavigationViewBackButtonVisible.Collapsed))
            {
                minimalIndent = 48;
            }

            Thickness currMargin = AppTitleBar.Margin;

            // Set the TitleBar margin dependent on NavigationView display mode
            if (navView.DisplayMode == Microsoft.UI.Xaml.Controls.NavigationViewDisplayMode.Minimal)
            {
                AppTitleBar.Margin = new Thickness(minimalIndent, currMargin.Top, currMargin.Right, currMargin.Bottom);
                ControlsPanel.Margin = new Thickness(minimalIndent + 36, currMargin.Top, currMargin.Right, currMargin.Bottom);
            }
            else
            {
                AppTitleBar.Margin = new Thickness(expandedIndent, currMargin.Top, currMargin.Right, currMargin.Bottom);
                ControlsPanel.Margin = new Thickness(expandedIndent + AppData.DesiredSize.Width + 132, currMargin.Top, currMargin.Right, currMargin.Bottom);
            }
        }

        #region Navigation
        /// <summary>
        /// Invoked whenever navigation happens within a frame.
        /// </summary>
        /// <param name="sender">Frame that navigated.</param>
        /// <param name="e">Details about the navigation.</param>
        private void OnNavigated(object sender, NavigationEventArgs e)
        {
            var type = this.ContentFrame.CurrentSourcePageType;
            bool hasKey = this.Destinations.TryGetKey(type, out var key);

            // We need to handle unlisted destinations
            if (!hasKey)
                hasKey = this.UnlistedDestinations.TryGetKey(type, out key);

            if (hasKey)
            {
                bool hasItem = this.NavDataSource.TryGetItem(key, out var item);
                if (hasItem)
                    this.NavView.SelectedItem = item;
            }
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        private void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// Invoked when a NavView item is invoked.
        /// </summary>
        /// <param name="sender">The NavigationView that contains the item.</param>
        /// <param name="args">Details about the item invocation.</param>
        private void NavigationView_ItemInvoked(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewItemInvokedEventArgs args)
        {
            string tag = args?.InvokedItemContainer?.Tag?.ToString();

            if (tag != null)
            {
                if (tag == "SettingsPage")
                {
                    this.Frame.Navigate(typeof(AllSettingsPage));
                    return;
                }

                if (this.ContentFrame.SourcePageType != Destinations[tag])
                {
                    this.ContentFrame.Navigate(Destinations[tag],
                        null, args.RecommendedNavigationTransitionInfo);
                }
            }
        }

        /// <summary>
        /// Invoked when an access key for an element inside a NavView is invoked.
        /// </summary>
        /// <param name="sender">The element associated with the access key that
        /// was invoked.</param>
        /// <param name="args">Details about the key invocation.</param>
        private void NavigationViewItem_AccessKeyInvoked(UIElement sender, AccessKeyInvokedEventArgs args)
        {
            string tag = ((FrameworkElement)sender).Tag.ToString();

            if (tag != null)
            {
                if (tag == "SettingsPage")
                {
                    this.Frame.Navigate(typeof(AllSettingsPage));
                    return;
                }

                if (this.ContentFrame.SourcePageType != Destinations[tag])
                {
                    this.ContentFrame.Navigate(Destinations[tag]);
                }
            }
        }

        /// <summary>
        /// Invoked when a NavView's back button is clicked.
        /// </summary>
        /// <param name="sender">The NavigationView that contains the button.</param>
        /// <param name="args">Details about the button click.</param>
        private void NavigationView_BackRequested(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewBackRequestedEventArgs args)
        {
            this.ContentFrame.GoBack();
        }
        #endregion

        public async Task HandleViewModelColorSettingAsync()
        {
            if (SViewModel.SelectedGlaze == GlazeTypes.MediaThumbnail)
            {
                if (MPViewModel.PlayingItem != null)
                {
                    var thumbUri = new Uri(MPViewModel.PlayingItem.Thumbnail);
                    var thumbStrm = RandomAccessStreamReference.CreateFromUri(thumbUri);

                    using var stream = await thumbStrm.OpenReadAsync();

                    var decoder = await BitmapDecoder.CreateAsync(stream);
                    var colorThief = new ColorThiefDotNet.ColorThief();

                    var stolen = (await colorThief.GetColor(decoder)).Color;
                    SViewModel.GlazeColors = Color.FromArgb(25, stolen.R, stolen.G, stolen.B);
                }
                else
                {
                    SViewModel.GlazeColors = Colors.Transparent;
                }
            }
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            _ = await typeof(Web.FeedbackPage).
                ShowInApplicationViewAsync(null, 375, 600, true);
        }

        private async void StartScan_Click(object sender, RoutedEventArgs e)
        {
            ProfileMenu.Hide();
            OpenSync.Visibility = Visibility.Collapsed;
            IsScanning.Visibility = Visibility.Visible;
            await Task.Run(async () => await App.MViewModel.StartFullCrawlAsync());
            IsScanning.Visibility = Visibility.Collapsed;
            OpenSync.Visibility = Visibility.Visible;
        }

        private void OpenSettings_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(AllSettingsPage));
        }

        private void HideItem_Click(object sender, RoutedEventArgs e)
            => NavDataSource.ChangeItemVisibility(RightClickedItem.Tag.ToString(), false);

        private void HideSection_Click(object sender, RoutedEventArgs e)
        {
            _ = NavDataSource.TryGetItem(RightClickedItem.Tag.ToString(), out var item);
            NavDataSource.HideGroup(item.HeaderGroup);
        }

        private void MoveUp_Click(object sender, RoutedEventArgs e)
            => NavDataSource.MoveUp(RightClickedItem.Tag.ToString());

        private void MoveDown_Click(object sender, RoutedEventArgs e)
            => NavDataSource.MoveDown(RightClickedItem.Tag.ToString());

        private void ToTop_Click(object sender, RoutedEventArgs e)
            => NavDataSource.MoveToTop(RightClickedItem.Tag.ToString());

        private void ToBottom_Click(object sender, RoutedEventArgs e)
            => NavDataSource.MoveToBottom(RightClickedItem.Tag.ToString());

        private void NavigationView_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            DependencyObject source = e.OriginalSource as DependencyObject;

            if (source.FindVisualParent<NavigationViewItem>()
                is NavigationViewItem item && !item.Tag.ToString().Equals("SettingsPage"))
            {
                RightClickedItem = item;
                string tag = item.Tag.ToString();

                if (tag.Equals("LocalVideosPage") || tag.Equals("DiscyPage"))
                {
                    NavViewLightItemFlyout.ShowAt(NavView, e.GetPosition(NavView));
                }
                else
                {
                    bool up = NavDataSource.CanMoveUp(tag);
                    bool down = NavDataSource.CanMoveDown(tag);

                    TopOption.IsEnabled = up;
                    UpOption.IsEnabled = up;

                    DownOption.IsEnabled = down;
                    BottomOption.IsEnabled = down;

                    NavViewItemFlyout.ShowAt(NavView, e.GetPosition(NavView));
                }
            }
        }

        private async void Messages_Click(object sender, RoutedEventArgs e)
        {
            ContentDialog dialog = new()
            {
                Title = "Messages & reports",
                CloseButtonText = "Close",
                DefaultButton = ContentDialogButton.Primary,
                Content = new MessagesDialog()
            };

            _ = await dialog.ShowAsync();
        }

        private async void Support_Click(object sender, RoutedEventArgs e)
        => await URLs.Support.LaunchAsync();

        private async void BigSearch_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            SearchItemViewModel searchItem = args.SelectedItem as SearchItemViewModel;
            sender.Text = searchItem.Title;
            sender.IsSuggestionListOpen = false;

            switch (searchItem.ItemType)
            {
                case "Album":
                    AlbumViewModel album = App.MViewModel.Albums.FirstOrDefault(a => a.Title.Equals(searchItem.Title));
                    _ = ContentFrame.Navigate(typeof(AlbumSongsPage), album.Model.Id);
                    break;

                case "Song":
                    SongViewModel song = App.MViewModel.Songs.FirstOrDefault(s => s.Title.Equals(searchItem.Title));
                    await MPViewModel.PlaySingleItemAsync(song);
                    break;

                case "Artist":
                    ArtistViewModel artist = App.MViewModel.Artists.FirstOrDefault(a => a.Name.Equals(searchItem.Title));
                    ContentFrame.Navigate(typeof(ArtistSongsPage), artist.Model.Id);
                    break;
            }
        }

        private void BigSearch_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                List<SearchItemViewModel> suitableItems = new();
                List<ArtistViewModel> suitableArtists = new();
                List<SongViewModel> suitableSongs = new();
                List<AlbumViewModel> suitableAlbums = new();

                int maxCount = 4;

                string[] splitText = sender.Text.ToLower().Split(" ");

                foreach (AlbumViewModel album in MViewModel.Albums)
                {
                    bool found = splitText.All((key) =>
                    {
                        return album.Title.ToLower().Contains(key);
                    });

                    if (found && suitableAlbums.Count < maxCount)
                    {
                        suitableItems.Add(new SearchItemViewModel
                        {
                            Title = album.Title,
                            Subtitle = $"{album.Artist} - {album.Genres}",
                            ItemType = "Album",
                            Thumbnail = album.Thumbnail
                        });
                        suitableAlbums.Add(album);
                    }
                }

                foreach (SongViewModel song in MViewModel.Songs)
                {
                    bool found = splitText.All((key) =>
                    {
                        return song.Title.ToLower().Contains(key);
                    });

                    if (found && suitableSongs.Count < maxCount)
                    {
                        suitableItems.Add(new SearchItemViewModel
                        {
                            Title = song.Title,
                            Subtitle = $"{song.Artist} - {song.Genres}",
                            ItemType = "Song",
                            Thumbnail = song.Thumbnail
                        });
                        suitableSongs.Add(song);
                    }
                }

                foreach (ArtistViewModel artist in MViewModel.Artists)
                {
                    bool found = splitText.All((key) =>
                    {
                        return artist.Name.ToLower().Contains(key);
                    });

                    if (found && suitableArtists.Count < maxCount)
                    {
                        suitableItems.Add(new SearchItemViewModel
                        {
                            Title = artist.Name,
                            ItemType = "Artist",
                            Thumbnail = artist.Picture
                        });
                        suitableArtists.Add(artist);
                    }
                }

                sender.ItemsSource = suitableItems;
            }
        }

        private async void Account_Click(object sender, RoutedEventArgs e)
        {
            if (Acc.Text != "Add an account")
            {
                string url = "https://www.last.fm/user/" + Acc.Text;
                _ = await Windows.System.Launcher.LaunchUriAsync(new Uri(url));
            }
            else
            {
                Frame.Navigate(typeof(AllSettingsPage));
            }
        }

        private void BigSearch_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            _ = ContentFrame.Navigate(typeof(SearchResultsPage), sender.Text);
        }

        private void BigSearch_GotFocus(object sender, RoutedEventArgs e)
        {
            App.MViewModel.IsSearchActive = true;
        }

        private void BigSearch_LostFocus(object sender, RoutedEventArgs e)
        {
            App.MViewModel.IsSearchActive = false;
        }

        public static Visibility IsStringEmpty(string str)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return Visibility.Collapsed;
            }
            else
            {
                return Visibility.Visible;
            }
        }

        private void AddedTip_ActionButtonClick(Microsoft.UI.Xaml.Controls.TeachingTip sender, object args)
        {
            _ = Frame.Navigate(typeof(AllSettingsPage));
        }

        private void OnAlbumButtonClick(object sender, RoutedEventArgs e)
        {
            if (MPViewModel.PlayingItem.ItemType != MediaPlaybackType.Music)
                return;

            AlbumViewModel album = MViewModel.Albums.AsParallel().
                FirstOrDefault(a => a.Title == MPViewModel.PlayingItem.ExtraInfo);
            ContentFrame.Navigate(typeof(AlbumSongsPage), album.Model.Id);

            PlayingItemMusicFlyout.Hide();
        }

        private void OnArtistButtonClick(object sender, RoutedEventArgs e)
        {
            if (MPViewModel.PlayingItem.ItemType != MediaPlaybackType.Music)
                return;

            ArtistViewModel artist = MViewModel.Artists.AsParallel().
                FirstOrDefault(a => a.Name == MPViewModel.PlayingItem.Subtitle);
            ContentFrame.Navigate(typeof(ArtistSongsPage), artist.Model.Id);

            PlayingItemMusicFlyout.Hide();
        }
    }

    public class NavViewItemTemplateSelector : DataTemplateSelector
    {
        public DataTemplate GlyphIconTemplate { get; set; }
        public DataTemplate AssetIconTemplate { get; set; }
        public DataTemplate HeaderTemplate { get; set; }
        public DataTemplate SeparatorTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item)
        {
            var itm = (NavViewItemViewModel)item;
            return GetTemplate(itm);
        }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            var itm = (NavViewItemViewModel)item;
            return GetTemplate(itm);
        }

        /// <summary>
        /// Gets the desired DataTemplate based on the item type.
        /// </summary>
        private DataTemplate GetTemplate(NavViewItemViewModel item)
        {
            switch (item.ItemType)
            {
                case NavViewItemType.Header:
                    return HeaderTemplate;

                case NavViewItemType.Item:
                    if (item.Icon.IsValidUri(UriKind.Absolute))
                    {
                        return AssetIconTemplate;
                    }

                    return GlyphIconTemplate;

                case NavViewItemType.Separator:
                    return SeparatorTemplate;

                default:
                    return null;
            }
        }
    }
}
