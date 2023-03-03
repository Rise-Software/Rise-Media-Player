using CommunityToolkit.Mvvm.Input;
using Rise.App.Converters;
using Rise.App.UserControls;
using Rise.App.ViewModels;
using Rise.Common.Extensions;
using Rise.Common.Helpers;
using Rise.Common.Interfaces;
using Rise.Data.Json;
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
        private JsonBackendController<PlaylistViewModel> PBackend
            => App.MViewModel.PBackend;

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

        public PlaylistDetailsPage()
            : base(App.MViewModel.Playlists)
        {
            InitializeComponent();

            NavigationHelper.LoadState += NavigationHelper_LoadState;
            NavigationHelper.SaveState += NavigationHelper_SaveState;

            PlaylistHelper.AddPlaylistsToSubItem(AddTo, AddSelectedItemToPlaylistCommand);
            PlaylistHelper.AddPlaylistsToSubItem(AddToVideo, AddVideoToPlaylistCommand);
        }

        private async void OnPageLoaded(object sender, RoutedEventArgs e)
        {
            PlaylistDuration.Text = await Task.Run(() => TimeSpanToString.GetShortFormat(TimeSpan.FromSeconds(MediaViewModel.Items.Cast<SongViewModel>().Select(s => s.Length).Aggregate((t, t1) => t + t1).TotalSeconds)));
        }

        private void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            if (e.NavigationParameter is Guid id)
            {
                SelectedPlaylist = MViewModel.Playlists.
                    FirstOrDefault(p => p.Id == id);

                CreateViewModel(string.Empty, SelectedPlaylist.Songs);
                VideosViewModel = new(string.Empty, SelectedPlaylist.Videos, false, null, MPViewModel);
            }
        }

        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
            VideosViewModel.Dispose();
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

            playlist.AddItem(SelectedVideo);
            return PBackend.SaveAsync();
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

        [RelayCommand]
        private Task RemoveItemAsync(IMediaItem item)
        {
            SelectedPlaylist.RemoveItem(item);
            return PBackend.SaveAsync();
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
