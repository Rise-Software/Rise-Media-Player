using Rise.App.ViewModels;
using Rise.Common.Extensions;
using Rise.Data.Json;
using System;
using System.Linq;
using Windows.Storage.FileProperties;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace Rise.App.Dialogs
{
    public sealed partial class CreatePlaylistDialog : ContentDialog
    {
        private JsonBackendController<PlaylistViewModel> PBackend
            => App.MViewModel.PBackend;
        private Uri _imagePath = new("ms-appx:///Assets/NavigationView/PlaylistsPage/blankplaylist.png");

        public CreatePlaylistDialog()
        {
            InitializeComponent();
        }

        #region Events/Methods

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            string title = string.IsNullOrWhiteSpace(TitleTextBox.Text) ? "Untitled" : TitleTextBox.Text;
            string description = string.IsNullOrWhiteSpace(DescriptionTextBox.Text) ? "No description." : DescriptionTextBox.Text;

            PlaylistViewModel plViewModel = new()
            {
                Title = title,
                Description = description,
                Icon = _imagePath.OriginalString
            };

            var pl = PBackend.Items.FirstOrDefault(p => p.Title == title);
            if (pl == null)
            {
                PBackend.Items.Add(plViewModel);
                PBackend.Save();
            }

            Hide();
        }

        private void ContentDialog_CloseButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args) => Hide();

        private async void UseCustomImageButton_Click(object sender, RoutedEventArgs e)
        {
            var picker = new Windows.Storage.Pickers.FileOpenPicker
            {
                ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail,
                SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary
            };
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".jpeg");
            picker.FileTypeFilter.Add(".png");

            Windows.Storage.StorageFile file = await picker.PickSingleFileAsync();

            if (file != null)
            {
                // Get file thumbnail and make a PNG out of it.
                StorageItemThumbnail thumbnail = await file.GetThumbnailAsync(ThumbnailMode.MusicView, 200);

                await thumbnail.SaveToFileAsync($@"playlist-{file.Name}.png");
                thumbnail?.Dispose();

                _imagePath = new Uri($@"ms-appdata:///local/playlist-{file.Name}.png");
            }

            PreviewImage.Source = new BitmapImage(_imagePath);
        }

        #endregion

    }
}
