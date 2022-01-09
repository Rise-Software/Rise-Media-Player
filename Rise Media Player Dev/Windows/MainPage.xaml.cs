﻿using Microsoft.Toolkit.Uwp.UI;
using Rise.App.Common;
using Rise.App.Dialogs;
using Rise.App.Settings;
using Rise.App.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.WindowManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using static Rise.App.Common.Enums;
using NavigationViewItem = Microsoft.UI.Xaml.Controls.NavigationViewItem;

namespace Rise.App.Views
{
    /// <summary>
    /// Main app page, hosts the NavigationView and ContentFrame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        #region Variables
        private SettingsViewModel SViewModel => App.SViewModel;
        private SidebarViewModel SBViewModel => App.SBViewModel;

        private AdvancedCollectionView Albums = App.MViewModel.FilteredAlbums;
        private AdvancedCollectionView Songs = App.MViewModel.FilteredSongs;
        private AdvancedCollectionView Artists = App.MViewModel.FilteredArtists;

        private bool IsInPageWithoutHeader = false;

        public static MainPage Current;

        public ObservableCollection<Crumb> Breadcrumbs { get; set; }
            = new ObservableCollection<Crumb>();

        public SettingsDialogContainer SDialog { get; }
            = new SettingsDialogContainer();

        private IDisposable SongsDefer { get; set; }
        private IDisposable AlbumsDefer { get; set; }
        private IDisposable ArtistsDefer { get; set; }
        private IDisposable GenresDefer { get; set; }
        private IDisposable VideosDefer { get; set; }
        private IDisposable PlaylistsDefer { get; set; }

        private NavigationViewItem RightClickedItem { get; set; }

        private AppWindow _nowPlayingWindow;
        #endregion

        public MainPage()
        {
            InitializeComponent();
            Current = this;
            SDialog.Content = new SettingsPage();
            Loaded += MainPage_Loaded;
            SizeChanged += MainPage_SizeChanged;

            NavigationCacheMode = NavigationCacheMode.Required;
            SuspensionManager.RegisterFrame(ContentFrame, "NavViewFrame");

            App.Indexer.Started += Indexer_Started;
            App.Indexer.Finished += Indexer_Finished;

            SViewModel.PropertyChanged += SViewModel_PropertyChanged;
            _ = NowPlayingFrame.Navigate(typeof(NowPlaying));
        }

        private void MainPage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (ApplicationView.GetForCurrentView().ViewMode == ApplicationViewMode.CompactOverlay)
            {
                OverlayModeContentPanel.Visibility = Visibility.Visible;
                AppTitleBar.Visibility = Visibility.Collapsed;
                ControlsPanel.Visibility = Visibility.Collapsed;
            }
            else
            {
                OverlayModeContentPanel.Visibility = Visibility.Collapsed;
                AppTitleBar.Visibility = Visibility.Visible;
                ControlsPanel.Visibility = Visibility.Visible;
            }

