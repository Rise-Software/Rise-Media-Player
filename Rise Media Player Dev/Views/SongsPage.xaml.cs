﻿using RMP.App.Common;
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
        #region Variables
        /// <summary>
        /// Gets the app-wide MViewModel instance.
        /// </summary>
        private MainViewModel MViewModel => App.MViewModel;

        /// <summary>
        /// Gets the app-wide PViewModel instance.
        /// </summary>
        private PlaybackViewModel PViewModel => App.PViewModel;

        private readonly NavigationHelper navigationHelper;
        /// <summary>
        /// Gets the <see cref="NavigationHelper"/> associated with this <see cref="Page"/>.
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return navigationHelper; }
        }

        private readonly static DependencyProperty SelectedSongProperty =
            DependencyProperty.Register("SelectedSong", typeof(SongViewModel), typeof(SongsPage), null);

        private SongViewModel SelectedSong
        {
            get => (SongViewModel)GetValue(SelectedSongProperty);
            set => SetValue(SelectedSongProperty, value);
        }

        private ObservableCollection<SongViewModel> Songs => MViewModel.Songs;

        private SortMethods CurrentMethod = SortMethods.Title;
        private bool DescendingSort { get; set; }
        #endregion

        public SongsPage()
        {
            InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;

            navigationHelper = new NavigationHelper(this);
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

                SelectedSong = null;
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
            => await SelectedSong.StartEdit();

        private async void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            if ((e.OriginalSource as FrameworkElement).DataContext is SongViewModel song)
            {
                int index = MainList.Items.IndexOf(song);
                SelectedSong = null;
                await PViewModel.StartPlayback(Songs, index);
                return;
            }

            await PViewModel.StartPlayback(Songs, 0);
        }

        private async void ShuffleButton_Click(object sender, RoutedEventArgs e)
        {
            SelectedSong = null;
            await PViewModel.StartShuffle(Songs);
        }

        private async void EditButton_Click(object sender, RoutedEventArgs e)
        {
            await SelectedSong.StartEdit();
            SelectedSong = null;
        }

        private void SortFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            MenuFlyoutItem item = sender as MenuFlyoutItem;
            switch (item.Tag.ToString())
            {
                case "Title":
                    CurrentMethod = SortMethods.Title;
                    break;

                case "Track":
                    CurrentMethod = SortMethods.Track;
                    break;

                case "Album":
                    CurrentMethod = SortMethods.Album;
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

        #region NavigationHelper registration
        /// <summary>
        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// Page specific logic should be placed in event handlers for the  
        /// <see cref="NavigationHelper.LoadState"/>
        /// and <see cref="NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method 
        /// in addition to page state preserved during an earlier session.
        /// </summary>
        protected override void OnNavigatedTo(NavigationEventArgs e)
            => navigationHelper.OnNavigatedTo(e);

        protected override void OnNavigatedFrom(NavigationEventArgs e)
            => navigationHelper.OnNavigatedFrom(e);
        #endregion
    }
}
