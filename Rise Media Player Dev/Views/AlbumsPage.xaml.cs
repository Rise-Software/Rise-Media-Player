using RMP.App.Settings.ViewModels;
using RMP.App.ViewModels;
using RMP.App.Windows;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using static RMP.App.Common.Enums;

namespace RMP.App.Views
{
    public sealed partial class AlbumsPage : Page
    {
        /// <summary>
        /// Gets the app-wide MViewModel instance.
        /// </summary>
        private MainViewModel MViewModel => App.MViewModel;

        /// <summary>
        /// Gets the app-wide PViewModel instance.
        /// </summary>
        private PlaybackViewModel PViewModel => App.PViewModel;

        /// <summary>
        /// Gets the app-wide SViewModel instance.
        /// </summary>
        private SettingsViewModel SViewModel => App.SViewModel;

        private AlbumViewModel SelectedAlbum { get; set; }
        private ObservableCollection<AlbumViewModel> Albums { get; set; }
            = new ObservableCollection<AlbumViewModel>();

        private SortMethods CurrentMethod = SortMethods.Title;
        private bool DescendingSort { get; set; }

        public AlbumsPage()
        {
            InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;

            RefreshList(SortMethods.Title, SViewModel.FilterByNameOnly);
        }

        #region Event handlers
        private void GridView_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if ((e.OriginalSource as FrameworkElement).DataContext is AlbumViewModel album)
            {
                _ = MainPage.Current.ContentFrame.Navigate(typeof(AlbumSongsPage), album);
                SelectedAlbum = null;
            }
        }

        private void MainGrid_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            if ((e.OriginalSource as FrameworkElement).DataContext is AlbumViewModel album)
            {
                SelectedAlbum = album;
                AlbumFlyout.ShowAt(MainGrid, e.GetPosition(MainGrid));
            }
        }

        private async void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedAlbum != null)
            {
                IEnumerable<SongViewModel> songs =
                    MViewModel.SongsFromAlbum(SelectedAlbum, MViewModel.Songs, Merge.IsChecked);
                await PViewModel.StartPlayback(songs, 0);
                return;
            }

            await PViewModel.StartPlayback(MViewModel.
                SortSongs(MViewModel.Songs, SortMethods.Album, DescendingSort), 0);
        }

        private async void ShuffleButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedAlbum != null)
            {
                IEnumerable<SongViewModel> songs =
                    MViewModel.SongsFromAlbum(SelectedAlbum, MViewModel.Songs, Merge.IsChecked);

                await PViewModel.StartPlayback(MViewModel.
                    SortSongs(songs, SortMethods.Random, DescendingSort), 0);
                return;
            }

            await PViewModel.StartPlayback(MViewModel.
                SortSongs(MViewModel.Songs, SortMethods.Random, DescendingSort), 0);
        }

        private void SelectToggleButton_Checked(object sender, RoutedEventArgs e)
            => MainGrid.Tapped -= GridView_Tapped;

        private void SelectToggleButton_Unchecked(object sender, RoutedEventArgs e)
            => MainGrid.Tapped += GridView_Tapped;

        private void SelectItem_Click(object sender, RoutedEventArgs e)
        {
            MainGrid.SelectedItem = SelectedAlbum;
        }

        private void SortFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            SViewModel.FilterByNameOnly = Merge.IsChecked;
            MenuFlyoutItem item = sender as MenuFlyoutItem;
            switch (item.Tag.ToString())
            {
                case "Title":
                    CurrentMethod = SortMethods.Title;
                    break;

                case "Artist":
                    CurrentMethod = SortMethods.Artist;
                    break;

                case "Genre":
                    CurrentMethod = SortMethods.Genre;
                    break;

                case "Year":
                    CurrentMethod = SortMethods.Year;
                    break;

                case "Ascending":
                    DescendingSort = false;
                    break;

                case "Descending":
                    DescendingSort = true;
                    break;

                default:
                    break;
            }

            RefreshList(CurrentMethod, Merge.IsChecked);
        }
        #endregion

        private void RefreshList(SortMethods method, bool mergeAlbums)
        {
            var albums = new ObservableCollection<AlbumViewModel>
                (App.MViewModel.SortAlbums(MViewModel.Albums, method, mergeAlbums, DescendingSort));

            Albums.Clear();
            foreach (AlbumViewModel album in albums)
            {
                Albums.Add(album);
            }
        }

    }
}
