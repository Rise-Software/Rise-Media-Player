using RMP.App.ViewModels;
using RMP.App.Windows;
using System.Collections.ObjectModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

namespace RMP.App.Views
{
    public sealed partial class ArtistsPage : Page
    {
        /// <summary>
        /// Gets the app-wide MViewModel instance.
        /// </summary>
        private MainViewModel MViewModel => App.MViewModel;

        private ArtistViewModel SelectedArtist { get; set; }
        private ObservableCollection<ArtistViewModel> Artists { get; set; }

        public ArtistsPage()
        {
            Artists = MViewModel.Artists;

            InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;
        }

        #region Event handlers
        private void GridView_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if ((e.OriginalSource as FrameworkElement).DataContext is ArtistViewModel artist)
            {
                _ = MainPage.Current.ContentFrame.Navigate(typeof(ArtistSongsPage), artist);
                SelectedArtist = null;
            }
        }
        #endregion
    }
}
