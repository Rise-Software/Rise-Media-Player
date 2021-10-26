using RMP.App.Common;
using RMP.App.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

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
        RelayCommand _playAllCommand;
        /// <summary>
        /// <see cref="RelayCommand"/> used to bind to the play Button's Command property
        /// to start playing songs.
        /// </summary>
        public RelayCommand PlayAllCommand
        {
            get
            {
                if (_playAllCommand == null)
                {
                    _playAllCommand = new RelayCommand(
                        async () => await StartPlayback(List, 0));
                }

                return _playAllCommand;
            }
            set
            {
                _playAllCommand = value;
            }
        }

        RelayCommand _shuffleCommand;
        /// <summary>
        /// <see cref="RelayCommand"/> used to bind to the shuffle Button's Command property
        /// to start shuffling songs.
        /// </summary>
        public RelayCommand ShuffleCommand
        {
            get
            {
                if (_shuffleCommand == null)
                {
                    _shuffleCommand = new RelayCommand(
                        async () => await StartShuffle(List));
                }

                return _shuffleCommand;
            }
            set
            {
                _shuffleCommand = value;
            }
        }

        RelayCommand _playCommand;
        /// <summary>
        /// <see cref="RelayCommand"/> used to bind to a ListView item's play button
        /// Command property to start playing songs from that offset.
        /// </summary>
        public RelayCommand PlayCommand
        {
            get
            {
                if (_playCommand == null)
                {
                    _playCommand = new RelayCommand(
                        async () => await StartPlayback(List, 0));
                }

                return _playCommand;
            }
            set
            {
                _playCommand = value;
            }
        }
        #endregion

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

        private async void MainList_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            int itemIndex = MainList.SelectedIndex;

            if (itemIndex < 0)
            {
                return;
            }

            await StartPlayback(List, itemIndex);
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

        public async Task StartPlayback(ObservableCollection<SongViewModel> songs, int startIndex)
        {
            ViewModel.CancelTask();
            await ViewModel.CreatePlaybackList(startIndex, songs, ViewModel.Token);
        }

        private async Task StartShuffle(ObservableCollection<SongViewModel> songs)
        {
            Random rng = new Random();
            songs = new ObservableCollection<SongViewModel>(songs.OrderBy(s => rng.Next()));

            ViewModel.CancelTask();
            await ViewModel.CreatePlaybackList(0, songs, ViewModel.Token);
        }

        private async void ListItemContainer_Click(object sender, RoutedEventArgs e)
        {
            if ((e.OriginalSource as FrameworkElement).DataContext is SongViewModel song)
            {
                int index = MainList.Items.IndexOf(song);
                await StartPlayback(List, index);
            }
        }
    }
}
