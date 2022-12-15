using Rise.App.ViewModels;
using Rise.Common.Extensions;
using Rise.Common.Extensions.Markup;
using Rise.Data.Json;
using System;
using System.Linq;
using Windows.Storage.FileProperties;
using Windows.Storage.Pickers;
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

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            string title = TitleTextBox.Text;
            if (string.IsNullOrWhiteSpace(title))
            {
                ErrorBlock.Text = ResourceHelper.GetString("TitleNotEmpty");
                ErrorBlock.Visibility = Visibility.Visible;

                args.Cancel = true;
                return;
            }

            PlaylistViewModel plViewModel = new()
            {
                Title = title,
                Description = DescriptionTextBox.Text,
                Icon = _imagePath.OriginalString
            };

            var pl = PBackend.Items.FirstOrDefault(p => p.Title.Equals(title, StringComparison.OrdinalIgnoreCase));
            if (pl == null)
            {
                PBackend.Items.Add(plViewModel);
                PBackend.Save();
            }
            else
            {
                ErrorBlock.Text = ResourceHelper.GetString("PlaylistAlreadyExists");
                ErrorBlock.Visibility = Visibility.Visible;

                args.Cancel = true;
            }
        }

        private async void UseCustomImageButton_Click(object sender, RoutedEventArgs e)
        {
            var picker = new FileOpenPicker
            {
                ViewMode = PickerViewMode.Thumbnail,
                SuggestedStartLocation = PickerLocationId.PicturesLibrary
            };

            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".jpeg");
            picker.FileTypeFilter.Add(".png");

            var file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                var thumbnail = await file.GetThumbnailAsync(ThumbnailMode.MusicView, 200);
                if (await thumbnail.SaveToFileAsync($@"playlist-{file.Name}.png"))
                    _imagePath = new Uri($@"ms-appdata:///local/playlist-{file.Name}.png");

                thumbnail?.Dispose();
            }

            PreviewImage.Source = new BitmapImage(_imagePath);
        }
    }
}
