using RMP.App.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace RMP.App.Views
{
    public sealed partial class AlbumsPage : Page
    {
        /// <summary>
        /// Gets the app-wide MViewModel instance.
        /// </summary>
        private MainViewModel MViewModel => App.MViewModel;

        /// <summary>
        /// Gets the app-wide NPViewModel instance.
        /// </summary>
        private static PlaybackViewModel PViewModel => App.PViewModel;

        public AlbumsPage()
        {
            InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;
        }

        private async void PlayItem_Click(object sender, RoutedEventArgs e)
        {
            PViewModel.CancelTask();

            MViewModel.ClearFilters();
            MViewModel.Filters[1] = MViewModel.SelectedAlbum.Title;
            MViewModel.Filters[2] = MViewModel.SelectedAlbum.Artist;

            await PViewModel.CreatePlaybackList(0, MViewModel.FilteredSongs, PViewModel.Token);
        }

        private async void ShuffleItem_Click(object sender, RoutedEventArgs e)
        {
            MViewModel.ClearFilters();

            if (MViewModel.SelectedAlbum != null)
            {
                MViewModel.Filters[1] = MViewModel.SelectedAlbum.Model.Title;
                MViewModel.Filters[2] = MViewModel.SelectedAlbum.Model.Artist;
            }

            MViewModel.OrderBy = Enums.SortMethods.Random;
            await PViewModel.CreatePlaybackList(0, MViewModel.FilteredSongs, PViewModel.Token);
        }
    }
}
