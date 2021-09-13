using System;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.ApplicationModel.Core;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using RMP.App.Dialogs;
using RMP.App.Views;
using RMP.App.Converters;
using RMP.App.Settings;
using NavigationViewItem = Microsoft.UI.Xaml.Controls.NavigationViewItem;
using NavigationViewItemBase = Microsoft.UI.Xaml.Controls.NavigationViewItemBase;
using Microsoft.UI.Xaml.Controls;
using System.Diagnostics;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace RMP.App
{
    /// <summary>
    /// Main app page, hosts the NavigationView and ContentFrame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        #region Variables
        public static MainPage Current;
        private readonly ObservableCollection<string> Breadcrumbs =
            new ObservableCollection<string>();

        public SongIndexer Indexer = new SongIndexer();
        public SettingsDialog Dialog = new SettingsDialog();
        #endregion

        #region NavView Icons
        private readonly ImageIcon homeIconColor =
            new ImageIcon() { Source = new BitmapImage(new Uri("ms-appx:///Assets/NavigationView/At a glance.png")) };

        private readonly FontIcon homeIconMono =
            new FontIcon() { Glyph = "\uECA5" };

        private readonly ImageIcon playlistsIconColor =
            new ImageIcon() { Source = new BitmapImage(new Uri("ms-appx:///Assets/NavigationView/Playlists.png")) };

        private readonly FontIcon playlistsIconMono =
            new FontIcon() { Glyph = "\uE142" };

        private readonly ImageIcon devicesIconColor =
            new ImageIcon() { Source = new BitmapImage(new Uri("ms-appx:///Assets/NavigationView/Devices.png")) };

        private readonly FontIcon devicesIconMono =
            new FontIcon() { Glyph = "\uE1C9" };

        private readonly ImageIcon songsIconColor =
            new ImageIcon() { Source = new BitmapImage(new Uri("ms-appx:///Assets/NavigationView/Songs.png")) };

        private readonly FontIcon songsIconMono =
            new FontIcon() { Glyph = "\uEC4F" };

        private readonly ImageIcon artistsIconColor =
            new ImageIcon() { Source = new BitmapImage(new Uri("ms-appx:///Assets/NavigationView/Artists.png")) };

        private readonly FontIcon artistsIconMono =
            new FontIcon() { Glyph = "\uE125" };

        private readonly ImageIcon albumsIconColor =
            new ImageIcon() { Source = new BitmapImage(new Uri("ms-appx:///Assets/NavigationView/Albums.png")) };

        private readonly FontIcon albumsIconMono =
            new FontIcon() { Glyph = "\uE93C" };

        private readonly ImageIcon genresIconColor =
            new ImageIcon() { Source = new BitmapImage(new Uri("ms-appx:///Assets/NavigationView/Genres.png")) };

        private readonly FontIcon genresIconMono =
            new FontIcon() { Glyph = "\uE138" };

        private readonly ImageIcon videosIconColor =
            new ImageIcon() { Source = new BitmapImage(new Uri("ms-appx:///Assets/NavigationView/Local Videos.png")) };

        private readonly FontIcon videosIconMono =
            new FontIcon() { Glyph = "\uE8B7" };

        private readonly ImageIcon streamingIconColor =
            new ImageIcon() { Source = new BitmapImage(new Uri("ms-appx:///Assets/NavigationView/Online Videos.png")) };

        private readonly FontIcon streamingIconMono =
            new FontIcon() { Glyph = "\uE12B" };

        private readonly ImageIcon helpIconColor =
            new ImageIcon() { Source = new BitmapImage(new Uri("ms-appx:///Assets/NavigationView/DiscyHelp.png")) };

        private readonly FontIcon helpIconMono =
            new FontIcon() { Glyph = "\uE897" };

        private readonly ImageIcon playingIconColor =
            new ImageIcon() { Source = new BitmapImage(new Uri("ms-appx:///Assets/NavigationView/Now Playing.png")) };

        private readonly FontIcon playingIconMono =
            new FontIcon() { Glyph = "\uE768" };
        #endregion

        public MainPage()
        {
            this.InitializeComponent();
            Current = this;

            ApplyStartupSettings();

            #region Titlebar
            ApplicationViewTitleBar titleBar = ApplicationView.GetForCurrentView().TitleBar;

            titleBar.ButtonBackgroundColor = Colors.Transparent;
            titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;

            // Hide default title bar.
            CoreApplicationViewTitleBar coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
            coreTitleBar.ExtendViewIntoTitleBar = true;
            UpdateTitleBarLayout(coreTitleBar);

            // Set XAML element as a draggable region.
            Window.Current.SetTitleBar(AppTitleBar);

            // Register a handler for when the size of the overlaid caption control changes.
            // For example, when the app moves to a screen with a different DPI.
            coreTitleBar.LayoutMetricsChanged += CoreTitleBar_LayoutMetricsChanged;

            // Register a handler for when the title bar visibility changes.
            // For example, when the title bar is invoked in full screen mode.
            coreTitleBar.IsVisibleChanged += CoreTitleBar_IsVisibleChanged;

            //Register a handler for when the window changes focus
            Window.Current.Activated += Current_Activated;
            #endregion
        }

        #region Titlebar
        private void CoreTitleBar_LayoutMetricsChanged(CoreApplicationViewTitleBar sender, object args)
        {
            UpdateTitleBarLayout(sender);
        }

        private void UpdateTitleBarLayout(CoreApplicationViewTitleBar coreTitleBar)
        {
            // Update title bar control size as needed to account for system size changes.
            AppTitleBar.Height = coreTitleBar.Height;

            // Ensure the custom title bar does not overlap window caption controls
            Thickness currMargin = AppTitleBar.Margin;
            AppTitleBar.Margin = new Thickness(currMargin.Left, currMargin.Top, coreTitleBar.SystemOverlayRightInset, currMargin.Bottom);
            SearchBar.Margin = new Thickness(currMargin.Left + AppData.DesiredSize.Width + 96, currMargin.Top, coreTitleBar.SystemOverlayRightInset + 32, currMargin.Bottom);

            UpdateTitleBarItems(NavView);
        }

        private void CoreTitleBar_IsVisibleChanged(CoreApplicationViewTitleBar sender, object args)
        {
            if (sender.IsVisible)
            {
                AppTitleBar.Visibility = Visibility.Visible;
            }
            else
            {
                AppTitleBar.Visibility = Visibility.Collapsed;
            }
        }

        // Update the TitleBar based on the inactive/active state of the app
        private void Current_Activated(object sender, Windows.UI.Core.WindowActivatedEventArgs e)
        {
            SolidColorBrush defaultForegroundBrush = (SolidColorBrush)Application.Current.Resources["TextFillColorPrimaryBrush"];
            SolidColorBrush inactiveForegroundBrush = (SolidColorBrush)Application.Current.Resources["TextFillColorDisabledBrush"];

            if (e.WindowActivationState == Windows.UI.Core.CoreWindowActivationState.Deactivated)
            {
                AppTitle.Foreground = inactiveForegroundBrush;
            }
            else
            {
                AppTitle.Foreground = defaultForegroundBrush;
            }
        }

        // Update the TitleBar content layout depending on NavigationView DisplayMode
        private void NavigationViewControl_DisplayModeChanged(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewDisplayModeChangedEventArgs args)
        {
            UpdateTitleBarItems(sender);
        }

        private void UpdateTitleBarItems(Microsoft.UI.Xaml.Controls.NavigationView NavView)
        {
            const int topIndent = 16;
            const int expandedIndent = 48;
            int minimalIndent = 104;

            Thickness currMargin = AppTitleBar.Margin;

            // Set the TitleBar margin dependent on NavigationView display mode
            if (NavView.PaneDisplayMode == Microsoft.UI.Xaml.Controls.NavigationViewPaneDisplayMode.Top)
            {
                AppTitleBar.Margin = new Thickness(topIndent, currMargin.Top, currMargin.Right, currMargin.Bottom);
                SearchBar.Margin = new Thickness(topIndent + AppData.DesiredSize.Width + 48, currMargin.Top, currMargin.Right, currMargin.Bottom);
            }
            else if (NavView.DisplayMode == Microsoft.UI.Xaml.Controls.NavigationViewDisplayMode.Minimal)
            {
                AppTitleBar.Margin = new Thickness(minimalIndent, currMargin.Top, currMargin.Right, currMargin.Bottom);
                SearchBar.Margin = new Thickness(minimalIndent + 36, currMargin.Top, currMargin.Right - 40, currMargin.Bottom);
            }
            else
            {
                AppTitleBar.Margin = new Thickness(expandedIndent, currMargin.Top, currMargin.Right, currMargin.Bottom);
                SearchBar.Margin = new Thickness(expandedIndent + AppData.DesiredSize.Width + 132, currMargin.Top, expandedIndent + AppData.DesiredSize.Width + 132, currMargin.Bottom);
            }
        }
        #endregion

        #region Navigation
        private async void NavView_ItemInvoked(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewItemInvokedEventArgs args)
        {
            if (args.InvokedItemContainer.Content.ToString() == Breadcrumbs.Last())
            {
                FinishNavigation();
                return;
            }

            string navTo = args.InvokedItemContainer.Tag.ToString();
            if (args.IsSettingsInvoked || navTo == "SettingsPage")
            {
                await Dialog.ShowAsync();
                return;
            }
            else
            {
                if (navTo != null)
                {
                    switch (navTo)
                    {
                        case "AlbumsPage":
                            ContentFrame.Navigate(typeof(AlbumsPage));
                            break;

                        case "ArtistsPage":
                            ContentFrame.Navigate(typeof(ArtistsPage));
                            break;

                        case "DevicesPage":
                            ContentFrame.Navigate(typeof(DevicesPage));
                            break;

                        case "DiscyPage":
                            ContentFrame.Navigate(typeof(DiscyPage));
                            break;

                        case "GenresPage":
                            ContentFrame.Navigate(typeof(GenresPage));
                            break;

                        case "HomePage":
                            ContentFrame.Navigate(typeof(HomePage));
                            break;

                        case "LocalVideosPage":
                            ContentFrame.Navigate(typeof(LocalVideosPage));
                            break;

                        case "NowPlayingPage":
                            ContentFrame.Navigate(typeof(NowPlayingPage));
                            break;

                        case "PlaylistsPage":
                            ContentFrame.Navigate(typeof(PlaylistsPage));
                            break;

                        case "SongsPage":
                            ContentFrame.Navigate(typeof(SongsPage));
                            break;

                        case "StreamingPage":
                            ContentFrame.Navigate(typeof(StreamingPage));
                            break;

                        default:
                            break;
                    }

                }
            }
            FinishNavigation();
        }

        private void NavView_BackRequested(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewBackRequestedEventArgs args)
        {
            ContentFrame.GoBack();
            FinishNavigation();
        }

        public void FinishNavigation()
        {
            string type = ContentFrame.CurrentSourcePageType.ToString();
            string tag = type.Split('.').Last();

            foreach (NavigationViewItemBase item in NavView.MenuItems)
            {
                if (item is NavigationViewItem && item.Tag.ToString() == tag)
                {
                    NavView.SelectedItem = item;
                    Breadcrumbs.Clear();
                    Breadcrumbs.Add(item.Content.ToString());
                    return;
                }
            }

            foreach (NavigationViewItemBase item in NavView.FooterMenuItems)
            {
                if (item is NavigationViewItem && item.Tag.ToString() == tag)
                {
                    NavView.SelectedItem = item;
                    Breadcrumbs.Clear();
                    Breadcrumbs.Add(item.Content.ToString());
                    return;
                }
            }
        }
        #endregion

        #region Settings
        public async void ApplyStartupSettings()
        {
            // Sidebar icon colors
            UpdateIconColor(NavigationSettings.ColorfulIcons);

            // Startup setting
            switch (AppearanceSettings.OpenTo)
            {
                case 0:
                    ContentFrame.Navigate(typeof(HomePage));
                    break;

                case 1:
                    ContentFrame.Navigate(typeof(PlaylistsPage));
                    break;

                case 2:
                    ContentFrame.Navigate(typeof(DevicesPage));
                    break;

                case 3:
                    ContentFrame.Navigate(typeof(SongsPage));
                    break;

                case 4:
                    ContentFrame.Navigate(typeof(ArtistsPage));
                    break;

                case 5:
                    ContentFrame.Navigate(typeof(AlbumsPage));
                    break;

                case 6:
                    ContentFrame.Navigate(typeof(GenresPage));
                    break;

                case 7:
                    ContentFrame.Navigate(typeof(LocalVideosPage));
                    break;

                case 8:
                    ContentFrame.Navigate(typeof(StreamingPage));
                    break;

                case 9:
                    ContentFrame.Navigate(typeof(NowPlayingPage));
                    break;

                default:
                    ContentFrame.Navigate(typeof(HomePage));
                    break;
            }

            FinishNavigation();

            await Indexer.IndexLibrarySongs();
        }

        public void UpdateSidebarItems(bool newVis, string itemName)
        {
            Visibility visibility = BindlessBooleanToVisibility.Convert(newVis);
            switch (itemName)
            {
                case "Home":
                    HomePageItem.Visibility = visibility;
                    break;

                case "Playlists":
                    PlaylistsPageItem.Visibility = visibility;
                    break;

                case "Devices":
                    DevicesPageItem.Visibility = visibility;
                    break;

                case "Songs":
                    SongsPageItem.Visibility = visibility;
                    break;

                case "Artists":
                    ArtistsPageItem.Visibility = visibility;
                    break;

                case "Albums":
                    AlbumsPageItem.Visibility = visibility;
                    break;

                case "Genres":
                    GenresPageItem.Visibility = visibility;
                    break;

                case "LocalVideos":
                    LocalVideosPageItem.Visibility = visibility;
                    break;

                case "Streaming":
                    StreamingPageItem.Visibility = visibility;
                    break;

                default:
                    break;
            }
        }

        public void UpdateIconColor(bool coloredIcons)
        {
            if (coloredIcons)
            {
                SettingsPageItem.Visibility = Visibility.Visible;
                NavView.IsSettingsVisible = false;

                HomePageItem.Icon = homeIconColor;
                PlaylistsPageItem.Icon = playlistsIconColor;
                DevicesPageItem.Icon = devicesIconColor;
                SongsPageItem.Icon = songsIconColor;
                ArtistsPageItem.Icon = artistsIconColor;
                AlbumsPageItem.Icon = albumsIconColor;
                GenresPageItem.Icon = genresIconColor;
                LocalVideosPageItem.Icon = videosIconColor;
                StreamingPageItem.Icon = streamingIconColor;
                DiscyPageItem.Icon = helpIconColor;
                NowPlayingPageItem.Icon = playingIconColor;
                return;
            }

            SettingsPageItem.Visibility = Visibility.Collapsed;
            NavView.IsSettingsVisible = true;

            HomePageItem.Icon = homeIconMono;
            PlaylistsPageItem.Icon = playlistsIconMono;
            DevicesPageItem.Icon = devicesIconMono;
            SongsPageItem.Icon = songsIconMono;
            ArtistsPageItem.Icon = artistsIconMono;
            AlbumsPageItem.Icon = albumsIconMono;
            GenresPageItem.Icon = genresIconMono;
            LocalVideosPageItem.Icon = videosIconMono;
            StreamingPageItem.Icon = streamingIconMono;
            DiscyPageItem.Icon = helpIconMono;
            NowPlayingPageItem.Icon = playingIconMono;
        }
        #endregion
    }
}