using Rise.App.ViewModels;
using Rise.Common.Extensions;
using System;
using System.Collections.Generic;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Rise.App.Views
{
    public sealed partial class PlaylistDetailsPropertiesPage : Page
    {
        private PlaylistViewModel Playlist;

        public PlaylistDetailsPropertiesPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            Playlist = e.Parameter as PlaylistViewModel;
        }

        private async void exportPlaylistArt_Click(object sender, RoutedEventArgs e)
        {
            StorageFile picFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri(Playlist.Icon));

            FileSavePicker filePicker = new()
            {
                SuggestedStartLocation = PickerLocationId.PicturesLibrary,
            };

            filePicker.FileTypeChoices.Add("Portable Network Graphics", new List<string>() { ".png" });

            StorageFile file = await filePicker.PickSaveFileAsync();

            if (file != null)
                await picFile.CopyAndReplaceAsync(file);
        }

        private async void EditPlaylistIcon_Click(Microsoft.UI.Xaml.Controls.SplitButton sender, Microsoft.UI.Xaml.Controls.SplitButtonClickEventArgs args)
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
                    CreateFileAsync($@"modified-artist-{file.Name}.png", CreationCollisionOption.ReplaceExisting);

                var result = await img.SaveToFileAsync(newFile);

                if (result)
                {
                    var uri = new Uri($@"ms-appdata:///local/modified-artist-{file.Name}.png");
                    Playlist.Icon = uri.ToString();
                }
            }
        }
    }
}
