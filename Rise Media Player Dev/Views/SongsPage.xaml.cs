using RMP.App.ViewModels;
using System.Collections.ObjectModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using static RMP.App.Common.Enums;

namespace RMP.App.Views
{
    public sealed partial class SongsPage : Page
    {
        /// <summary>
        /// Gets the app-wide MViewModel instance.
        /// </summary>
        private MainViewModel MViewModel => App.MViewModel;

        /// <summary>
        /// Gets the app-wide PViewModel instance.
        /// </summary>
        private PlaybackViewModel PViewModel => App.PViewModel;

        private SongViewModel SelectedSong { get; set; }
        private ObservableCollection<SongViewModel> Songs => MViewModel.Songs;

        private SortMethods CurrentMethod = SortMethods.Title;
        private bool DescendingSort { get; set; }

        public SongsPage()
        {
            InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;

            RefreshList(CurrentMethod);
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
                SelectedSong = song;
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

        private void SortFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            MenuFlyoutItem item = sender as MenuFlyoutItem;
            switch (item.Tag.ToString())
            {
                case "Track":
                    CurrentMethod = SortMethods.Default;
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
