using RMP.App.ViewModels;
using RMP.App.Views;
using RMP.App.Windows;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace RMP.App.UserControls
{
    public sealed partial class ArtistGrid : UserControl
    {
        /// <summary>
        /// Gets the app-wide MViewModel instance.
        /// </summary>
        public MainViewModel ViewModel => App.MViewModel;

        public ArtistGrid()
        {
            InitializeComponent();
        }

        private void Image_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if ((e.OriginalSource as FrameworkElement).DataContext is ArtistViewModel artist)
            {
                _ = MainPage.Current.ContentFrame.Navigate(typeof(ArtistSongsPage), artist);
            }
        }
    }
}
