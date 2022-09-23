using Rise.App.Dialogs;
using Rise.App.Helpers;
using Rise.App.UserControls;
using Rise.App.ViewModels;
using Rise.Common.Extensions;
using Rise.Common.Helpers;
using System;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace Rise.App.Views
{
    public sealed partial class SongsPage : MediaPageBase
    {
        private SettingsViewModel SViewModel => App.SViewModel;
        private readonly AddToPlaylistHelper PlaylistHelper;

        public SongViewModel SelectedItem
        {
            get => (SongViewModel)GetValue(SelectedItemProperty);
            set => SetValue(SelectedItemProperty, value);
        }

        private readonly string Label = "Songs";

        public SongsPage()
            : base("Title", App.MViewModel.Songs)
        {
            InitializeComponent();

            PlaylistHelper = new(App.MViewModel.Playlists, AddToPlaylistAsync);
            PlaylistHelper.AddPlaylistsToSubItem(AddTo);
            PlaylistHelper.AddPlaylistsToFlyout(AddToBar);
        }
    }

    // Playlists
    public sealed partial class SongsPage
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
    public sealed partial class SongsPage
    {
        private void MainList_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            if ((e.OriginalSource as FrameworkElement).DataContext is SongViewModel song)
                MediaViewModel.PlayFromItemCommand.Execute(song);
        }

        private void MenuFlyout_Opening(object sender, object e)
        {
            var fl = sender as MenuFlyout;
            var cont = MainList.ItemFromContainer(fl.Target);

            if (cont == null)
                fl.Hide();
            else
                SelectedItem = (SongViewModel)cont;
        }

        private void AskDiscy_Click(object sender, RoutedEventArgs e)
        {
            DiscyOnSong.IsOpen = true;
        }

        private async void PlayFromUrl_Click(object sender, RoutedEventArgs e)
        {
            _ = await new MusicStreamingDialog().ShowAsync();
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

        private async void Remove_Click(object sender, RoutedEventArgs e)
        {
            var svm = SelectedItem;
            ContentDialog dialog = new()
            {
                Title = "Delete song",
                Content = $"Are you sure that you want to remove the song \"{svm.Title}\"?",
                PrimaryButtonStyle = Resources["AccentButtonStyle"] as Style,
                PrimaryButtonText = "Delete anyway",
                SecondaryButtonText = "Cancel"
            };

            var result = await dialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
                await svm.DeleteAsync();
            else
                dialog.Hide();
        }
    }
}
