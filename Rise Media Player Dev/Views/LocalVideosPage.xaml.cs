using Rise.App.Dialogs;
using Rise.App.Helpers;
using Rise.App.UserControls;
using Rise.App.ViewModels;
using Rise.Common.Enums;
using Rise.Common.Extensions;
using Rise.Common.Helpers;
using Rise.Data.ViewModels;
using System;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Rise.App.Views
{
    public sealed partial class LocalVideosPage : MediaPageBase
    {
        private MediaPlaybackViewModel MPViewModel => App.MPViewModel;
        private readonly AddToPlaylistHelper PlaylistHelper;

        private VideoViewModel SelectedItem
        {
            get => (VideoViewModel)GetValue(SelectedItemProperty);
            set => SetValue(SelectedItemProperty, value);
        }

        private readonly string Label = "LocalVideos";
        private double? _offset = null;

        public LocalVideosPage()
            : base(MediaItemType.Video, App.MViewModel.Videos)
        {
            InitializeComponent();

            NavigationHelper.LoadState += NavigationHelper_LoadState;
            NavigationHelper.SaveState += NavigationHelper_SaveState;

            PlaylistHelper = new(App.MViewModel.Playlists, AddToPlaylistAsync);
            PlaylistHelper.AddPlaylistsToSubItem(AddTo);
        }

        private void OnPageLoaded(object sender, RoutedEventArgs e)
        {
            if (_offset != null)
                MainGrid.FindVisualChild<ScrollViewer>().ChangeView(null, _offset, null);
        }

        private void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            if (e.PageState != null)
            {
                bool result = e.PageState.TryGetValue("Offset", out var offset);
                if (result)
                    _offset = (double)offset;
            }
        }

        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
            var scr = MainGrid.FindVisualChild<ScrollViewer>();
            if (scr != null)
                e.PageState["Offset"] = scr.VerticalOffset;
        }
    }

    // Playlists
    public sealed partial class LocalVideosPage
    {
        private Task AddToPlaylistAsync(PlaylistViewModel playlist)
        {
            if (playlist == null)
                return PlaylistHelper.CreateNewPlaylistAsync(SelectedItem);
            else
                return playlist.AddVideoAsync(SelectedItem);
        }
    }

    // Event handlers
    public sealed partial class LocalVideosPage
    {
        private async void GridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem is VideoViewModel video)
            {
                if (!KeyboardHelpers.IsCtrlPressed())
                {
                    await MPViewModel.PlaySingleItemAsync(video);
                    if (Window.Current.Content is Frame rootFrame)
                        rootFrame.Navigate(typeof(VideoPlaybackPage));
                }
                else
                {
                    SelectedItem = video;
                }
            }
        }

        private void Play_Click(object sender, RoutedEventArgs e)
        {
            if (Window.Current.Content is Frame rootFrame)
                _ = rootFrame.Navigate(typeof(VideoPlaybackPage));
        }

        private void AskDiscy_Click(object sender, RoutedEventArgs e)
        {
            DiscyOnVideo.IsOpen = true;
        }

        private void MainGrid_RightTapped(object sender, Windows.UI.Xaml.Input.RightTappedRoutedEventArgs e)
        {
            if ((e.OriginalSource as FrameworkElement).DataContext is VideoViewModel video)
            {
                SelectedItem = video;
                VideosFlyout.ShowAt(MainGrid, e.GetPosition(MainGrid));
            }
        }

        private async void PlayFromUrl_Click(object sender, RoutedEventArgs e)
        {
            _ = await new VideoStreamingDialog().ShowAsync();
        }

        private async void AddFolders_Click(object sender, RoutedEventArgs e)
        {
            ContentDialog dialog = new()
            {
                Title = "Manage local media folders",
                CloseButtonText = "Close",
                Content = new Settings.MediaSourcesPage()
            };

            _ = await dialog.ShowAsync();
        }
    }
}
