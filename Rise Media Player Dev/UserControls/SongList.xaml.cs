using RMP.App.Common;
using RMP.App.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using static RMP.App.Common.Enums;

namespace RMP.App.UserControls
{
    public sealed partial class SongList : UserControl
    {
        #region Variables
        /// <summary>
        /// Gets the app-wide NPViewModel instance.
        /// </summary>
        private static PlaybackViewModel ViewModel => App.PViewModel;

        private SongViewModel SelectedSong { get; set; }
        public AlbumViewModel SelectedAlbum { get; set; }
        public ArtistViewModel SelectedArtist { get; set; }

        public ObservableCollection<SongViewModel> List { get; set; }

        public static SongList Current;
        #endregion

        public SongList()
        {
            InitializeComponent();
            Loaded += SongList_Loaded;

            DataContext = this;
            Current = this;
        }

        #region Commands
        /// <summary>
        /// <see cref="RelayCommand"/> used to bind to the shuffle Button's Command property
        /// to start shuffling songs.
        /// </summary>
        public RelayCommand ShuffleCommand =
            new RelayCommand(async () => await StartShuffle(Current.List));

        /// <summary>
        /// <see cref="RelayCommand"/> used to bind to a ListView item's play button
        /// Command property to start playing songs from that offset.
        /// </summary>
        public RelayCommand PlayCommand =
            new RelayCommand(async () => await StartPlayback(Current.List, 0));

        private RelayCommand _sortDefaultCommand;
        /// <summary>
        /// <see cref="RelayCommand"/> used to bind to a flyout item's
        /// Command property to sort songs based on disc and track (default).
        /// </summary>
        public RelayCommand SortDefaultCommand
        {
            get
            {
                if (_sortDefaultCommand == null)
                {
                    _sortDefaultCommand =
                        new RelayCommand(() => RefreshList());
                }

                return _sortDefaultCommand;
            }
        }

        private RelayCommand _sortTitleCommand;
        /// <summary>
        /// <see cref="RelayCommand"/> used to bind to a flyout item's
        /// Command property to sort songs based on title.
        /// </summary>
        public RelayCommand SortTitleCommand
        {
            get
            {
                if (_sortTitleCommand == null)
                {
                    _sortTitleCommand =
                        new RelayCommand(() => RefreshList(SortMethods.Title));
                }

                return _sortTitleCommand;
            }
        }

        private RelayCommand _sortArtistCommand;
        /// <summary>
        /// <see cref="RelayCommand"/> used to bind to a flyout item's
        /// Command property to sort songs based on artist.
        /// </summary>
        public RelayCommand SortArtistCommand
        {
            get
            {
                if (_sortArtistCommand == null)
                {
                    _sortArtistCommand =
                        new RelayCommand(() => RefreshList(SortMethods.Artist));
                }

                return _sortArtistCommand;
            }
        }

        private RelayCommand _sortGenreCommand;
        /// <summary>
        /// <see cref="RelayCommand"/> used to bind to a flyout item's
        /// Command property to sort songs based on genre.
        /// </summary>
        public RelayCommand SortGenreCommand
        {
            get
            {
                if (_sortGenreCommand == null)
                {
                    _sortGenreCommand =
                        new RelayCommand(() => RefreshList(SortMethods.Genre));
                }

                return _sortGenreCommand;
            }
        }

        private RelayCommand _sortYearCommand;
        /// <summary>
        /// <see cref="RelayCommand"/> used to bind to a flyout item's
        /// Command property to sort songs based on release year.
        /// </summary>
        public RelayCommand SortYearCommand
        {
            get
            {
                if (_sortYearCommand == null)
                {
                    _sortYearCommand =
                        new RelayCommand(() => RefreshList(SortMethods.Year));
                }

                return _sortYearCommand;
            }
        }
        #endregion

        #region Event handlers
        private void SongList_Loaded(object sender, RoutedEventArgs e)
        {
            if (SelectedAlbum != null)
            {
                MainList.HeaderTemplate = Resources["AlbumHeader"] as DataTemplate;

                // ScrollViewer sv = Methods.FindVisualChild<ScrollViewer>(MainList);
                // sv.ViewChanged += ScrollViewerChanged;
            }

            if (SelectedArtist != null)
            {
                MainList.HeaderTemplate = Resources["ArtistHeader"] as DataTemplate;
            }
        }

        private void ScrollViewerChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            ScrollViewer sv = (ScrollViewer)sender;
            var verticalOffset = sv.VerticalOffset;

            if (verticalOffset > 160)
            {
                // Enable the compact header
                Header.Visibility = Visibility.Visible;
            }
            else
            {
                // Disable the compact header
                Header.Visibility = Visibility.Collapsed;
            }
        }

        private async void MainList_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            if ((e.OriginalSource as FrameworkElement).DataContext is SongViewModel song)
            {
                int itemIndex = MainList.SelectedIndex;

                if (itemIndex < 0)
                {
                    return;
                }

                await StartPlayback(List, itemIndex);
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

        private async void ListItemContainer_Click(object sender, RoutedEventArgs e)
        {
            if ((e.OriginalSource as FrameworkElement).DataContext is SongViewModel song)
            {
                int index = MainList.Items.IndexOf(song);
                await StartPlayback(List, index);
            }
        }
        #endregion

        private static async Task StartShuffle(ObservableCollection<SongViewModel> songs)
        {
            Random rng = new Random();
            songs = new ObservableCollection<SongViewModel>(songs.OrderBy(s => rng.Next()));

            ViewModel.CancelTask();
            await ViewModel.CreatePlaybackList(0, songs, ViewModel.Token);
        }

        private static async Task StartPlayback(ObservableCollection<SongViewModel> songs, int startIndex)
        {
            ViewModel.CancelTask();
            await ViewModel.CreatePlaybackList(startIndex, songs, ViewModel.Token);
        }

        private void RefreshList(SortMethods method = SortMethods.Default)
        {
            var songs = new ObservableCollection<SongViewModel>(SortList(method));
            List.Clear();

            foreach (SongViewModel song in songs)
            {
                List.Add(song);
            }
        }

        private IOrderedEnumerable<SongViewModel> SortList(SortMethods method)
        {
            Debug.WriteLine("Sorting...");
            IOrderedEnumerable<SongViewModel> songs;

            switch (method)
            {
                case SortMethods.Title:
                    songs = List.OrderBy(s => s.Title);
                    break;

                case SortMethods.Artist:
                    songs = List.OrderBy(s => s.Artist);
                    break;

                case SortMethods.Genre:
                    songs = List.OrderBy(s => s.Genres);
                    break;

                case SortMethods.Year:
                    songs = List.OrderBy(s => s.Year);
                    break;

                default:
                    songs = List.OrderBy(s => s.Disc).ThenBy(s => s.Track);
                    break;
            }

            return songs;
        }
    }
}
