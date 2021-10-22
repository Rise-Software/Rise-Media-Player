using RMP.App.ViewModels;
using RMP.App.Views;
using RMP.App.Windows;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace RMP.App.UserControls
{
    public sealed partial class AlbumGrid : UserControl
    {
        /// <summary>
        /// Gets the app-wide MViewModel instance.
        /// </summary>
        private MainViewModel ViewModel => App.MViewModel;

        public AlbumGrid()
        {
            InitializeComponent();
        }

        private void Image_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if ((e.OriginalSource as FrameworkElement).DataContext is AlbumViewModel album)
            {
                _ = MainPage.Current.ContentFrame.Navigate(typeof(AlbumSongsPage), album);
            }
        }
    }
}
