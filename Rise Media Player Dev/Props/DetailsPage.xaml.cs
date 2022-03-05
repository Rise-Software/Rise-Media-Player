﻿using Rise.App.Common;
using Rise.App.ViewModels;
using System;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

namespace Rise.App.Props
{
    public sealed partial class DetailsPage : Page
    {
        private SongPropertiesViewModel Props { get; set; }

        public DetailsPage()
        {
            InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is SongPropertiesViewModel props)
            {
                Props = props;
            }

            base.OnNavigatedTo(e);
        }

        private void LocalButton_Click(object sender, RoutedEventArgs e)
        {
            LocalTip.IsOpen = true;
        }

        private async void EditArtButton_Click(Microsoft.UI.Xaml.Controls.SplitButton sender, Microsoft.UI.Xaml.Controls.SplitButtonClickEventArgs args)
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
                await FileHelpers.SaveBitmapFromThumbnailAsync(thumbnail, $@"modified-artist-{file.Name}.png");

                var uri = new Uri($@"ms-appdata:///local/modified-artist-{file.Name}.png");

                thumbnail.Dispose();
                Props.Thumbnail = uri.ToString();
                //Update the source in the XAML view
                imgAlbum.Source = new BitmapImage(uri);
            }
        }

        private async void exportAlbumArt_Click(object sender, RoutedEventArgs e)
        {
            StorageFile picFile =
                await StorageFile.GetFileFromApplicationUriAsync
                (new Uri(Props.Thumbnail));

            FolderPicker folderPicker = new FolderPicker
            {
                SuggestedStartLocation = PickerLocationId.PicturesLibrary
            };
            folderPicker.FileTypeFilter.Add("*");

            StorageFolder folder = await folderPicker.PickSingleFolderAsync();
            if (folder != null)
            {
                await picFile.CopyAsync(folder);
            }
        }
    }
}
