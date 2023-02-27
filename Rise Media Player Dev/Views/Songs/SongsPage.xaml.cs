using Rise.App.Dialogs;
using Rise.App.Helpers;
using Rise.App.UserControls;
using Rise.App.ViewModels;
using Rise.Common.Extensions.Markup;
using Rise.Data.Collections;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace Rise.App.Views
{
    public sealed partial class SongsPage : MediaPageBase
    {
        private SettingsViewModel SViewModel => App.SViewModel;

        public SongViewModel SelectedItem
        {
            get => (SongViewModel)GetValue(SelectedItemProperty);
            set => SetValue(SelectedItemProperty, value);
        }

        public SongsPage()
            : base(App.MViewModel.Playlists)
        {
            InitializeComponent();

            PlaylistHelper.AddPlaylistsToSubItem(AddTo, AddSelectedItemToPlaylistCommand);
            PlaylistHelper.AddPlaylistsToFlyout(AddToBar, AddSelectedItemToPlaylistCommand);

            var del = CollectionViewDelegates.GetDelegate("SongTitle");
            var sort = new SortDescription(SortDirection.Ascending, del);

            var groupDel = CollectionViewDelegates.GetDelegate("GSongTitle");
            CreateViewModel(App.MViewModel.Songs, new[] { sort }, null, groupDel, true);
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
                Title = ResourceHelper.GetString("/Settings/MediaLibraryManageFoldersTitle"),
                CloseButtonText = ResourceHelper.GetString("Close"),
                Content = new Settings.MediaSourcesPage()
            };
            _ = await dialog.ShowAsync();
        }

        private async void Remove_Click(object sender, RoutedEventArgs e)
        {
            var svm = SelectedItem;
            ContentDialog dialog = new()
            {
                Title = ResourceHelper.GetString("DeleteSong"),
                Content = string.Format(ResourceHelper.GetString("ConfirmRemovalSong"), svm.Title),
                PrimaryButtonStyle = Resources["AccentButtonStyle"] as Style,
                PrimaryButtonText = ResourceHelper.GetString("DeleteAnyway"),
                SecondaryButtonText = ResourceHelper.GetString("Close")
            };

            var result = await dialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
                await svm.DeleteAsync();
            else
                dialog.Hide();
        }
    }
}
