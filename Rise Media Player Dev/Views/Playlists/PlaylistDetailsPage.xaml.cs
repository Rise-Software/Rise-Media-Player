using Microsoft.Toolkit.Uwp.UI.Animations;
using Rise.App.ViewModels;
using Rise.Common.Helpers;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

namespace Rise.App.Views
{
    // Constructor, Lifecycle management
    public sealed partial class PlaylistDetailsPage : Page
    {
        /// <summary>
        /// Gets the <see cref="NavigationHelper"/> associated with this <see cref="Page"/>.
        /// </summary>
        private readonly NavigationHelper _navigationHelper;

        public PlaylistDetailsPage()
        {
            InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;
            Loaded += PlaylistDetailsPage_Loaded;

            _navigationHelper = new NavigationHelper(this);
            _navigationHelper.LoadState += NavigationHelper_LoadState;
            _navigationHelper.SaveState += NavigationHelper_SaveState;
        }

        private void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            if (e.NavigationParameter is Guid id)
            {
                SelectedPlaylist = App.MViewModel.Playlists.
                    FirstOrDefault(p => p.Model.Id == id);
            }
        }

        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
            Frame.SetListDataItemForNextConnectedAnimation(SelectedPlaylist);
        }

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

    // Fields, properties
    public sealed partial class PlaylistDetailsPage : Page
    {
        private SongViewModel _song;

        private static readonly DependencyProperty SelectedSongProperty =
            DependencyProperty.Register("SelectedSong", typeof(SongViewModel), typeof(PlaylistDetailsPage), null);

        private SongViewModel SelectedSong
        {
            get => (SongViewModel)GetValue(SelectedSongProperty);
            set => SetValue(SelectedSongProperty, value);
        }

        private static readonly DependencyProperty SelectedPlaylistProperty =
                DependencyProperty.Register("SelectedPlaylist", typeof(PlaylistViewModel), typeof(PlaylistDetailsPage), null);

        private PlaylistViewModel SelectedPlaylist
        {
            get => (PlaylistViewModel)GetValue(SelectedPlaylistProperty);
            set => SetValue(SelectedPlaylistProperty, value);
        }
    }

    // Event handlers
    public sealed partial class PlaylistDetailsPage : Page
    {
        private async void PlaylistDetailsPage_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                await Task.Run(async () =>
                {
                    foreach (SongViewModel song in SelectedPlaylist.Songs)
                    {
                        if (!File.Exists(song.Location))
                        {
                            await SelectedPlaylist.RemoveSongAsync(song);
                        }
                    }
                });
            }
            catch (Exception)
            {

            }
        }

        private void Grid_PointerEntered(object sender, PointerRoutedEventArgs e)
            => EventsLogic.FocusSong(ref _song, e);

        private void Grid_PointerExited(object sender, PointerRoutedEventArgs e)
            => EventsLogic.UnfocusSong(ref _song, e);

        private void MainList_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            if ((e.OriginalSource as FrameworkElement).DataContext is SongViewModel song)
            {
                SelectedSong = song;
                SongFlyout.ShowAt(MainList, e.GetPosition(MainList));
            }
        }

        private async void MainList_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            if ((e.OriginalSource as FrameworkElement).DataContext is SongViewModel song)
            {
                int index = MainList.Items.IndexOf(song);
                await App.PViewModel.StartMusicPlaybackAsync(SelectedPlaylist.Songs.GetEnumerator(), index, SelectedPlaylist.Songs.Count, false);
            }
        }
        private async void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            if ((e.OriginalSource as FrameworkElement).DataContext is SongViewModel song)
            {
                int index = MainList.Items.IndexOf(song);
                await App.PViewModel.StartMusicPlaybackAsync(SelectedPlaylist.Songs.GetEnumerator(), index, SelectedPlaylist.Songs.Count, false);
            }
            else
            {
                await App.PViewModel.StartMusicPlaybackAsync(SelectedPlaylist.Songs.GetEnumerator(), 0, SelectedPlaylist.Songs.Count, false);
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

        private void Album_Click(Hyperlink sender, HyperlinkClickEventArgs args)
            => EventsLogic.GoToAlbum(sender);

        private void Artist_Click(Hyperlink sender, HyperlinkClickEventArgs args)
            => EventsLogic.GoToArtist(sender);

        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine(SelectedPlaylist.Songs.Remove(SelectedSong));
        }

        private async void PlaylistProperties_Click(object sender, RoutedEventArgs e)
        {
            await SelectedPlaylist.StartEditAsync();
        }

        private async void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            await SelectedPlaylist.DeleteAsync();
            this.Frame.GoBack();
        }

        private void RemovefromPlaylist_Click(object sender, RoutedEventArgs e)
        {
            if ((e.OriginalSource as FrameworkElement).DataContext is SongViewModel song)
            {
                SelectedSong = song;
                Debug.WriteLine(SelectedPlaylist.Songs.Remove(SelectedSong));
            }
        }
    }
}
