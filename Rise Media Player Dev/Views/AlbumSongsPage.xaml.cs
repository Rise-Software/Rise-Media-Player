using RMP.App.ViewModels;
using RMP.App.Windows;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
        private ObservableCollection<SongViewModel> Songs { get; set; }
            = new ObservableCollection<SongViewModel>();

        public AlbumSongsPage()
        {
            InitializeComponent();
            MainPage.Current.CrumbsHeader.Visibility = Visibility.Collapsed;

            Loaded += AlbumSongsPage_Loaded;
        }

        private void AlbumSongsPage_Loaded(object sender, RoutedEventArgs e)
        {
            IEnumerable<SongViewModel> songs =
                ViewModel.SongsFromAlbum(SelectedAlbum, ViewModel.Songs);

            Songs.Clear();
            foreach (SongViewModel song in songs)
            {
                Songs.Add(song);
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is AlbumViewModel album)
            {
                SelectedAlbum = album;
            }

            base.OnNavigatedTo(e);
        }
    }
}
