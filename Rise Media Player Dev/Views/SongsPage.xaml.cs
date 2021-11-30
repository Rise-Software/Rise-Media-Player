using Microsoft.Toolkit.Uwp.UI;
using Rise.App.Common;
using Rise.App.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

namespace Rise.App.Views
{
    public sealed partial class SongsPage : Page
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
        /// Gets the <see cref="NavigationHelper"/> associated with this <see cref="Page"/>.
        /// </summary>
        private readonly NavigationHelper _navigationHelper;

        private SongViewModel SelectedSong
        {
            get => MViewModel.SelectedSong;
            set => MViewModel.SelectedSong = value;
        }

        private AdvancedCollectionView Songs => MViewModel.FilteredSongs;

        private string SortProperty = "Title";
        private SortDirection CurrentSort = SortDirection.Ascending;
        #endregion

        public SongsPage()
        {
            InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;

            _navigationHelper = new NavigationHelper(this);
            _navigationHelper.LoadState += NavigationHelper_LoadState;
        }

        private void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            Songs.Filter = null;
            Songs.SortDescriptions.Clear();
            Songs.SortDescriptions.Add(new SortDescription(SortProperty, CurrentSort));
            Songs.Refresh();
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
            => _ = Frame.Navigate(typeof(ArtistSongsPage), SelectedSong.Artist);

        private void ShowAlbum_Click(object sender, RoutedEventArgs e)
            => _ = Frame.Navigate(typeof(AlbumSongsPage), SelectedSong.Album);

        private async void PlayButton_Click(object sender, RoutedEventArgs e)
            => await StartPlaybackAsync();

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

        private void SortFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            MenuFlyoutItem item = sender as MenuFlyoutItem;
            Songs.SortDescriptions.Clear();

            string tag = item.Tag.ToString();
            switch (tag)
            {
                case "Ascending":
                    CurrentSort = SortDirection.Ascending;
                    break;

                case "Descending":
                    CurrentSort = SortDirection.Descending;
                    break;

                case "Track":
                    Songs.SortDescriptions.
                        Add(new SortDescription("Disc", CurrentSort));
                    SortProperty = tag;
                    break;

                default:
                    SortProperty = tag;
                    break;
            }

            Songs.SortDescriptions.
                Add(new SortDescription(SortProperty, CurrentSort));
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