            if (e.NewSize.Width < 800 && IsInPageWithoutHeader)
            {
                ContentFrame.Margin = new Thickness(0, 48, 0, 0);
            } 
            else if (e.NewSize.Width < 800 && !IsInPageWithoutHeader)
            {
                ContentFrame.Margin = new Thickness(0);
            } 
            else if (e.NewSize.Width >= 800)
            {
                ContentFrame.Margin = new Thickness(0);
            }
        }

        private async void MainPage_Loaded(object sender, RoutedEventArgs args)
        {
            // Sidebar icons
            await SBViewModel.LoadItemsAsync();
            ChangeIconPack(SViewModel.CurrentPack);

            // Startup setting
            if (ContentFrame.Content == null)
            {
                await Navigate(SViewModel.Open);
            }

            FinishNavigation();

            App.MViewModel.CanIndex = true;
            _ = Task.Run(async () => await App.MViewModel.StartFullCrawlAsync());

            //PlayerElement.SetMediaPlayer(App.PViewModel.Player);
            _ = new ApplicationTitleBar(AppTitleBar, CoreTitleBar_LayoutMetricsChanged);
            await HandleViewModelColorSettingAsync();

            Loaded -= MainPage_Loaded;

            // When the page is loaded, initialize the titlebar and setup the player.
            Loaded += (s, e) =>
            {
                //PlayerElement.SetMediaPlayer(App.PViewModel.Player);
                _ = new ApplicationTitleBar(AppTitleBar, CoreTitleBar_LayoutMetricsChanged);
            };
        }

        private async void Indexer_Started()
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                AddedTip.IsOpen = false;
                CheckTip.IsOpen = true;

                SongsDefer = App.MViewModel.FilteredSongs.DeferRefresh();
                AlbumsDefer = App.MViewModel.FilteredAlbums.DeferRefresh();
                ArtistsDefer = App.MViewModel.FilteredArtists.DeferRefresh();
                GenresDefer = App.MViewModel.FilteredGenres.DeferRefresh();
                VideosDefer = App.MViewModel.FilteredVideos.DeferRefresh();
                PlaylistsDefer = App.MViewModel.FilteredPlaylists.DeferRefresh();
            });
        }

        private async void Indexer_Finished(object sender, int e)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                CheckTip.IsOpen = false;
                AddedTip.IsOpen = true;

                SongsDefer.Dispose();
                AlbumsDefer.Dispose();
                ArtistsDefer.Dispose();
                GenresDefer.Dispose();
                VideosDefer.Dispose();
                PlaylistsDefer.Dispose();

                App.MViewModel.FilteredSongs.Refresh();
                App.MViewModel.FilteredAlbums.Refresh();
                App.MViewModel.FilteredArtists.Refresh();
                App.MViewModel.FilteredGenres.Refresh();
                App.MViewModel.FilteredVideos.Refresh();
                App.MViewModel.FilteredPlaylists.Refresh();
            });
        }

        #region TitleBar
        // Update the TitleBar content layout.
        private void NavigationViewControl_DisplayModeChanged(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewDisplayModeChangedEventArgs args)
            => UpdateTitleBarItems(sender);

        private void CoreTitleBar_LayoutMetricsChanged(CoreApplicationViewTitleBar sender, object args)
            => UpdateTitleBarLayout(sender);

        /// <summary>
        /// Update the TitleBar layout.
        /// </summary>
        private void UpdateTitleBarLayout(CoreApplicationViewTitleBar coreTitleBar)
        {
            // Update title bar control size as needed to account for system size changes.
            AppTitleBar.Height = coreTitleBar.Height;

            // Ensure the custom title bar does not overlap window caption controls
            Thickness currMargin = AppTitleBar.Margin;
            AppTitleBar.Margin = new Thickness(currMargin.Left, currMargin.Top, coreTitleBar.SystemOverlayRightInset, currMargin.Bottom);
            ControlsPanel.Margin = new Thickness(48 + AppTitleBar.LabelWidth + 132, currMargin.Top, 48 + AppTitleBar.LabelWidth + 132, currMargin.Bottom);

            UpdateTitleBarItems(NavView);
        }

        /// <summary>
        /// Update the TitleBar content layout depending on NavigationView DisplayMode.
        /// </summary>
        public void UpdateTitleBarItems(Microsoft.UI.Xaml.Controls.NavigationView navView)
        {
            const int topIndent = 16;
            const int expandedIndent = 48;
            int minimalIndent = 104;

            // If the back button is not visible, reduce the TitleBar content indent.
            if (navView.IsBackButtonVisible.Equals(Microsoft.UI.Xaml.Controls.NavigationViewBackButtonVisible.Collapsed))
            {
                minimalIndent = 48;
            }

            Thickness currMargin = AppTitleBar.Margin;

            // Set the TitleBar margin dependent on NavigationView display mode
            if (navView.PaneDisplayMode == Microsoft.UI.Xaml.Controls.NavigationViewPaneDisplayMode.Top)
            {
                AppTitleBar.Margin = new Thickness(topIndent, currMargin.Top, currMargin.Right, currMargin.Bottom);
                ControlsPanel.Margin = new Thickness(topIndent + AppTitleBar.LabelWidth + 48, currMargin.Top, currMargin.Right, currMargin.Bottom);
            }
            else if (navView.DisplayMode == Microsoft.UI.Xaml.Controls.NavigationViewDisplayMode.Minimal)
            {
                AppTitleBar.Margin = new Thickness(minimalIndent, currMargin.Top, currMargin.Right, currMargin.Bottom);
                ControlsPanel.Margin = new Thickness(minimalIndent + 36, currMargin.Top, currMargin.Right, currMargin.Bottom);
            }
            else
            {
                AppTitleBar.Margin = new Thickness(expandedIndent, currMargin.Top, currMargin.Right, currMargin.Bottom);
                ControlsPanel.Margin = new Thickness(expandedIndent + AppTitleBar.LabelWidth + 132, currMargin.Top, expandedIndent + AppTitleBar.LabelWidth + 132, currMargin.Bottom);
            }
        }
        #endregion

        #region Navigation
        private async void NavView_ItemInvoked(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewItemInvokedEventArgs args)
        {
            string navTo = args?.InvokedItemContainer?.Tag?.ToString();
            if (navTo == ContentFrame.CurrentSourcePageType.ToString())
            {
                FinishNavigation();
                return;
            }

            if (navTo != null)
            {
                await Navigate(navTo);
            }
        }

        private async void NavViewItem_AccessKeyInvoked(UIElement sender, AccessKeyInvokedEventArgs args)
        {
            var item = sender as NavigationViewItem;
            string navTo = item.Tag.ToString();

            if (navTo == ContentFrame.CurrentSourcePageType.ToString())
            {
                FinishNavigation();
                return;
            }

            if (navTo != null)
            {
                await Navigate(navTo);
            }
        }

        private void NavView_BackRequested(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewBackRequestedEventArgs args)
            => ContentFrame.GoBack();

        private async Task Navigate(string navTo)
        {
            switch (navTo)
            {
                case "HomePage":
                    _ = ContentFrame.Navigate(typeof(HomePage));
                    break;

                case "NowPlayingPage":
                    if (_nowPlayingWindow == null)
                    {
                        _nowPlayingWindow = await typeof(NowPlaying).
                            PlaceInWindowAsync(AppWindowPresentationKind.Default, 320, 300, false);
                    }

                    _nowPlayingWindow.Closed += (s, e) =>
                    {
                        _nowPlayingWindow = null;
                    };
                    _ = await _nowPlayingWindow.TryShowAsync();
                    break;

                case "PlaylistsPage":
                    _ = ContentFrame.Navigate(typeof(PlaylistsPage));
                    break;

                case "SongsPage":
                    _ = ContentFrame.Navigate(typeof(SongsPage));
                    break;

                case "ArtistsPage":
                    _ = ContentFrame.Navigate(typeof(ArtistsPage));
                    break;

                case "AlbumsPage":
                    _ = ContentFrame.Navigate(typeof(AlbumsPage));
                    break;

                case "GenresPage":
                    _ = ContentFrame.Navigate(typeof(GenresPage));
                    break;

                case "LocalVideosPage":
                    _ = ContentFrame.Navigate(typeof(LocalVideosPage));
                    break;

                case "DiscyPage":
                    _ = ContentFrame.Navigate(typeof(DiscyPage));
                    break;

                case "SettingsPage":
                    _ = await SDialog.ShowAsync(ExistingDialogOptions.CloseExisting);
                    break;

                default:
                    break;
            }
        }

        private void ContentFrame_Navigated(object sender, NavigationEventArgs e)
        {
            App.MViewModel.SelectedSong = null;
            FinishNavigation();
        }

        public void FinishNavigation()
        {
            string type = ContentFrame.CurrentSourcePageType.ToString();
            string tag = type.Split('.').Last();

            Breadcrumbs.Clear();
            CrumbsHeader.Visibility = Visibility.Visible;

            switch (tag)
            {
                case "HomePage":
                    NavView.SelectedItem = SBViewModel.
                        Items.FirstOrDefault(i => i.Tag == tag);
                    IsInPageWithoutHeader = true;
                    return;

                case "AlbumSongsPage":
                    CrumbsHeader.Visibility = Visibility.Collapsed;
                    IsInPageWithoutHeader = true;
                    NavView.SelectedItem = SBViewModel.
                        Items.FirstOrDefault(i => i.Tag == "AlbumsPage");
                    return;

                case "PlaylistDetailsPage":
                    CrumbsHeader.Visibility = Visibility.Collapsed;
                    IsInPageWithoutHeader = true;
                    return;

                case "ArtistSongsPage":
                    CrumbsHeader.Visibility = Visibility.Collapsed;
                    IsInPageWithoutHeader = true;
                    NavView.SelectedItem = SBViewModel.
                        Items.FirstOrDefault(i => i.Tag == "ArtistsPage");
                    return;

                case "GenreSongsPage":
                    IsInPageWithoutHeader = false;
                    NavView.SelectedItem = SBViewModel.
                        Items.FirstOrDefault(i => i.Tag == "GenresPage");
                    return;

                case "SearchResultsPage":
                    CrumbsHeader.Visibility = Visibility.Collapsed;
                    IsInPageWithoutHeader = true;
                    return;

                case "DiscyPage":
                    IsInPageWithoutHeader = false;
                    return;

                default:
                    IsInPageWithoutHeader = false;
                    break;
            }

            foreach (NavViewItemViewModel item in SBViewModel.Items)
            {
                if (item.Tag == tag)
                {
                    NavView.SelectedItem = item;
                    Breadcrumbs.Add(new Crumb
                    {
                        Title = ResourceLoaders.SidebarLoader.GetString(item.LabelResource)
                    });
                    return;
                }
            }

            foreach (NavViewItemViewModel item in SBViewModel.FooterItems)
            {
                if (item.Tag == tag)
                {
                    NavView.SelectedItem = item;
                    Breadcrumbs.Add(new Crumb
                    {
                        Title = ResourceLoaders.SidebarLoader.GetString(item.LabelResource)
                    });
                    return;
                }
            }
        }
        #endregion

        #region Settings
        public void ChangeIconPack(string newIcons)
        {
            SBViewModel.ChangeIconPack(newIcons);

            // Refresh item templates.
            NavView.MenuItemsSource = null;
            NavView.FooterMenuItemsSource = null;

            NavView.MenuItemsSource = SBViewModel.Items;
            NavView.FooterMenuItemsSource = SBViewModel.FooterItems;
        }

        private async void SViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Color")
            {
                await HandleViewModelColorSettingAsync();
            }
        }

        public async Task HandleViewModelColorSettingAsync()
        {
            var uiSettings = new UISettings();
            Color accentColor = uiSettings.GetColorValue(UIColorType.Accent);

            byte opacity = 25;
            switch (SViewModel.Color)
            {
                case -3:
                    if (App.PViewModel.CurrentSong != null)
                    {
                        Uri imageUri = new Uri(App.PViewModel.CurrentSong.Thumbnail);
                        _Grid.Background = new SolidColorBrush(Colors.Transparent);
                        if (App.PViewModel.CurrentSong.Thumbnail != "ms-appx:///Assets/Default.png")
                        {
                            RandomAccessStreamReference random = RandomAccessStreamReference.CreateFromUri(imageUri);
                            using (IRandomAccessStream stream = await random.OpenReadAsync())
                            {
                                var decoder = await BitmapDecoder.CreateAsync(stream);
                                var colorThief = new ColorThiefDotNet.ColorThief();

                                var color = await colorThief.GetColor(decoder);
                                _Grid.Background = new SolidColorBrush(Color.FromArgb(opacity, color.Color.R, color.Color.G, color.Color.B));
                            }
                        }
                    }
                    break;

                case -2:
                    _Grid.Background = new SolidColorBrush(Color.FromArgb(opacity, accentColor.R, accentColor.G, accentColor.B));
                    break;

                case -1:
                    _Grid.Background = new SolidColorBrush(Colors.Transparent);
                    break;

                case 0:
                    _Grid.Background = new SolidColorBrush(Color.FromArgb(opacity, 255, 185, 0));
                    break;

                case 1:
                    _Grid.Background = new SolidColorBrush(Color.FromArgb(opacity, 255, 140, 0));
                    break;

                case 2:
                    _Grid.Background = new SolidColorBrush(Color.FromArgb(opacity, 247, 99, 12));
                    break;

                case 3:
                    _Grid.Background = new SolidColorBrush(Color.FromArgb(opacity, 202, 80, 16));
                    break;

                case 4:
                    _Grid.Background = new SolidColorBrush(Color.FromArgb(opacity, 218, 59, 1));
                    break;

                case 5:
                    _Grid.Background = new SolidColorBrush(Color.FromArgb(opacity, 239, 105, 80));
                    break;

                case 6:
                    _Grid.Background = new SolidColorBrush(Color.FromArgb(opacity, 209, 52, 56));
                    break;

                case 7:
                    _Grid.Background = new SolidColorBrush(Color.FromArgb(opacity, 255, 67, 67));
                    break;

                case 8:
                    _Grid.Background = new SolidColorBrush(Color.FromArgb(opacity, 231, 72, 86));
                    break;

                case 9:
                    _Grid.Background = new SolidColorBrush(Color.FromArgb(opacity, 232, 17, 35));
                    break;

                case 10:
                    _Grid.Background = new SolidColorBrush(Color.FromArgb(opacity, 234, 0, 94));
                    break;

                case 11:
                    _Grid.Background = new SolidColorBrush(Color.FromArgb(opacity, 195, 0, 82));
                    break;

                case 12:
                    _Grid.Background = new SolidColorBrush(Color.FromArgb(opacity, 227, 0, 140));
                    break;

                case 13:
                    _Grid.Background = new SolidColorBrush(Color.FromArgb(opacity, 191, 0, 119));
                    break;

                case 14:
                    _Grid.Background = new SolidColorBrush(Color.FromArgb(opacity, 194, 57, 179));
                    break;

                case 15:
                    _Grid.Background = new SolidColorBrush(Color.FromArgb(opacity, 154, 0, 137));
                    break;

                case 16:
                    _Grid.Background = new SolidColorBrush(Color.FromArgb(opacity, 0, 120, 212));
                    break;

                case 17:
                    _Grid.Background = new SolidColorBrush(Color.FromArgb(opacity, 0, 99, 177));
                    break;

                case 18:
                    _Grid.Background = new SolidColorBrush(Color.FromArgb(opacity, 142, 140, 216));
                    break;

                case 19:
                    _Grid.Background = new SolidColorBrush(Color.FromArgb(opacity, 107, 105, 214));
                    break;

                case 20:
                    _Grid.Background = new SolidColorBrush(Color.FromArgb(opacity, 135, 100, 184));
                    break;

                case 21:
                    _Grid.Background = new SolidColorBrush(Color.FromArgb(opacity, 116, 77, 169));
                    break;

                case 22:
                    _Grid.Background = new SolidColorBrush(Color.FromArgb(opacity, 177, 70, 194));
                    break;

                case 23:
                    _Grid.Background = new SolidColorBrush(Color.FromArgb(opacity, 136, 23, 152));
                    break;

                case 24:
                    _Grid.Background = new SolidColorBrush(Color.FromArgb(opacity, 0, 153, 188));
                    break;

                case 25:
                    _Grid.Background = new SolidColorBrush(Color.FromArgb(opacity, 45, 125, 154));
                    break;

                case 26:
                    _Grid.Background = new SolidColorBrush(Color.FromArgb(opacity, 0, 183, 195));
                    break;

                case 27:
                    _Grid.Background = new SolidColorBrush(Color.FromArgb(opacity, 3, 131, 135));
                    break;

                case 28:
                    _Grid.Background = new SolidColorBrush(Color.FromArgb(opacity, 0, 178, 148));
                    break;


                case 29:
                    _Grid.Background = new SolidColorBrush(Color.FromArgb(opacity, 1, 133, 116));
                    break;

                case 30:
                    _Grid.Background = new SolidColorBrush(Color.FromArgb(opacity, 0, 204, 106));
                    break;

                case 31:
                    _Grid.Background = new SolidColorBrush(Color.FromArgb(opacity, 16, 137, 62));
                    break;

                case 32:
                    _Grid.Background = new SolidColorBrush(Color.FromArgb(opacity, 122, 117, 116));
                    break;

                case 33:
                    _Grid.Background = new SolidColorBrush(Color.FromArgb(opacity, 93, 90, 88));
                    break;

                case 34:
                    _Grid.Background = new SolidColorBrush(Color.FromArgb(opacity, 104, 118, 138));
                    break;

                case 35:
                    _Grid.Background = new SolidColorBrush(Color.FromArgb(opacity, 81, 92, 107));
                    break;

                case 36:
                    _Grid.Background = new SolidColorBrush(Color.FromArgb(opacity, 86, 124, 115));
                    break;

                case 37:
                    _Grid.Background = new SolidColorBrush(Color.FromArgb(opacity, 72, 104, 96));
                    break;

                case 38:
                    _Grid.Background = new SolidColorBrush(Color.FromArgb(opacity, 73, 130, 5));
                    break;

                case 39:
                    _Grid.Background = new SolidColorBrush(Color.FromArgb(opacity, 16, 124, 16));
                    break;

                case 40:
                    _Grid.Background = new SolidColorBrush(Color.FromArgb(opacity, 118, 118, 118));
                    break;

                case 41:
                    _Grid.Background = new SolidColorBrush(Color.FromArgb(opacity, 76, 74, 72));
                    break;

                case 42:
                    _Grid.Background = new SolidColorBrush(Color.FromArgb(opacity, 105, 121, 126));
                    break;

                case 43:
                    _Grid.Background = new SolidColorBrush(Color.FromArgb(opacity, 74, 84, 89));
                    break;

                case 44:
                    _Grid.Background = new SolidColorBrush(Color.FromArgb(opacity, 100, 124, 100));
                    break;

                case 45:
                    _Grid.Background = new SolidColorBrush(Color.FromArgb(opacity, 82, 94, 84));
                    break;

                case 46:
                    _Grid.Background = new SolidColorBrush(Color.FromArgb(opacity, 132, 117, 69));
                    break;

                case 47:
                    _Grid.Background = new SolidColorBrush(Color.FromArgb(opacity, 126, 115, 95));
                    break;

            }
        }
        #endregion

        private async Task<bool> OpenPageAsWindowAsync(Type t)
        {
            var view = CoreApplication.CreateNewView();
            int id = 0;

            await view.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                var frame = new Frame();
                frame.Navigate(t, null);
                Window.Current.Content = frame;
                Window.Current.Activate();
                id = ApplicationView.GetForCurrentView().Id;
            });

            return await ApplicationViewSwitcher.TryShowAsStandaloneAsync(id);
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            await OpenPageAsWindowAsync(typeof(Web.FeedbackPage));
        }

        private async void StartScan_Click(object sender, RoutedEventArgs e)
        {
            await App.MViewModel.StartFullCrawlAsync();
            await App.MViewModel.SyncAsync();
        }

        private async void OpenSettings_Click(object sender, RoutedEventArgs e)
            => _ = await SDialog.ShowAsync(ExistingDialogOptions.Enqueue);

        private void HideItem_Click(object sender, RoutedEventArgs e)
            => SBViewModel.ChangeItemVisibility(RightClickedItem.Tag.ToString(), false);

        private void HideSection_Click(object sender, RoutedEventArgs e)
        {
            NavViewItemViewModel item = SBViewModel.
                ItemFromTag(RightClickedItem.Tag.ToString());

            SBViewModel.HideGroup(item.HeaderGroup);
        }

        private void MoveUp_Click(object sender, RoutedEventArgs e)
            => SBViewModel.MoveUp(RightClickedItem.Tag.ToString());

        private void MoveDown_Click(object sender, RoutedEventArgs e)
            => SBViewModel.MoveDown(RightClickedItem.Tag.ToString());

        private void ToTop_Click(object sender, RoutedEventArgs e)
            => SBViewModel.MoveToTop(RightClickedItem.Tag.ToString());

        private void ToBottom_Click(object sender, RoutedEventArgs e)
            => SBViewModel.MoveToBottom(RightClickedItem.Tag.ToString());

        private void NavView_RightTapped(object sender, RightTappedRoutedEventArgs e)
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
                    bool up = SBViewModel.CanMoveUp(tag);
                    bool down = SBViewModel.CanMoveDown(tag);

                    TopOption.IsEnabled = up;
                    UpOption.IsEnabled = up;

                    DownOption.IsEnabled = down;
                    BottomOption.IsEnabled = down;

                    NavViewItemFlyout.ShowAt(NavView, e.GetPosition(NavView));
                }
            }
        }

        private void AppTitleBar_Loaded(object sender, RoutedEventArgs e)
        {

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

            await dialog.ShowAsync();
        }
        
        private async void Support_Click(object sender, RoutedEventArgs e)
            => _ = await URLs.Support.LaunchAsync();

        private async void BigSearch_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            SearchItemViewModel searchItem = args.SelectedItem as SearchItemViewModel;
            sender.Text = searchItem.Title;
            sender.IsSuggestionListOpen = false;

            switch (searchItem.ItemType)
            {
                case "Album":
                    AlbumViewModel album = App.MViewModel.Albums.FirstOrDefault(a => a.Title.Equals(searchItem.Title));
                    ContentFrame.Navigate(typeof(AlbumSongsPage), album);
                    break;
                case "Song":
                    SongViewModel song = App.MViewModel.Songs.FirstOrDefault(s => s.Title.Equals(searchItem.Title));
                    await EventsLogic.StartMusicPlaybackAsync(App.MViewModel.Songs.IndexOf(song), false);
                    break;
                case "Artist":
                    ArtistViewModel artist = App.MViewModel.Artists.FirstOrDefault(a => a.Name.Equals(searchItem.Title));
                    ContentFrame.Navigate(typeof(ArtistSongsPage), artist);
                    break;
            }
        }

        private void BigSearch_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                List<SearchItemViewModel> suitableItems = new List<SearchItemViewModel>();
                List<ArtistViewModel> suitableArtists = new List<ArtistViewModel>();
                List<SongViewModel> suitableSongs = new List<SongViewModel>();
                List<AlbumViewModel> suitableAlbums = new List<AlbumViewModel>();

                int maxCount = 4;

                string[] splitText = sender.Text.ToLower().Split(" ");

                foreach (AlbumViewModel album in Albums)
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

                foreach (SongViewModel song in Songs)
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

                foreach (ArtistViewModel artist in Artists)
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

        private void BigSearch_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            ContentFrame.Navigate(typeof(SearchResultsPage), sender.Text);
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
    }

    [ContentProperty(Name = "GlyphTemplate")]
    public class MenuItemTemplateSelector : DataTemplateSelector
    {
        public DataTemplate GlyphTemplate { get; set; }
        public DataTemplate ImageTemplate { get; set; }

        public DataTemplate HeaderTemplate { get; set; }
        public DataTemplate SeparatorTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item)
        {
            NavViewItemViewModel itemData = item as NavViewItemViewModel;
            if (itemData.Tag == "Header")
            {
                return HeaderTemplate;
            }

            if (itemData.Tag == "Separator")
            {
                return SeparatorTemplate;
            }

            if (itemData.Icon.IsValidUri(UriKind.Absolute))
            {
                return ImageTemplate;
            }
            else
            {
                return GlyphTemplate;
            }
        }
    }

    public class Crumb
    {
        public string Title { get; set; }

        public override string ToString()
        {
            return Title;
        }
    }

    

}
