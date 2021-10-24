using RMP.App.ViewModels;
using RMP.App.Windows;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace RMP.App.Views
{
    public sealed partial class ArtistSongsPage : Page
    {
        /// <summary>
        /// Gets the app-wide MViewModel instance.
        /// </summary>
        private MainViewModel ViewModel => App.MViewModel;

        private ArtistViewModel SelectedArtist { get; set; }

        public ArtistSongsPage()
        {
            InitializeComponent();
            MainPage.Current.CrumbsHeader.Visibility = Visibility.Collapsed;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is ArtistViewModel artist)
            {
                SelectedArtist = artist;
                ViewModel.ClearFilters();

                ViewModel.Filters[3] = artist.Model.Name;
                ViewModel.StrictFilters[3] = true;

                SongList.List = ViewModel.FilteredSongs;
            }

            base.OnNavigatedTo(e);
        }
    }
}
