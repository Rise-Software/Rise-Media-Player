using RMP.App.Common;
using RMP.App.ViewModels;
using RMP.App.Views;
using RMP.App.Windows;
using System.Collections.Generic;
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
    public sealed partial class AlbumGrid : UserControl
    {
        /// <summary>
        /// Gets the app-wide MViewModel instance.
        /// </summary>
        private MainViewModel MViewModel => App.MViewModel;

        /// <summary>
        /// Gets the app-wide MViewModel instance.
        /// </summary>
        private PlaybackViewModel PViewModel => App.PViewModel;

        public ObservableCollection<AlbumViewModel> List { get; set; }
        private SortMethods CurrentMethod = SortMethods.Default;

        public static AlbumGrid Current;

        public AlbumGrid()
        {
            InitializeComponent();
            Current = this;

            Loaded += AlbumGrid_Loaded;
        }

        private void AlbumGrid_Loaded(object sender, RoutedEventArgs e)
        {
            RefreshList(CurrentMethod);
        }

        #region Commands
        private RelayCommand _shuffleCommand;
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
                    _shuffleCommand =
                        new RelayCommand(async () => await StartShuffle());
                }

                return _shuffleCommand;
            }
        }

        private RelayCommand _playCommand;
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
                    _playCommand =
                        new RelayCommand(async () => await StartPlayback());
                }

                return _playCommand;
            }
        }

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

        private RelayCommand _sortNameOnlyCommand;
        /// <summary>
        /// <see cref="RelayCommand"/> used to bind to a flyout item's
        /// Command property to filter albums only based on their title.
        /// </summary>
        public RelayCommand SortNameOnlyCommand
        {
            get
            {
                if (_sortNameOnlyCommand == null)
                {
                    _sortNameOnlyCommand =
                        new RelayCommand(() => RefreshList(CurrentMethod));
                }

                return _sortNameOnlyCommand;
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

        private async Task StartShuffle()
        {
            IEnumerable<SongViewModel> songs;
            if (MViewModel.SelectedAlbum != null)
            {
                songs = MViewModel.SongsFromAlbum(MViewModel.SelectedAlbum, MViewModel.Songs);
                songs = MViewModel.RandomizeSongs(songs);
                await PViewModel.CreatePlaybackList(0, songs, PViewModel.Token);
            }
        }

        private async Task StartPlayback()
        {
            IEnumerable<SongViewModel> songs;
            if (MViewModel.SelectedAlbum != null)
            {
                songs = MViewModel.SongsFromAlbum(MViewModel.SelectedAlbum, MViewModel.Songs);
                await PViewModel.CreatePlaybackList(0, songs, PViewModel.Token);
            }
        }

        private void RefreshList(SortMethods method = SortMethods.Default)
        {
            CurrentMethod = method;
            var albums = new ObservableCollection<AlbumViewModel>(SortList(method));
            
            List.Clear();
            foreach (AlbumViewModel album in albums)
            {
                List.Add(album);
            }
        }

        private IEnumerable<AlbumViewModel> SortList(SortMethods method)
        {
            Debug.WriteLine("Sorting...");
            IEnumerable<AlbumViewModel> albums;

            if (App.SViewModel.FilterByNameOnly)
            {
                albums = MViewModel.Albums.GroupBy(a => a.Model.Title).Select(a => a.First());
            }
            else
            {
                albums = List;
            }

            switch (method)
            {
                case SortMethods.Artist:
                    albums = albums.OrderBy(a => a.Artist);
                    break;

                default:
                    albums = albums.OrderBy(a => a.Title);
                    break;
            }

            return albums;
        }

        private void GridView_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if ((e.OriginalSource as FrameworkElement).DataContext is AlbumViewModel album)
            {
                _ = MainPage.Current.ContentFrame.Navigate(typeof(AlbumSongsPage), album);
                MViewModel.SelectedAlbum = null;
            }
        }
    }
}
