using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Toolkit.Uwp.UI.Animations;
using Rise.App.ViewModels;
using Rise.Common.Extensions;
using Rise.Common.Helpers;
using Rise.Data.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

namespace Rise.App.Views
{
    // Fields, properties
    public sealed partial class PlaylistDetailsPage : Page
    {
        private SongViewModel _song;

        private MediaPlaybackViewModel MPViewModel => App.MPViewModel;

        private static readonly DependencyProperty SelectedSongProperty =
            DependencyProperty.Register("SelectedSong", typeof(SongViewModel), typeof(PlaylistDetailsPage), null);

        private static readonly DependencyProperty SelectedVideoProperty =
            DependencyProperty.Register("SelectedVideo", typeof(VideoViewModel), typeof(PlaylistDetailsPage), null);

        private SongViewModel SelectedSong
        {
            get => (SongViewModel)GetValue(SelectedSongProperty);
            set => SetValue(SelectedSongProperty, value);
        }

        private VideoViewModel SelectedVideo
        {
            get => (VideoViewModel)GetValue(SelectedVideoProperty);
            set => SetValue(SelectedVideoProperty, value);
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
                var songs = new List<SongViewModel>(SelectedPlaylist.Songs);

                songs.MoveRangeToEnd(0, index - 1);
                await MPViewModel.PlayItemsAsync(songs);
            }
        }

        private async void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            if ((e.OriginalSource as FrameworkElement).DataContext is SongViewModel song)
            {
                int index = MainList.Items.IndexOf(song);
                var songs = new List<SongViewModel>(SelectedPlaylist.Songs);

                songs.MoveRangeToEnd(0, index - 1);
                await MPViewModel.PlayItemsAsync(songs);
            }
            else
            {
                await MPViewModel.PlayItemsAsync(SelectedPlaylist.Songs);
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

        private async void ShuffleButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await EventsLogic.StartMusicPlaybackAsync(0, true);
            }
            catch
            {

            }
        }

        private async void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            await SelectedPlaylist.DeleteAsync();
            Frame.GoBack();
        }

        private void RemovefromPlaylist_Click(object sender, RoutedEventArgs e)
        {
            if ((e.OriginalSource as FrameworkElement).DataContext is SongViewModel song)
            {
                SelectedSong = song;
                Debug.WriteLine(SelectedPlaylist.Songs.Remove(SelectedSong));
            }
        }

        private async void MoveSongUp_Click(object sender, RoutedEventArgs e)
        {
            if ((e.OriginalSource as FrameworkElement).DataContext is SongViewModel song)
            {
                if ((SelectedPlaylist.Songs.IndexOf(song) - 1) >= 0)
                {
                    var index = SelectedPlaylist.Songs.IndexOf(song);

                    SelectedPlaylist.Songs.Remove(song);
                    SelectedPlaylist.Songs.Insert(index - 1, song);
                    await SelectedPlaylist.SaveEditsAsync();
                }
            }
        }

        private async void MoveSongDown_Click(object sender, RoutedEventArgs e)
        {
            if ((e.OriginalSource as FrameworkElement).DataContext is SongViewModel song)
            {
                if ((SelectedPlaylist.Songs.IndexOf(song) + 1) < SelectedPlaylist.Songs.Count)
                {
                    var index = MainList.Items.IndexOf(song);

                    SelectedPlaylist.Songs.Remove(song);
                    SelectedPlaylist.Songs.Insert(index + 1, song);
                    await SelectedPlaylist.SaveEditsAsync();
                }
            }
        }

        private void MainGrid_RightTapped_1(object sender, RightTappedRoutedEventArgs e)
        {
            if ((e.OriginalSource as FrameworkElement).DataContext is VideoViewModel video)
            {
                SelectedVideo = video;
                VideosFlyout.ShowAt(MainGrid, e.GetPosition(MainGrid));
            }
        }

        private async void GridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (!KeyboardHelpers.IsCtrlPressed())
            {
                if (e.ClickedItem is VideoViewModel video)
                {
                    await MPViewModel.PlaySingleItemAsync(video);
                    if (Window.Current.Content is Frame rootFrame)
                    {
                        rootFrame.Navigate(typeof(VideoPlaybackPage));
                    }
                    SelectedVideo = null;
                }
            }
            else
            {
                if (e.ClickedItem is VideoViewModel video)
                {
                    SelectedVideo = video;
                }
            }
        }

        private async void Play_Click(object sender, RoutedEventArgs e)
        {
            if (MainGrid.Items.Count > 0)
            {
                if (SelectedVideo != null)
                {
                    int index = MainList.Items.IndexOf(SelectedVideo);
                    var videos = new List<VideoViewModel>(SelectedPlaylist.Videos);

                    videos.MoveRangeToEnd(0, index - 1);
                    await MPViewModel.PlayItemsAsync(videos);
                }
                else
                {
                    await MPViewModel.PlayItemsAsync(SelectedPlaylist.Videos);
                }

                if (Window.Current.Content is Frame rootFrame)
                {
                    _ = rootFrame.Navigate(typeof(VideoPlaybackPage));
                }
            }
        }

        private async void NewPlaylistItem_Click(object sender, RoutedEventArgs e)
        {
            PlaylistViewModel playlist = new()
            {
                Title = $"Untitled Playlist #{App.MViewModel.Playlists.Count + 1}",
                Description = "",
                Icon = "ms-appx:///Assets/NavigationView/PlaylistsPage/blankplaylist.png",
                Duration = "0"
            };

            await playlist.AddVideoAsync(SelectedVideo, true);
        }

        private async void Item_Click(object sender, RoutedEventArgs e)
        {
            PlaylistViewModel playlist = (sender as MenuFlyoutItem).Tag as PlaylistViewModel;
            await playlist.AddVideoAsync(SelectedVideo);
        }
    }

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
        {
            _navigationHelper.OnNavigatedTo(e);
        }


        protected override void OnNavigatedFrom(NavigationEventArgs e)
            => _navigationHelper.OnNavigatedFrom(e);
        #endregion
    }
}
