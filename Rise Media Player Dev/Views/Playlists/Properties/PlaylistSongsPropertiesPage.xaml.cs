using Rise.App.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Rise.App.Views.Playlists.Properties
{
    public sealed partial class PlaylistSongsPropertiesPage : Page
    {
        private PlaylistViewModel Playlist;

        public PlaylistSongsPropertiesPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            Playlist = e.Parameter as PlaylistViewModel;
        }

        private void RemoveSong_Click(object sender, RoutedEventArgs e)
        {
            SongViewModel song = (sender as Button).Tag as SongViewModel;
            Playlist.Songs.Remove(song);
        }

        private void MoveBottom_Click(object sender, RoutedEventArgs e)
        {
            SongViewModel song = (sender as Button).Tag as SongViewModel;

            if ((Playlist.Songs.IndexOf(song) + 1) < Playlist.Songs.Count)
            {
                try
                {
                    Playlist.Songs.Move(Playlist.Songs.IndexOf(song), Playlist.Songs.IndexOf(song) + 1);
                }
                catch
                {

                }
            }
        }

        private void MoveUp_Click(object sender, RoutedEventArgs e)
        {
            SongViewModel song = (sender as Button).Tag as SongViewModel;

            if ((Playlist.Songs.IndexOf(song) - 1) > 0)
            {
                Playlist.Songs.Move(Playlist.Songs.IndexOf(song), Playlist.Songs.IndexOf(song) - 1);
                int index1 = _plViewModel.Songs.IndexOf(song);
                int index2 = _plViewModel.Songs.IndexOf(song) - 1;
                System.Diagnostics.Debug.WriteLine(index1);
                System.Diagnostics.Debug.WriteLine(index2);
                System.Diagnostics.Debug.WriteLine(_plViewModel.Songs);
                //_plViewModel.Songs.Move(_plViewModel.Songs.IndexOf(song), _plViewModel.Songs.IndexOf(song) -1);
            }
        }
    }
}
