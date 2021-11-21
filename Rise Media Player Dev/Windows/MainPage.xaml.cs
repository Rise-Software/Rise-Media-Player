using Rise.App.Common;
using Rise.App.Dialogs;
using Rise.App.Settings;
using Rise.App.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
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

        public static MainPage Current;

        public ObservableCollection<Crumb> Breadcrumbs { get; set; }
            = new ObservableCollection<Crumb>();

        private MainTitleBar MainTitleBarHandle { get; set; }

        public SettingsDialogContainer SDialog { get; }
            = new SettingsDialogContainer();

        private IDisposable SongsDefer { get; set; }
        private IDisposable AlbumsDefer { get; set; }
        private IDisposable ArtistsDefer { get; set; }
        private IDisposable GenresDefer { get; set; }
        private IDisposable VideosDefer { get; set; }

        private NavigationViewItem RightClickedItem { get; set; }
        private int _viewId = -1;
        #endregion

        #region Classes
        public class Crumb
        {
            public string Title { get; set; }

            public override string ToString()
            {
                return Title;
            }
        }
        #endregion

        public MainPage()
        {
            InitializeComponent();
            Current = this;

            MainTitleBarHandle = new MainTitleBar();
            Loaded += MainPage_Loaded;

            NavigationCacheMode = NavigationCacheMode.Required;
            SDialog.Content = new SettingsPage();

            SuspensionManager.RegisterFrame(ContentFrame, "NavViewFrame");

            App.Indexer.Started += Indexer_Started;
            App.Indexer.Finished += Indexer_Finished;
        }

        private async void MainPage_Loaded(object sender, RoutedEventArgs e)
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
            PlayerElement.SetMediaPlayer(App.PViewModel.Player);

            App.MViewModel.CanIndex = true;
            _ = Task.Run(async () => await App.MViewModel.StartFullCrawlAsync());

            Loaded -= MainPage_Loaded;
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
            });
        }

        private async void Indexer_Finished(object sender, int e)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                CheckTip.IsOpen = false;
                AddedTip.IsOpen = true;

                SongsDefer.Dispose();
                AlbumsDefer.Dispose();
                ArtistsDefer.Dispose();
                GenresDefer.Dispose();
                VideosDefer.Dispose();

                App.MViewModel.FilteredSongs.Refresh();
                App.MViewModel.FilteredAlbums.Refresh();
                App.MViewModel.FilteredArtists.Refresh();
                App.MViewModel.FilteredGenres.Refresh();
                App.MViewModel.FilteredVideos.Refresh();
            });
        }

        // Update the TitleBar content layout depending on NavigationView DisplayMode
        private void NavigationViewControl_DisplayModeChanged(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewDisplayModeChangedEventArgs args)
            => MainTitleBarHandle.UpdateTitleBarItems(sender);

        #region Navigation
        private async void NavView_ItemInvoked(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewItemInvokedEventArgs args)
        {
            string navTo = args.InvokedItemContainer.Tag.ToString();
            if (args.IsSettingsInvoked || navTo == "SettingsPage")
            {
                _ = await SDialog.ShowAsync();
                FinishNavigation();
                return;
            }

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
            UnavailableDialog dialog;

            switch (navTo)
            {
                case "AlbumsPage":
                    _ = ContentFrame.Navigate(typeof(AlbumsPage));
                    break;

                case "ArtistsPage":
                    _ = ContentFrame.Navigate(typeof(ArtistsPage));
                    break;

                case "DevicesPage":
                    // _ = ContentFrame.Navigate(typeof(DevicesPage));
                    dialog = new UnavailableDialog
                    {
                        Header = "Device view and sync is not available yet.",
                        Description = "This will be coming in a future update.",
                        CenterHero = new BitmapImage(new Uri("ms-appx:///Assets/NavigationView/Devices.png"))
                    };

                    _ = await dialog.ShowAsync();
                    break;

                case "DiscyPage":
                    // _ = ContentFrame.Navigate(typeof(DiscyPage));
                    dialog = new UnavailableDialog
                    {
                        Header = "Help & Tips are not available yet.",
                        Description = "Hopefully you'll find this section helpful!",
                        CenterHero = new BitmapImage(new Uri("ms-appx:///Assets/NavigationView/DiscyPage/Colorful.png")),
                    };

                    _ = await dialog.ShowAsync();
                    break;

                case "GenresPage":
                    // _ = ContentFrame.Navigate(typeof(GenresPage));
                    dialog = new UnavailableDialog
                    {
                        Header = "You can't check out the genres yet.",
                        Description = "Hopefully you can start soon!",
                        CenterHero = new BitmapImage(new Uri("ms-appx:///Assets/NavigationView/GenresPage/Colorful.png"))
                    };

                    _ = await dialog.ShowAsync();
                    break;

                case "HomePage":
                    _ = ContentFrame.Navigate(typeof(HomePage));
                    break;

                case "LocalVideosPage":
                    _ = ContentFrame.Navigate(typeof(LocalVideosPage));
                    break;

                case "NowPlayingPage":
                    if (_viewId == -1)
                    {
                        _viewId = await typeof(NowPlaying).
                            OpenInWindowAsync(ApplicationViewMode.Default, 320, 300);
                    }
                    else
                    {
                        await ApplicationViewSwitcher.TryShowAsStandaloneAsync(_viewId);
                    }
                    break;

                case "PlaylistsPage":
                    // _ = ContentFrame.Navigate(typeof(PlaylistsPage));
                    dialog = new UnavailableDialog
                    {
                        Header = "Unfortunately, playlists aren't available yet. Go to your music library instead.",
                        Description = "Hopefully we won't be long adding them!",
                        CenterHero = new BitmapImage(new Uri("ms-appx:///Assets/Unavailable/Playlists.png"))
                    };

                    _ = await dialog.ShowAsync();
                    break;

                case "SongsPage":
                    _ = ContentFrame.Navigate(typeof(SongsPage));
                    break;

                case "StreamingPage":
                    // _ = ContentFrame.Navigate(typeof(StreamingPage));
                    dialog = new UnavailableDialog
                    {
                        Header = "Streaming services for videos, films and TV are not available yet.",
                        Description = "We are prioritising other features first, including local playback.",
                        LeftHero = new BitmapImage(new Uri("ms-appx:///Assets/Unavailable/Netflix.png")),
                        CenterHero = new BitmapImage(new Uri("ms-appx:///Assets/Unavailable/Prime.png")),
                        RightHero = new BitmapImage(new Uri("ms-appx:///Assets/Unavailable/YouTube.png"))
                    };

                    _ = await dialog.ShowAsync();
                    break;

                default:
                    break;
            }
        }

        public void FinishNavigation()
        {
            string type = ContentFrame.CurrentSourcePageType.ToString();
            string tag = type.Split('.').Last();

            Breadcrumbs.Clear();
            if (tag == "AlbumSongsPage" || tag == "ArtistSongsPage")
            {
                Breadcrumbs.Add(new Crumb
                {
                    Title = ""
                });
                return;
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
        #endregion

        private async void Button_Click(object sender, RoutedEventArgs e)
            => _ = await URLs.Feedback.LaunchAsync();

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

        private async void Button_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            await App.MViewModel.StartFullCrawlAsync();
            await App.MViewModel.SyncAsync();
        }

        private void ContentFrame_Navigated(object sender, NavigationEventArgs e)
            => FinishNavigation();

        private void NavView_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            DependencyObject source = e.OriginalSource as DependencyObject;

            if (source.FindVisualParent<NavigationViewItem>()
                is NavigationViewItem item && !item.Tag.ToString().Equals("SettingsPage"))
            {
                RightClickedItem = item;
                NavViewItemFlyout.ShowAt(NavView, e.GetPosition(NavView));
            }
        }
    }

    [ContentProperty(Name = "GlyphTemplate")]
    public class MenuItemTemplateSelector : DataTemplateSelector
    {
        public DataTemplate GlyphTemplate { get; set; }
        public DataTemplate ImageTemplate { get; set; }

        public DataTemplate HeaderTemplate { get; set; }
        // public DataTemplate SeparatorTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item)
        {
            NavViewItemViewModel itemData = item as NavViewItemViewModel;
            if (itemData.Tag == "Header")
            {
                return HeaderTemplate;
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
}
