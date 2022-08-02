using Microsoft.Toolkit.Uwp.UI.Animations;
using Rise.App.Helpers;
using Rise.App.UserControls;
using Rise.App.ViewModels;
using Rise.Common.Enums;
using Rise.Common.Extensions;
using Rise.Common.Helpers;
using Rise.Data.ViewModels;
using System;
using System.Collections.Generic;
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

        private readonly AddToPlaylistHelper PlaylistHelper;
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
        {
            InitializeComponent();

            NavigationHelper.LoadState += NavigationHelper_LoadState;
            NavigationHelper.SaveState += NavigationHelper_SaveState;

            PlaylistHelper = new(MViewModel.Playlists, AddToPlaylistAsync);
        }

        private void OnPageLoaded(object sender, RoutedEventArgs e)
        {
            if (_offset != null)
                RootViewer.ChangeView(null, _offset, null);
        }

        private void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            if (e.NavigationParameter is Guid id)
            {
                SelectedPlaylist = App.MViewModel.Playlists.
                    FirstOrDefault(p => p.Model.Id == id);

                CreateViewModel(MediaItemType.Song, SelectedPlaylist.Songs);
                VideosViewModel = new(MediaItemType.Video, SelectedPlaylist.Videos, null, MPViewModel);
            }

            if (e.PageState != null)
            {
                bool result = e.PageState.TryGetValue("Offset", out var offset);
                if (result)
                    _offset = (double)offset;
            }
        }

        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
            e.PageState["Offset"] = RootViewer.VerticalOffset;
            Frame.SetListDataItemForNextConnectedAnimation(SelectedPlaylist);
        }
    }

    // Playlists
    public sealed partial class PlaylistDetailsPage
    {
        private Task AddToPlaylistAsync(PlaylistViewModel playlist)
        {
            if (playlist == null)
                return PlaylistHelper.CreateNewPlaylistAsync(SelectedItem);
            else
                return playlist.AddSongAsync(SelectedItem);
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

        private async void Remove_Click(object sender, RoutedEventArgs e)
        {
            await SelectedPlaylist.RemoveSongAsync(SelectedItem);
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
