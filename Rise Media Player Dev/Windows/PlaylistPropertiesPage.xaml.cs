using Rise.App.Common;
using Rise.App.ViewModels;
using Rise.App.Views;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.FileProperties;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Rise.App.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PlaylistPropertiesPage : Page
    {
        private PlaylistViewModel _plViewModel;
        private PlaylistViewModel _updatedPlViewModel;
        private Uri _imagePath = new("ms-appx:///Assets/NavigationView/PlaylistsPage/blankplaylist.png");
        private IEnumerable<ToggleButton> Toggles { get; set; }

        public PlaylistPropertiesPage()
        {
            InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;

            Toggles = ItemGrid.GetChildren<ToggleButton>();
            //Loaded += (s, e) =>
            //{
            //    _ = new ApplicationTitleBar(TitleBar);
            //};
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            Details.IsChecked = true;
            _plViewModel = e.Parameter as PlaylistViewModel;
            _updatedPlViewModel = new(_plViewModel.Model);
        }

        private void ToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            Details.IsChecked = false;
            Songs.IsChecked = false;

            ToggleButton clicked = (ToggleButton)sender;
            clicked.Checked -= ToggleButton_Checked;
            clicked.IsChecked = true;

            switch (clicked.Tag.ToString())
            {
                case "DetailsItem":
                    //_ = PropsFrame.Navigate(typeof(DetailsPage), Props);
                    break;

                case "SongsItem":
                    //_ = PropsFrame.Navigate(typeof(FilePage), Props);
                    break;

                default:
                    break;
            }

            clicked.Checked += ToggleButton_Checked;
        }

        //private void Image_PointerEntered(object sender, PointerRoutedEventArgs e)
        //{
        //    UseCustomImageButton.Visibility = Visibility.Visible;
        //}

        //private void Image_PointerExited(object sender, PointerRoutedEventArgs e)
        //{
        //    UseCustomImageButton.Visibility = Visibility.Collapsed;
        //}

        //private async void UseCustomImageButton_Click(object sender, RoutedEventArgs e)
        //{
        //    var picker = new Windows.Storage.Pickers.FileOpenPicker
        //    {
        //        ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail,
        //        SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary
        //    };
        //    picker.FileTypeFilter.Add(".jpg");
        //    picker.FileTypeFilter.Add(".jpeg");
        //    picker.FileTypeFilter.Add(".png");

        //    Windows.Storage.StorageFile file = await picker.PickSingleFileAsync();

        //    if (file != null)
        //    {
        //        // Get file thumbnail and make a PNG out of it.
        //        StorageItemThumbnail thumbnail = await file.GetThumbnailAsync(ThumbnailMode.MusicView, 200);

        //        await FileHelpers.SaveBitmapFromThumbnailAsync(thumbnail, $@"playlist-{file.Name}.png");

        //        thumbnail.Dispose();
        //        _imagePath = new Uri($@"ms-appdata:///local/playlist-{file.Name}.png");
        //    }

        //    PreviewImage.Source = new BitmapImage(_imagePath);
        //}

        private async void CancelButton_Click(object sender, RoutedEventArgs e)
            => _ = await ApplicationView.GetForCurrentView().TryConsolidateAsync();

    }
}
