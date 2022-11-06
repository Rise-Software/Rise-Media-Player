using Rise.App.Dialogs;
using Rise.App.UserControls;
using Rise.App.ViewModels;
using Rise.Common.Enums;
using Rise.Common.Helpers;
using Rise.Data.Sources;
using Rise.Data.ViewModels;
using System;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Rise.App.Views
{
    public sealed partial class PlaylistsPage : MediaPageBase
    {
        private NavViewDataSource NavDataSource => App.NavDataSource;

        public PlaylistViewModel SelectedItem
        {
            get => (PlaylistViewModel)GetValue(SelectedItemProperty);
            set => SetValue(SelectedItemProperty, value);
        }

        private readonly string Label = "Playlists";

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
                _ = Frame.Navigate(typeof(PlaylistDetailsPage), playlist.Model.Id);
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
            await SelectedItem.DeleteAsync();
        }

        private void PinToSidebar_Click(object sender, RoutedEventArgs e)
        {
            bool hasItem = NavDataSource.TryGetItem("PlaylistsPage", out var item);
            if (hasItem)
            {
                var playlist = SelectedItem;
                var itm = new NavViewItemViewModel
                {
                    Id = playlist.Model.Id.ToString(),
                    ItemType = NavViewItemType.SubItem,
                    Icon = playlist.Icon,
                    Label = playlist.Title,
                    ParentId = item.Id,
                    FlyoutId = "RemoveItemFlyout"
                };
                item.SubItems.Add(itm);
            }
        }

        private async void ImportPlaylist_Click(object sender, RoutedEventArgs e)
        {
            FileOpenPicker picker = new();
            picker.FileTypeFilter.Add(".m3u");

            StorageFile file = await picker.PickSingleFileAsync();

            if (file != null)
            {
                var playlist = await PlaylistViewModel.GetFromFileAsync(file);
                await playlist.SaveAsync();
            }
        }
    }
}
