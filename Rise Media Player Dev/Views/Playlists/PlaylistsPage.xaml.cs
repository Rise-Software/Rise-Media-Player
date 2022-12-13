using Rise.App.Dialogs;
using Rise.App.UserControls;
using Rise.App.ViewModels;
using Rise.Common.Helpers;
using Rise.Data.Json;
using System;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Rise.App.Views
{
    public sealed partial class PlaylistsPage : MediaPageBase
    {
        private JsonBackendController<PlaylistViewModel> PBackend
            => App.MViewModel.PBackend;

        public PlaylistViewModel SelectedItem
        {
            get => (PlaylistViewModel)GetValue(SelectedItemProperty);
            set => SetValue(SelectedItemProperty, value);
        }

        public PlaylistsPage()
            : base("Title", App.MViewModel.Playlists)
        {
            InitializeComponent();
        }
    }

    // Event handlers
    public sealed partial class PlaylistsPage
    {
        private void MainGrid_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem is PlaylistViewModel playlist && !KeyboardHelpers.IsCtrlPressed())
            {
                _ = Frame.Navigate(typeof(PlaylistDetailsPage), playlist.Id);
            }
        }

        private void MenuFlyout_Opening(object sender, object e)
        {
            var fl = sender as MenuFlyout;
            var cont = MainGrid.ItemFromContainer(fl.Target);

            if (cont == null)
                fl.Hide();
            else
                SelectedItem = (PlaylistViewModel)cont;
        }

        private void AskDiscy_Click(object sender, RoutedEventArgs e)
        {
            DiscyOnPlaylist.IsOpen = true;
        }

        private async void CreatePlaylistButton_Click(object sender, RoutedEventArgs e)
        {
            await new CreatePlaylistDialog().ShowAsync();
        }

        private async void DeletePlaylist_Click(object sender, RoutedEventArgs e)
        {
            PBackend.Items.Remove(SelectedItem);
            await PBackend.SaveAsync();
        }

        private async void ImportPlaylist_Click(object sender, RoutedEventArgs e)
        {
            FileOpenPicker picker = new();
            picker.FileTypeFilter.Add(".m3u");

            StorageFile file = await picker.PickSingleFileAsync();

            if (file != null)
            {
                var playlist = await PlaylistViewModel.GetFromFileAsync(file);

                PBackend.Items.Add(playlist);
                await PBackend.SaveAsync();
            }
        }
    }
}
