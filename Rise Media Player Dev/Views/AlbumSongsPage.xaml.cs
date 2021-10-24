using RMP.App.ViewModels;
using RMP.App.Windows;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace RMP.App.Views
{
    /// <summary>
    /// Contains the songs inside an album.
    /// </summary>
    public sealed partial class AlbumSongsPage : Page
    {
        /// <summary>
        /// Gets the app-wide MViewModel instance.
        /// </summary>
        private MainViewModel ViewModel => App.MViewModel;

        private AlbumViewModel SelectedAlbum { get; set; }

        public AlbumSongsPage()
        {
            InitializeComponent();
            MainPage.Current.CrumbsHeader.Visibility = Visibility.Collapsed;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is AlbumViewModel album)
            {
                SelectedAlbum = album;
                ViewModel.ClearFilters();

                ViewModel.Filters[1] = album.Model.Title;
                ViewModel.Filters[2] = album.Model.Artist;
                ViewModel.StrictFilters[1] = true;
                ViewModel.StrictFilters[2] = true;

                SongList.List = ViewModel.FilteredSongs;
            }

            base.OnNavigatedTo(e);
        }
    }
}
