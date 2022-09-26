using Rise.App.Dialogs;
using Rise.App.Helpers;
using Rise.App.UserControls;
using Rise.App.ViewModels;
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

        public VideoViewModel SelectedItem
        {
            get => (VideoViewModel)GetValue(SelectedItemProperty);
            set => SetValue(SelectedItemProperty, value);
        }

        private readonly string Label = "LocalVideos";

        public LocalVideosPage()
            : base("Title", App.MViewModel.Videos)
        {
            InitializeComponent();

            PlaylistHelper = new(App.MViewModel.Playlists, AddToPlaylistAsync);
            PlaylistHelper.AddPlaylistsToSubItem(AddTo);
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
        private async void MainGrid_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem is VideoViewModel video && !KeyboardHelpers.IsCtrlPressed())
            {
                await MPViewModel.PlaySingleItemAsync(video);
                if (Window.Current.Content is Frame rootFrame)
                    rootFrame.Navigate(typeof(VideoPlaybackPage));
            }
        }

        private void MenuFlyout_Opening(object sender, object e)
        {
            var fl = sender as MenuFlyout;
            var cont = MainGrid.ItemFromContainer(fl.Target);

            if (cont == null)
                fl.Hide();
            else
                SelectedItem = (VideoViewModel)cont;
        }

        private void AskDiscy_Click(object sender, RoutedEventArgs e)
        {
            DiscyOnVideo.IsOpen = true;
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
