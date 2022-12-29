using Rise.App.ViewModels;
using Rise.Common.Extensions;
using Rise.Models;
using System;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

namespace Rise.App.Views
{
    public sealed partial class SongDetailsPage : Page
    {
        private SongPropertiesViewModel Props { get; set; }

        public SongDetailsPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is SongPropertiesViewModel props)
            {
                Props = props;
            }

            if (Props.Model.Model.IsLocal)
                LocalExpander.Visibility = Visibility.Visible;
            else
                OnlineExpander.Visibility = Visibility.Visible;

            base.OnNavigatedTo(e);
        }

        private void LocalButton_Click(object sender, RoutedEventArgs e)
        {
            LocalTip.IsOpen = true;
        }

        private async void EditArtButton_Click(Microsoft.UI.Xaml.Controls.SplitButton sender, Microsoft.UI.Xaml.Controls.SplitButtonClickEventArgs args)
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
                // Get file thumbnail and make a PNG out of it.
                using StorageItemThumbnail thumbnail = await file.GetThumbnailAsync(ThumbnailMode.MusicView, 200);
                await thumbnail.SaveToFileAsync($@"modified-artist-{file.Name}.png");

                var uri = new Uri($@"ms-appdata:///local/modified-artist-{file.Name}.png");

                Props.Thumbnail = uri.ToString();

                imgAlbum.Source = new BitmapImage(uri);
            }
        }

        private async void exportAlbumArt_Click(object sender, RoutedEventArgs e)
        {
            StorageFile picFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri(Props.Thumbnail));

            FileSavePicker fileSavePicker = new()
            {
                SuggestedStartLocation = PickerLocationId.PicturesLibrary
            };

            fileSavePicker.FileTypeChoices.Add("PNG Image", new string[] { ".png" });
            fileSavePicker.FileTypeChoices.Add("JPEG Image", new string[] { ".jpg" });

            StorageFile file = await fileSavePicker.PickSaveFileAsync();

            if (file != null)
                await picFile.CopyAndReplaceAsync(file);
        }

        private void OnlineButton_Click(object sender, RoutedEventArgs e)
        {
            OnlineTip.IsOpen = true;
        }
    }
}
