using RMP.App.Settings.ViewModels;
using RMP.App.ViewModels;
using RMP.App.Windows;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

namespace RMP.App.Views
{
    public sealed partial class AlbumsPage : Page
    {
        public static AlbumsPage Current;

        /// <summary>
        /// Gets the app-wide MViewModel instance.
        /// </summary>
        private MainViewModel MViewModel => App.MViewModel;

        private ObservableCollection<AlbumViewModel> Albums { get; set; }

        /// <summary>
        /// Gets the app-wide NPViewModel instance.
        /// </summary>
        private static PlaybackViewModel PViewModel => App.PViewModel;

        /// <summary>
        /// Gets the app-wide SViewModel instance.
        /// </summary>
        private SettingsViewModel SViewModel => App.SViewModel;

        public AlbumsPage()
        {
            InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;

            Current = this;
            RefreshAlbums();
        }

        public void RefreshAlbums()
        {
            Debug.WriteLine("getting sussy albums");
            if (App.SViewModel.FilterByNameOnly)
            {
                Albums = new ObservableCollection<AlbumViewModel>
                    (MViewModel.Albums.GroupBy(a => a.Model.Title).Select(a => a.First()));
            }
            else
            {
                Albums = MViewModel.Albums;
            }

            AlbumGrid.ItemsSource = Albums;
        }

        private async void PlayItem_Click(object sender, RoutedEventArgs e)
        {
            if (MViewModel.SelectedAlbum != null)
            {
                PViewModel.CancelTask();

                MViewModel.ClearFilters();
                MViewModel.Filters[1] = MViewModel.SelectedAlbum.Model.Title;
                MViewModel.Filters[2] = MViewModel.SelectedAlbum.Model.Artist;

                await PViewModel.CreatePlaybackList(0, MViewModel.FilteredSongs, PViewModel.Token);
            }
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

        private void Image_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if ((e.OriginalSource as FrameworkElement).DataContext is AlbumViewModel album)
            {
                _ = MainPage.Current.ContentFrame.Navigate(typeof(AlbumSongsPage), album);
            }
        }
    }
}
