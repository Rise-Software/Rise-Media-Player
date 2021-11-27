using Microsoft.Toolkit.Uwp.UI;
using Rise.App.ViewModels;
using Rise.App.Views;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

namespace Rise.App.UserControls
{
    public sealed partial class AlbumGrid : UserControl
    {
        #region Variables
        /// <summary>
        /// Gets the app-wide MViewModel instance.
        /// </summary>
        private MainViewModel MViewModel => App.MViewModel;

        /// <summary>
        /// Gets the app-wide PViewModel instance.
        /// </summary>
        private PlaybackViewModel PViewModel => App.PViewModel;

        private static readonly DependencyProperty SelectedAlbumProperty =
            DependencyProperty.Register("SelectedAlbum", typeof(AlbumViewModel), typeof(AlbumGrid), null);

        private AlbumViewModel SelectedAlbum
        {
            get => (AlbumViewModel)GetValue(SelectedAlbumProperty);
            set => SetValue(SelectedAlbumProperty, value);
        }

        private AdvancedCollectionView Albums => MViewModel.FilteredAlbums;
        private AdvancedCollectionView Songs => MViewModel.FilteredSongs;

        public string SortProperty = "Title";
        public SortDirection CurrentSort = SortDirection.Ascending;
        #endregion

        public AlbumGrid()
        {
            InitializeComponent();
        }

        #region Event handlers
        private void GridView_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if ((e.OriginalSource as FrameworkElement).DataContext is AlbumViewModel album)
            {
                _ = MainPage.Current.ContentFrame.
                    Navigate(typeof(AlbumSongsPage), album);
            }

            SelectedAlbum = null;
        }

        private void MainGrid_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            if ((e.OriginalSource as FrameworkElement).DataContext is AlbumViewModel album)
            {
                SelectedAlbum = album;
                AlbumFlyout.ShowAt(MainGrid, e.GetPosition(MainGrid));
            }
        }

        private void Hyperlink_Click(Hyperlink sender, HyperlinkClickEventArgs args)
        {
            FrameworkElement parent =
                VisualTreeHelper.GetParent(sender) as FrameworkElement;

            if (parent.DataContext is AlbumViewModel album)
            {
                _ = MainPage.Current.ContentFrame.Navigate(typeof(ArtistSongsPage),
                    App.MViewModel.Artists.FirstOrDefault(a => a.Name == album.Artist));
            }

            SelectedAlbum = null;
        }

        private void ShowArtist_Click(object sender, RoutedEventArgs e)
        {
            _ = MainPage.Current.ContentFrame.Navigate(typeof(ArtistSongsPage),
                App.MViewModel.Artists.FirstOrDefault(a => a.Name == SelectedAlbum.Artist));

            SelectedAlbum = null;
        }

        private async void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            Songs.Filter = null;
            Songs.SortDescriptions.Clear();

            if (SelectedAlbum != null)
            {
                Songs.Filter = s => ((SongViewModel)s).Album == SelectedAlbum.Title;
            }
            else
            {
                Songs.SortDescriptions.Add(new SortDescription("Album", CurrentSort));
            }

            Songs.SortDescriptions.Add(new SortDescription("Disc", SortDirection.Ascending));
            Songs.SortDescriptions.Add(new SortDescription("Track", SortDirection.Ascending));

            IEnumerator<object> enumerator = Songs.GetEnumerator();
            List<SongViewModel> songs = new List<SongViewModel>();

            while (enumerator.MoveNext())
            {
                songs.Add(enumerator.Current as SongViewModel);
            }

            enumerator.Dispose();
            await PViewModel.StartMusicPlaybackAsync(songs.GetEnumerator(), 0, songs.Count);
        }

        private async void ShuffleButton_Click(object sender, RoutedEventArgs e)
        {
            Songs.Filter = null;
            if (SelectedAlbum != null)
            {
                Songs.Filter = s => ((SongViewModel)s).Album == SelectedAlbum.Title;
            }

            IEnumerator<object> enumerator = Songs.GetEnumerator();
            List<SongViewModel> songs = new List<SongViewModel>();

            while (enumerator.MoveNext())
            {
                songs.Add(enumerator.Current as SongViewModel);
            }

            enumerator.Dispose();
            await PViewModel.StartMusicPlaybackAsync(songs.GetEnumerator(), 0, songs.Count, true);
        }

        private void SortFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            MenuFlyoutItem item = sender as MenuFlyoutItem;

            Albums.SortDescriptions.Clear();
            string tag = item.Tag.ToString();
            switch (tag)
            {
                case "Ascending":
                    CurrentSort = SortDirection.Ascending;
                    break;

                case "Descending":
                    CurrentSort = SortDirection.Descending;
                    break;

                default:
                    SortProperty = tag;
                    break;
            }

            Albums.SortDescriptions.
                Add(new SortDescription(SortProperty, CurrentSort));
        }
        #endregion
    }
}
