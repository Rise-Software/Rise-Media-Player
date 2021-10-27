using RMP.App.ViewModels;
using RMP.App.Windows;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        private ObservableCollection<SongViewModel> Songs { get; set; }
            = new ObservableCollection<SongViewModel>();

        public ArtistSongsPage()
        {
            InitializeComponent();
            MainPage.Current.CrumbsHeader.Visibility = Visibility.Collapsed;

            Loaded += ArtistSongsPage_Loaded;
        }

        private void ArtistSongsPage_Loaded(object sender, RoutedEventArgs e)
        {
            IEnumerable<SongViewModel> songs =
                ViewModel.SongsFromArtist(SelectedArtist, ViewModel.Songs);

            Songs.Clear();
            foreach (SongViewModel song in songs)
            {
                Songs.Add(song);
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is ArtistViewModel artist)
            {
                SelectedArtist = artist;
            }

            base.OnNavigatedTo(e);
        }
    }
}
