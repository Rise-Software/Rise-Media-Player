using Microsoft.Toolkit.Uwp.UI;
using RMP.App.Common;
using RMP.App.Settings.ViewModels;
using RMP.App.ViewModels;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace RMP.App.Views
{
    public sealed partial class AlbumsPage : Page
    {
        #region Variables
        /// <summary>
        /// Gets the app-wide MViewModel instance.
        /// </summary>
        private MainViewModel MViewModel => App.MViewModel;

        /// <summary>
        /// Gets the app-wide PViewModel instance.
        /// </summary>
        private PlaybackViewModel PViewModel => App.PViewModel;

        /// <summary>
        /// Gets the app-wide SViewModel instance.
        /// </summary>
        private SettingsViewModel SViewModel => App.SViewModel;

        private readonly NavigationHelper navigationHelper;
        /// <summary>
        /// Gets the <see cref="NavigationHelper"/> associated with this <see cref="Page"/>.
        /// </summary>
        public NavigationHelper NavigationHelper => navigationHelper;

        private static readonly DependencyProperty SelectedAlbumProperty =
            DependencyProperty.Register("SelectedAlbum", typeof(AlbumViewModel), typeof(AlbumsPage), null);

        private AlbumViewModel SelectedAlbum
        {
            get => (AlbumViewModel)GetValue(SelectedAlbumProperty);
            set => SetValue(SelectedAlbumProperty, value);
        }

        private AdvancedCollectionView Albums => MViewModel.FilteredAlbums;
        private AdvancedCollectionView Songs => MViewModel.FilteredSongs;

        private string SortProperty = "Title";
        private SortDirection CurrentSort = SortDirection.Ascending;
        #endregion

        public AlbumsPage()
        {
            InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;

            navigationHelper = new NavigationHelper(this);
            navigationHelper.LoadState += NavigationHelper_LoadState;
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="sender">
        /// The source of the event; typically <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Event data that provides both the navigation parameter passed to
        /// <see cref="Frame.Navigate(Type, object)"/> when this page was initially requested and
        /// a dictionary of state preserved by this page during an earlier
        /// session.  The state will be null the first time a page is visited.</param>
        private void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            Albums.SortDescriptions.Clear();
            Albums.SortDescriptions.Add(new SortDescription(SortProperty, CurrentSort));
        }

        #region Event handlers
        private void GridView_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if ((e.OriginalSource as FrameworkElement).DataContext is AlbumViewModel album)
            {
                _ = Frame.Navigate(typeof(AlbumSongsPage), album);
            }

            SelectedAlbum = null;
        }

        private void MainGrid_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            if ((e.OriginalSource as FrameworkElement).DataContext is AlbumViewModel album)
            {
                SelectedAlbum = album;
                AlbumFlyout.ShowAt(MainGrid, e.GetPosition(MainGrid));
            }
        }

        private void Hyperlink_Click(Hyperlink sender, HyperlinkClickEventArgs args)
        {
            FrameworkElement parent =
                VisualTreeHelper.GetParent(sender) as FrameworkElement;

            if (parent.DataContext is AlbumViewModel album)
            {
                _ = Frame.Navigate(typeof(ArtistSongsPage),
                    App.MViewModel.Artists.FirstOrDefault(a => a.Name == album.Artist));
            }

            SelectedAlbum = null;
        }

        private void ShowArtist_Click(object sender, RoutedEventArgs e)
        {
            _ = Frame.Navigate(typeof(ArtistSongsPage),
                App.MViewModel.Artists.FirstOrDefault(a => a.Name == SelectedAlbum.Artist));

            SelectedAlbum = null;
        }

        private async void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            Songs.Filter = null;
            Songs.SortDescriptions.Clear();

            if (SelectedAlbum != null)
            {
                if (App.SViewModel.FilterByNameOnly)
                {
                    Songs.Filter = s => ((SongViewModel)s).Album == SelectedAlbum.Title;
                }
                else
                {
                    Songs.Filter = s => ((SongViewModel)s).Album == SelectedAlbum.Title
                        && ((SongViewModel)s).AlbumArtist == SelectedAlbum.Artist;
                }
            }
            else
            {
                Songs.SortDescriptions.Add(new SortDescription("Album", CurrentSort));
            }

            Songs.SortDescriptions.Add(new SortDescription("Disc", SortDirection.Ascending));
            Songs.SortDescriptions.Add(new SortDescription("Track", SortDirection.Ascending));

            using (Songs.DeferRefresh())
            {
                await PViewModel.StartPlayback
                    (Songs.GetEnumerator(), 0, Songs.Count);
            }
            Songs.Refresh();
        }

        private async void ShuffleButton_Click(object sender, RoutedEventArgs e)
        {
            Songs.Filter = null;
            if (SelectedAlbum != null)
            {
                if (App.SViewModel.FilterByNameOnly)
                {
                    Songs.Filter = s => ((SongViewModel)s).Album == SelectedAlbum.Title;
                }
                else
                {
                    Songs.Filter = s => ((SongViewModel)s).Album == SelectedAlbum.Title
                        && ((SongViewModel)s).AlbumArtist == SelectedAlbum.Artist;
                }
            }

            using (Songs.DeferRefresh())
            {
                await PViewModel.StartShuffle
                    (Songs.GetEnumerator(), Songs.Count);
            }
            Songs.Refresh();
        }

        private void SelectToggleButton_Checked(object sender, RoutedEventArgs e)
            => MainGrid.Tapped -= GridView_Tapped;

        private void SelectToggleButton_Unchecked(object sender, RoutedEventArgs e)
            => MainGrid.Tapped += GridView_Tapped;

        private void SortFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            MenuFlyoutItem item = sender as MenuFlyoutItem;

            Albums.SortDescriptions.Clear();
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
                    SortProperty = tag;
                    break;
            }

            Albums.SortDescriptions.
                Add(new SortDescription(SortProperty, CurrentSort));
        }

        private void Merge_Click(object sender, RoutedEventArgs e)
        {
            ToggleMenuFlyoutItem item = sender as ToggleMenuFlyoutItem;
            if (item.IsChecked)
            {
                // Do something to filter the albums based on name only.
                return;
            }

            Albums.Filter = null;
        }
        #endregion

        #region NavigationHelper registration
        /// <summary>
        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// Page specific logic should be placed in event handlers for the  
        /// <see cref="NavigationHelper.LoadState"/>
        /// and <see cref="NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method 
        /// in addition to page state preserved during an earlier session.
        /// </summary>
        protected override void OnNavigatedTo(NavigationEventArgs e)
            => navigationHelper.OnNavigatedTo(e);

        protected override void OnNavigatedFrom(NavigationEventArgs e)
            => navigationHelper.OnNavigatedFrom(e);
        #endregion
    }
}
