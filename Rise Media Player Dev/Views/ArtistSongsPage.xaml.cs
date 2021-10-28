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
    public sealed partial class ArtistSongsPage : Page
    {
        /// <summary>
        /// Gets the app-wide MViewModel instance.
        /// </summary>
        public MainViewModel MViewModel => App.MViewModel;

        /// <summary>
        /// Gets the app-wide PViewModel instance.
        /// </summary>
        private PlaybackViewModel PViewModel => App.PViewModel;

        private ArtistViewModel SelectedArtist { get; set; }
        private SongViewModel SelectedSong => MViewModel.SelectedSong;

        private ObservableCollection<SongViewModel> Songs { get; set; }
            = new ObservableCollection<SongViewModel>();

        private SortMethods CurrentMethod = SortMethods.Track;
        private bool DescendingSort { get; set; }

        public ArtistSongsPage()
        {
            InitializeComponent();
            MainPage.Current.CrumbsHeader.Visibility = Visibility.Collapsed;

            DataContext = this;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is ArtistViewModel artist)
            {
                SelectedArtist = artist;

                IEnumerable<SongViewModel> songs =
                    MViewModel.SongsFromArtist(SelectedArtist, MViewModel.Songs, App.SViewModel.FilterByNameOnly);

                Songs.Clear();
                foreach (SongViewModel song in songs)
                {
                    Songs.Add(song);
                }

                RefreshList(SortMethods.Track);
            }

            base.OnNavigatedTo(e);
        }

        #region Event handlers
        private async void MainList_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            if ((e.OriginalSource as FrameworkElement).DataContext is SongViewModel)
            {
                int itemIndex = MainList.SelectedIndex;

                if (itemIndex < 0)
                {
                    return;
                }

                await PViewModel.StartPlayback(Songs, itemIndex);
            }
        }

        private void MainList_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            if ((e.OriginalSource as FrameworkElement).DataContext is SongViewModel song)
            {
                MViewModel.SelectedSong = song;
                SongFlyout.ShowAt(MainList, e.GetPosition(MainList));
            }
        }

        private async void Props_Click(object sender, RoutedEventArgs e)
        {
            await SelectedSong.StartEdit();
        }

        private async void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            if ((e.OriginalSource as FrameworkElement).DataContext is SongViewModel song)
            {
                int index = MainList.Items.IndexOf(song);
                await PViewModel.StartPlayback(Songs, index);
                return;
            }

            await PViewModel.StartPlayback(Songs, 0);
        }

        private async void ShuffleButton_Click(object sender, RoutedEventArgs e)
        {
            await PViewModel.StartShuffle(Songs);
        }

        private async void EditButton_Click(object sender, RoutedEventArgs e)
        {
            await MViewModel.SelectedSong.StartEdit();
        }

        private void SortFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
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

                default:
                    break;
            }

            RefreshList(CurrentMethod);
        }

        private void Descending_Click(object sender, RoutedEventArgs e)
        {
            ToggleMenuFlyoutItem item = sender as ToggleMenuFlyoutItem;
            DescendingSort = item.IsChecked;

            RefreshList(CurrentMethod);
        }
        #endregion

        private void RefreshList(SortMethods method)
        {
            var songs = new ObservableCollection<SongViewModel>
                (App.MViewModel.SortSongs(Songs, method, DescendingSort));

            Songs.Clear();
            foreach (SongViewModel song in songs)
            {
                Songs.Add(song);
            }
        }
    }
}
