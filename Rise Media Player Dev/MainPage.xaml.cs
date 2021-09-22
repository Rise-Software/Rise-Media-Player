using Microsoft.UI.Xaml.Controls;
using RMP.App.Converters;
using RMP.App.Dialogs;
using RMP.App.Settings;
using RMP.App.Views;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.WindowManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Media.Imaging;
using NavigationViewItem = Microsoft.UI.Xaml.Controls.NavigationViewItem;
using NavigationViewItemBase = Microsoft.UI.Xaml.Controls.NavigationViewItemBase;

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

        private MainTitleBar MainTitleBarHandle { get; set; }
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
            InitializeComponent();
            Current = this;

            MainTitleBarHandle = new MainTitleBar();
            MainTitleBarHandle.InitTitleBar();

            ApplyStartupSettings();
        }

        // Update the TitleBar content layout depending on NavigationView DisplayMode
        private void NavigationViewControl_DisplayModeChanged(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewDisplayModeChangedEventArgs args)
        {
            MainTitleBarHandle.UpdateTitleBarItems(sender);
        }

        #region Navigation
        private async void NavView_ItemInvoked(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewItemInvokedEventArgs args)
        {
            string navTo = args.InvokedItemContainer.Tag.ToString();
            if (args.IsSettingsInvoked || navTo == "SettingsPage")
            {
                _ = await Dialog.ShowAsync();
                FinishNavigation();
                return;
            }

            if (args.InvokedItemContainer.Content.ToString() == Breadcrumbs.Last())
            {
                FinishNavigation();
                return;
            }

            if (navTo != null)
            {
                switch (navTo)
                {
                    case "AlbumsPage":
                        _ = ContentFrame.Navigate(typeof(AlbumsPage));
                        break;

                    case "ArtistsPage":
                        _ = ContentFrame.Navigate(typeof(ArtistsPage));
                        break;

                    case "DevicesPage":
                        _ = ContentFrame.Navigate(typeof(DevicesPage));
                        break;

                    case "DiscyPage":
                        _ = ContentFrame.Navigate(typeof(DiscyPage));
                        break;

                    case "GenresPage":
                        _ = ContentFrame.Navigate(typeof(GenresPage));
                        break;

                    case "HomePage":
                        _ = ContentFrame.Navigate(typeof(HomePage));
                        break;

                    case "LocalVideosPage":
                        _ = ContentFrame.Navigate(typeof(LocalVideosPage));
                        break;

                    case "NowPlayingPage":
                        _ = ContentFrame.Navigate(typeof(NowPlayingPage));
                        break;

                    case "PlaylistsPage":
                        _ = ContentFrame.Navigate(typeof(PlaylistsPage));
                        break;

                    case "SongsPage":
                        _ = ContentFrame.Navigate(typeof(SongsPage));
                        break;

                    case "StreamingPage":
                        _ = ContentFrame.Navigate(typeof(StreamingPage));
                        break;

                    default:
                        break;
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
        public void ApplyStartupSettings()
        {
            // Sidebar icon colors
            UpdateIconColor(NavigationSettings.ColorfulIcons);

            // Startup setting
            switch (AppearanceSettings.OpenTo)
            {
                case 0:
                    _ = ContentFrame.Navigate(typeof(HomePage));
                    break;

                case 1:
                    _ = ContentFrame.Navigate(typeof(PlaylistsPage));
                    break;

                case 2:
                    _ = ContentFrame.Navigate(typeof(DevicesPage));
                    break;

                case 3:
                    _ = ContentFrame.Navigate(typeof(SongsPage));
                    break;

                case 4:
                    _ = ContentFrame.Navigate(typeof(ArtistsPage));
                    break;

                case 5:
                    _ = ContentFrame.Navigate(typeof(AlbumsPage));
                    break;

                case 6:
                    _ = ContentFrame.Navigate(typeof(GenresPage));
                    break;

                case 7:
                    _ = ContentFrame.Navigate(typeof(LocalVideosPage));
                    break;

                case 8:
                    _ = ContentFrame.Navigate(typeof(StreamingPage));
                    break;

                case 9:
                    _ = ContentFrame.Navigate(typeof(NowPlayingPage));
                    break;

                default:
                    _ = ContentFrame.Navigate(typeof(HomePage));
                    break;
            }

            FinishNavigation();
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