using RMP.App.Settings.ViewModels;
using RMP.App.ViewModels;
using RMP.App.Windows;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using static RMP.App.Common.Enums;

namespace RMP.App.Views
{
    public sealed partial class AlbumsPage : Page
    {
        public static AlbumsPage Current;

        /// <summary>
        /// Gets the app-wide MViewModel instance.
        /// </summary>
        private MainViewModel MViewModel => App.MViewModel;

        private SortMethods SortBy = SortMethods.Default;

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
            IEnumerable<AlbumViewModel> enumerable;
            if (App.SViewModel.FilterByNameOnly)
            {
                enumerable = MViewModel.Albums.GroupBy(a => a.Model.Title).Select(a => a.First());
            }
            else
            {
                enumerable = MViewModel.Albums;
            }

            switch (SortBy)
            {
                case SortMethods.Artist:
                    enumerable = enumerable.OrderBy(a => a.Artist);
                    break;

                default:
                    enumerable = enumerable.OrderBy(s => s.Title);
                    break;
            }

            AlbumGrid.ItemsSource = enumerable;
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

            MViewModel.SortBy = SortMethods.Random;
            await PViewModel.CreatePlaybackList(0, MViewModel.FilteredSongs, PViewModel.Token);
        }

        private void Image_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if ((e.OriginalSource as FrameworkElement).DataContext is AlbumViewModel album)
            {
                _ = MainPage.Current.ContentFrame.Navigate(typeof(AlbumSongsPage), album);
            }
        }

        private void SortFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            var item = (MenuFlyoutItem)sender;
            switch (item.Tag.ToString())
            {
                case "Artist":
                    SortBy = SortMethods.Artist;
                    break;

                default:
                    SortBy = SortMethods.Default;
                    break;
            }

            RefreshAlbums();
        }
    }
}
