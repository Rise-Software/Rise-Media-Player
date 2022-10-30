using CommunityToolkit.Mvvm.Input;
using Rise.App.UserControls;
using Rise.App.ViewModels;
using Rise.Common.Extensions;
using Rise.Common.Helpers;
using Rise.Data.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace Rise.App.Views
{
    public sealed partial class PlaylistDetailsPage : MediaPageBase
    {
        private MainViewModel MViewModel => App.MViewModel;
        private MediaPlaybackViewModel MPViewModel => App.MPViewModel;

        private MediaCollectionViewModel VideosViewModel;

        public static readonly DependencyProperty SelectedVideoProperty =
            DependencyProperty.Register("SelectedVideo", typeof(VideoViewModel),
                typeof(PlaylistDetailsPage), new PropertyMetadata(null));

        public SongViewModel SelectedItem
        {
            get => (SongViewModel)GetValue(SelectedItemProperty);
            set => SetValue(SelectedItemProperty, value);
        }

        public VideoViewModel SelectedVideo
        {
            get => (VideoViewModel)GetValue(SelectedVideoProperty);
            set => SetValue(SelectedVideoProperty, value);
        }

        private PlaylistViewModel SelectedPlaylist;
        private double? _offset = null;

        public PlaylistDetailsPage()
            : base(App.MViewModel.Playlists)
        {
            InitializeComponent();

            NavigationHelper.LoadState += NavigationHelper_LoadState;

            PlaylistHelper.AddPlaylistsToSubItem(AddTo, AddSelectedItemToPlaylistCommand);
            PlaylistHelper.AddPlaylistsToSubItem(AddToVideo, AddVideoToPlaylistCommand);
        }

        private void OnPageLoaded(object sender, RoutedEventArgs e)
        {
            if (_offset != null)
                MainList.FindVisualChild<ScrollViewer>().ChangeView(null, _offset, null);
        }

        private void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            if (e.NavigationParameter is Guid id)
            {
                SelectedPlaylist = App.MViewModel.Playlists.
                    FirstOrDefault(p => p.Model.Id == id);

                CreateViewModel("Title", SelectedPlaylist.Songs);
                VideosViewModel = new("Title", SelectedPlaylist.Videos, null, MPViewModel);
            }
        }
    }

    // Playlists
    public sealed partial class PlaylistDetailsPage
    {
        [RelayCommand]
        private Task AddVideoToPlaylistAsync(PlaylistViewModel playlist)
        {
            if (playlist == null)
                return PlaylistHelper.CreateNewPlaylistAsync(SelectedVideo);
            else
                return playlist.AddVideoAsync(SelectedVideo);
        }
    }

    // Event handlers
    public sealed partial class PlaylistDetailsPage
    {
        private void MainList_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            if ((e.OriginalSource as FrameworkElement).DataContext is SongViewModel song)
                MediaViewModel.PlayFromItemCommand.Execute(song);
        }

        private void SongFlyout_Opening(object sender, object e)
        {
            var fl = sender as MenuFlyout;
            var cont = MainList.ItemFromContainer(fl.Target);

            if (cont == null)
                fl.Hide();
            else
                SelectedItem = (SongViewModel)cont;
        }

        private void VideoFlyout_Opening(object sender, object e)
        {
            var fl = sender as MenuFlyout;
            var cont = MainGrid.ItemFromContainer(fl.Target);

            if (cont == null)
                fl.Hide();
            else
                SelectedVideo = (VideoViewModel)cont;
        }

        private async void RemoveSong_Click(object sender, RoutedEventArgs e)
        {
            await SelectedPlaylist.RemoveSongAsync(SelectedItem);
        }

        private async void RemoveVideo_Click(object sender, RoutedEventArgs e)
        {
            await SelectedPlaylist.RemoveVideoAsync(SelectedVideo);
        }

        private async void GridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem is VideoViewModel video && !KeyboardHelpers.IsCtrlPressed())
            {
                await MPViewModel.PlaySingleItemAsync(video);
                if (Window.Current.Content is Frame rootFrame)
                    rootFrame.Navigate(typeof(VideoPlaybackPage));
            }
        }

        private async void PlayVideo_Click(object sender, RoutedEventArgs e)
        {
            await MPViewModel.PlaySingleItemAsync(SelectedVideo);
            if (Window.Current.Content is Frame rootFrame)
                _ = rootFrame.Navigate(typeof(VideoPlaybackPage));
        }
    }
}
