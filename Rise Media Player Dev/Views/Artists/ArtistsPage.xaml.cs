using CommunityToolkit.Mvvm.Input;
using Rise.App.UserControls;
using Rise.App.ViewModels;
using Rise.Common.Extensions;
using Rise.Common.Helpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Rise.App.Views
{
    public sealed partial class ArtistsPage : MediaPageBase
    {
        private MainViewModel MViewModel => App.MViewModel;

        public ArtistViewModel SelectedItem
        {
            get => (ArtistViewModel)GetValue(SelectedItemProperty);
            set => SetValue(SelectedItemProperty, value);
        }

        public ArtistsPage()
            : base("Name", App.MViewModel.Artists, App.MViewModel.Playlists)
        {
            InitializeComponent();

            PlaylistHelper.AddPlaylistsToSubItem(AddTo, AddToPlaylistCommand);
            PlaylistHelper.AddPlaylistsToFlyout(AddToBar, AddToPlaylistCommand);
        }
    }

    // Playlists
    public sealed partial class ArtistsPage
    {
        [RelayCommand]
        private Task AddToPlaylistAsync(PlaylistViewModel playlist)
        {
            var name = SelectedItem.Name;
            var items = new List<SongViewModel>();

            foreach (var itm in MViewModel.Songs)
                if (itm.Artist == name)
                    items.Add(itm);

            if (playlist == null)
                return PlaylistHelper.CreateNewPlaylistAsync(items);
            else
                return playlist.AddItemsAsync(items);
        }
    }

    // Event handlers
    public sealed partial class ArtistsPage
    {
        private void MainGrid_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem is ArtistViewModel artist && !KeyboardHelpers.IsCtrlPressed())
            {
                _ = Frame.Navigate(typeof(ArtistSongsPage), artist.Model.Id);
            }
        }

        private void MenuFlyout_Opening(object sender, object e)
        {
            var fl = sender as MenuFlyout;
            var cont = MainGrid.ItemFromContainer(fl.Target);

            if (cont == null)
                fl.Hide();
            else
                SelectedItem = (ArtistViewModel)cont;
        }

        private void AskDiscy_Click(object sender, RoutedEventArgs e)
        {
            DiscyOnArtist.IsOpen = true;
        }

        private async void ChngArtImg_Click(object sender, RoutedEventArgs e)
        {
            var picker = new FileOpenPicker
            {
                ViewMode = PickerViewMode.Thumbnail,
                SuggestedStartLocation = PickerLocationId.PicturesLibrary
            };

            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".jpeg");
            picker.FileTypeFilter.Add(".png");

            StorageFile file = await picker.PickSingleFileAsync();

            if (file != null)
            {
                var img = await file.GetBitmapAsync(200, 200);

                var newFile = await ApplicationData.Current.LocalFolder.
                    CreateFileAsync($@"modified-artist-{SelectedItem.Name}.png", CreationCollisionOption.ReplaceExisting);

                var result = await img.SaveToFileAsync(newFile);

                if (result)
                {
                    SelectedItem.Picture = $@"ms-appdata:///local/modified-artist-{SelectedItem.Name}.png";
                    await SelectedItem.SaveAsync();
                }
            }
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
