using Rise.App.Common;
using Rise.App.Dialogs;
using Rise.App.Settings;
using Rise.App.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Markup;
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

        private NavigationViewItem RightClickedItem { get; set; }
        private int _viewId = -1;
        #endregion

        public MainPage()
        {
            InitializeComponent();
            Current = this;
            SDialog.Content = new SettingsPage();

            Loaded += MainPage_Loaded;

            NavigationCacheMode = NavigationCacheMode.Required;
            SuspensionManager.RegisterFrame(ContentFrame, "NavViewFrame");

            App.Indexer.Started += Indexer_Started;
            App.Indexer.Finished += Indexer_Finished;
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

            PlayerElement.SetMediaPlayer(App.PViewModel.Player);
            _ = new ApplicationTitleBar(AppTitleBar, CoreTitleBar_LayoutMetricsChanged);

            Loaded -= MainPage_Loaded;

            // When the page is loaded, initialize the titlebar and setup the player.
            Loaded += (s, e) =>
            {
                PlayerElement.SetMediaPlayer(App.PViewModel.Player);
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
            string navTo = args.InvokedItemContainer.Tag.ToString();
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
                case "HomePage":
                    _ = ContentFrame.Navigate(typeof(HomePage));
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

                    _ = await dialog.ShowAsync(ExistingDialogOptions.CloseExisting);
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

                case "VideoPlaybackPage":
                    if (Window.Current.Content is Frame rootFrame)
                    {
                        _ = rootFrame.Navigate(typeof(VideoPlaybackPage));
                    }
                    break;

                case "DiscyPage":
                    // _ = ContentFrame.Navigate(typeof(DiscyPage));
                    dialog = new UnavailableDialog
                    {
                        Header = "Help & Tips are not available yet.",
                        Description = "Hopefully you'll find this section helpful!",
                        CenterHero = new BitmapImage(new Uri("ms-appx:///Assets/NavigationView/DiscyPage/Colorful.png")),
                    };

                    _ = await dialog.ShowAsync(ExistingDialogOptions.CloseExisting);
                    break;

                case "SettingsPage":
                    _ = await SDialog.ShowAsync(ExistingDialogOptions.CloseExisting);
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

            switch (tag)
            {
                case "HomePage":
                    NavView.SelectedItem = SBViewModel.
                        Items.First(i => i.Tag == tag);
                    return;

                case "AlbumSongsPage":
                    NavView.SelectedItem = SBViewModel.
                        Items.First(i => i.Tag == "AlbumsPage");
                    return;

                case "ArtistSongsPage":
                    NavView.SelectedItem = SBViewModel.
                        Items.First(i => i.Tag == "ArtistsPage");
                    return;

                case "GenreSongsPage":
                    NavView.SelectedItem = SBViewModel.
                        Items.First(i => i.Tag == "GenresPage");
                    return;

                default:
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
        #endregion

        private async void Button_Click(object sender, RoutedEventArgs e)
            => _ = await URLs.Feedback.LaunchAsync();

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

        private void ContentFrame_Navigated(object sender, NavigationEventArgs e)
            => FinishNavigation();

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

        private async void KeyboardAccelerator_Invoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
            => _ = await SDialog.ShowAsync(ExistingDialogOptions.Enqueue);
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
