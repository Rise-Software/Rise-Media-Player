using Rise.App.Common;
using Rise.App.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Rise.App.Views.Playlists.Properties
{

   
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PlaylistDetailsPropertiesPage : Page
    {
        private PlaylistViewModel _plViewModel;
        private PlaylistViewModel _updatedPlViewModel;
        public static PlaylistDetailsPropertiesPage Current;

        public PlaylistDetailsPropertiesPage()
        {
            this.InitializeComponent();
            Current = this;
            
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            _plViewModel = e.Parameter as PlaylistViewModel;
            _updatedPlViewModel = _plViewModel;
        }

        private async void exportPlaylistArt_Click(object sender, RoutedEventArgs e)
        {
            StorageFile picFile =
                await StorageFile.GetFileFromApplicationUriAsync
                (new Uri(_plViewModel.Icon));

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

        private async void EditPlaylistIcon_Click(Microsoft.UI.Xaml.Controls.SplitButton sender, Microsoft.UI.Xaml.Controls.SplitButtonClickEventArgs args)
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
                _updatedPlViewModel.Icon = uri.ToString();
                //Update the source in the XAML view
                imgAlbum.Source = new BitmapImage(uri);
            }
        }
    }
}
