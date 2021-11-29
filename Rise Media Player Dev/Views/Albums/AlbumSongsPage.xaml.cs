using Microsoft.Toolkit.Uwp.UI;
using Rise.App.Common;
using Rise.App.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

namespace Rise.App.Views
{
    /// <summary>
    /// Contains the songs inside an album.
    /// </summary>
    public sealed partial class AlbumSongsPage : Page
    {
        #region Variables
        /// <summary>
        /// Gets the app-wide MViewModel instance.
        /// </summary>
        public MainViewModel MViewModel => App.MViewModel;

        /// <summary>
        /// Gets the app-wide PViewModel instance.
        /// </summary>
        private PlaybackViewModel PViewModel => App.PViewModel;

        /// <summary>
        /// Gets the <see cref="NavigationHelper"/> associated with this <see cref="Page"/>.
        /// </summary>
        private readonly NavigationHelper _navigationHelper;

        private ArtistViewModel Artist;
        private static readonly DependencyProperty SelectedAlbumProperty =
            DependencyProperty.Register("SelectedAlbum", typeof(AlbumViewModel), typeof(AlbumSongsPage), null);

        private AlbumViewModel SelectedAlbum
        {
            get => (AlbumViewModel)GetValue(SelectedAlbumProperty);
            set => SetValue(SelectedAlbumProperty, value);
        }

        private static readonly DependencyProperty SelectedSongProperty =
            DependencyProperty.Register("SelectedSong", typeof(SongViewModel), typeof(AlbumSongsPage), null);

        private SongViewModel SelectedSong
        {
            get => (SongViewModel)GetValue(SelectedSongProperty);
            set => SetValue(SelectedSongProperty, value);
        }

        private AdvancedCollectionView Songs => MViewModel.FilteredSongs;
        // private AdvancedCollectionView Albums => MViewModel.FilteredAlbums;
        #endregion

        public AlbumSongsPage()
        {
            InitializeComponent();

            DataContext = this;
            NavigationCacheMode = NavigationCacheMode.Enabled;

            _navigationHelper = new NavigationHelper(this);
            _navigationHelper.LoadState += NavigationHelper_LoadState;
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
            if (e.NavigationParameter is AlbumViewModel album)
            {
                SelectedAlbum = album;
                Songs.Filter = s => ((SongViewModel)s).Album == album.Title;

                Songs.SortDescriptions.Clear();
                Songs.SortDescriptions.Add(new SortDescription("Disc", SortDirection.Ascending));
                Songs.SortDescriptions.Add(new SortDescription("Track", SortDirection.Ascending));

                // TODO: Get "more album from this artist" to work.
                /*Albums.Filter = a => ((AlbumViewModel)a).Artist == album.Artist;
                Albums.SortDescriptions.Clear();
                Albums.SortDescriptions.Add(new SortDescription("Year", SortDirection.Ascending));*/
            }
        }

        #region Event handlers
        private async void MainList_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            if ((e.OriginalSource as FrameworkElement).DataContext is SongViewModel song)
            {
                int index = MainList.Items.IndexOf(song);
                await StartPlaybackAsync(index);
            }
        }

        private void MainList_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            if ((e.OriginalSource as FrameworkElement).DataContext is SongViewModel song)
            {
                SelectedSong = song;
                SongFlyout.ShowAt(MainList, e.GetPosition(MainList));
            }
        }

        private async void Props_Click(object sender, RoutedEventArgs e)
            => await SelectedSong.StartEdit();

        private void ShowArtist_Click(object sender, RoutedEventArgs e)
        {
            _ = Frame.Navigate(typeof(ArtistSongsPage),
                App.MViewModel.Artists.FirstOrDefault(a => a.Name == SelectedSong.Artist));

            SelectedSong = null;
        }

        private async void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            if ((e.OriginalSource as FrameworkElement).DataContext is SongViewModel song)
            {
                int index = MainList.Items.IndexOf(song);
                await StartPlaybackAsync(index);
                return;
            }

            await StartPlaybackAsync();
        }

        private async void ShuffleButton_Click(object sender, RoutedEventArgs e)
            => await StartPlaybackAsync(0, true);

        private async Task StartPlaybackAsync(int index = 0, bool shuffle = false)
        {
            if (SelectedSong != null && index == 0)
            {
                index = MainList.Items.IndexOf(SelectedSong);
                SelectedSong = null;
            }

            IEnumerator<object> enumerator = Songs.GetEnumerator();
            List<SongViewModel> songs = new List<SongViewModel>();

            while (enumerator.MoveNext())
            {
                songs.Add(enumerator.Current as SongViewModel);
            }

            enumerator.Dispose();
            await PViewModel.StartMusicPlaybackAsync(songs.GetEnumerator(), index, songs.Count, shuffle);
        }

        private async void EditButton_Click(object sender, RoutedEventArgs e)
        {
            await SelectedSong.StartEdit();
            SelectedSong = null;
        }

        private void Hyperlink_Click(Hyperlink sender, HyperlinkClickEventArgs args)
        {
            if (Artist == null)
            {
                Artist = App.MViewModel.Artists.
                    FirstOrDefault(a => a.Name == SelectedAlbum.Artist);
            }

            Frame.Navigate(typeof(ArtistSongsPage), Artist);
        }

        private void GridView_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if ((e.OriginalSource as FrameworkElement).DataContext is AlbumViewModel album)
            {
                _ = Frame.Navigate(typeof(AlbumSongsPage), album);
            }
        }

        private void MainGrid_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            if ((e.OriginalSource as FrameworkElement).DataContext is AlbumViewModel album)
            {
                SelectedAlbum = album;
            }
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
            => _navigationHelper.OnNavigatedTo(e);

        protected override void OnNavigatedFrom(NavigationEventArgs e)
            => _navigationHelper.OnNavigatedFrom(e);
        #endregion
    }
}
