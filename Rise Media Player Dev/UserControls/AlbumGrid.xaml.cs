using RMP.App.ViewModels;
using System;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace RMP.App.UserControls
{
    public sealed partial class AlbumGrid : UserControl
    {
        /// <summary>
        /// Gets the app-wide ViewModel instance.
        /// </summary>
        public MainViewModel ViewModel => App.ViewModel;
        public AlbumGrid()
        {
            this.InitializeComponent();
            this.Loaded += AlbumGrid_Loaded;
        }

        private async void AlbumGrid_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            foreach (AlbumViewModel model in ViewModel.Albums)
            {
                BitmapImage bmp = new BitmapImage();
                StorageFolder localCache = ApplicationData.Current.LocalCacheFolder;

                IStorageItem resultingItem = await localCache.TryGetItemAsync(model.Thumbnail);

                // If the file exists, apply thumbnail, use default one otherwise
                if (resultingItem != null)
                {
                    StorageFile file = resultingItem as StorageFile;
                    IRandomAccessStreamWithContentType stream = await file.OpenReadAsync();
                    bmp.SetSource(stream);

                    model.ThumbnailBitmap = bmp;
                }
                else
                {
                    model.ThumbnailBitmap = new BitmapImage(new Uri("ms-appx:///Assets/Default.png"));
                }
            }

            AlbumGridView.ItemsSource = ViewModel.Albums;
        }
    }
}
