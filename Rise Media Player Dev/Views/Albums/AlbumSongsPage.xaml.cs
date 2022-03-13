using Microsoft.Toolkit.Uwp.UI;
using Microsoft.Toolkit.Uwp.UI.Animations;
using Rise.App.ViewModels;
using Rise.Common.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Windows.Storage;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

namespace Rise.App.Views
{
    /// <summary>
    /// Contains the songs inside an album.
    /// </summary>
    public sealed partial class AlbumSongsPage : Page
    {
        #region Variables
        /// <summary>
        /// Gets the app-wide MViewModel instance.
        /// </summary>
        public MainViewModel MViewModel => App.MViewModel;

        /// <summary>
        /// Gets the <see cref="NavigationHelper"/> associated with this <see cref="Page"/>.
        /// </summary>
        private readonly NavigationHelper _navigationHelper;

        private static readonly DependencyProperty SelectedAlbumProperty =
            DependencyProperty.Register("SelectedAlbum", typeof(AlbumViewModel), typeof(AlbumSongsPage), null);

        private AlbumViewModel SelectedAlbum
        {
            get => (AlbumViewModel)GetValue(SelectedAlbumProperty);
            set => SetValue(SelectedAlbumProperty, value);
        }

        public bool HasMoreAlbumsByArtist;

        private SongViewModel _song;
        public SongViewModel SelectedSong
        {
            get => MViewModel.SelectedSong;
            set => MViewModel.SelectedSong = value;
        }

        private AdvancedCollectionView Songs => MViewModel.FilteredSongs;
        private AdvancedCollectionView Albums => MViewModel.FilteredAlbums;
        private AdvancedCollectionView AlbumsByArtist = new();

        #endregion

        public AlbumSongsPage()
        {
            InitializeComponent();

            DataContext = this;
            NavigationCacheMode = NavigationCacheMode.Enabled;

            _navigationHelper = new NavigationHelper(this);
            _navigationHelper.LoadState += NavigationHelper_LoadState;
            _navigationHelper.SaveState += NavigationHelper_SaveState;
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="sender">
        /// The source of the event; typically <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Event data that provides both the navigation parameter passed to
        /// <see cref="Frame.Navigate(Type, object)"/> when this page was initially requested and
        /// a dictionary of state preserved by this page during an earlier
        /// session.  The state will be null the first time a page is visited.</param>
        private void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            if (e.NavigationParameter is Guid id)
            {
                SelectedAlbum = App.MViewModel.Albums.
                    FirstOrDefault(a => a.Model.Id == id);

                Songs.Filter = s => ((SongViewModel)s).Album == SelectedAlbum.Title;

                // TODO: Get "more album from this artist" to work.
                FindAlbumsByArtist(SelectedAlbum.Artist);
            }
            else if (e.NavigationParameter is string str)
            {
                SelectedAlbum = App.MViewModel.Albums.FirstOrDefault(a => a.Title == str);
                Songs.Filter = s => ((SongViewModel)s).Album == str;
            }

            Songs.SortDescriptions.Clear();
            Songs.SortDescriptions.Add(new SortDescription("Disc", SortDirection.Ascending));
            Songs.SortDescriptions.Add(new SortDescription("Track", SortDirection.Ascending));
        }

        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
            Frame.SetListDataItemForNextConnectedAnimation(SelectedAlbum);
        }

        private void AskDiscy_Click(object sender, RoutedEventArgs e)
        {
            DiscyOnSong.IsOpen = true;
        }

        private async void PropsHover_Click(object sender, RoutedEventArgs e)
        {
            if ((e.OriginalSource as FrameworkElement).DataContext is SongViewModel song)
            {
                SelectedSong = song;
                await SelectedSong.StartEditAsync();
            }
        }

        private void FindAlbumsByArtist(string artist)
        {
            if (Albums.Count > 0)
            {
                AlbumsByArtist.Clear();
                HasMoreAlbumsByArtist = true;

                try
                {
                    foreach (AlbumViewModel album in Albums)
                    {
                        if (album.Artist == artist && !album.Equals(SelectedAlbum))
                        {
                            AlbumsByArtist.Add(album);
                        }
                    }
                }
                finally
                {
                    AlbumsByArtist.SortDescriptions.Clear();
                    AlbumsByArtist.SortDescriptions.Add(new SortDescription("Year", SortDirection.Descending));
                    AlbumsByArtist.Refresh();
                }
            }
            else
            {
                
            }
        }

        #region Event handlers
        private async void MainList_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            if ((e.OriginalSource as FrameworkElement).DataContext is SongViewModel song)
            {
                int index = MainList.Items.IndexOf(song);
                await EventsLogic.StartMusicPlaybackAsync(index);
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
            => await SelectedSong.StartEditAsync();

        private void ShowArtist_Click(object sender, RoutedEventArgs e)
            => _ = Frame.Navigate(typeof(ArtistSongsPage), SelectedSong.Artist);

        private async void EditButton_Click(object sender, RoutedEventArgs e)
        {
            await SelectedSong.StartEditAsync();
            SelectedSong = null;
        }

        private void GridView_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if ((e.OriginalSource as FrameworkElement).DataContext is AlbumViewModel album)
            {
                _ = Frame.Navigate(typeof(AlbumSongsPage), album.Model.Id);
            }
        }

        private void MainGrid_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            if ((e.OriginalSource as FrameworkElement).DataContext is AlbumViewModel album)
            {
                SelectedAlbum = album;
            }
        }
        #endregion

        #region Common handlers
        private async void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            if ((e.OriginalSource as FrameworkElement).DataContext is SongViewModel song)
            {
                int index = MainList.Items.IndexOf(song);
                await EventsLogic.StartMusicPlaybackAsync(index);
                return;
            }

            await EventsLogic.StartMusicPlaybackAsync();
        }

        private async void ShuffleButton_Click(object sender, RoutedEventArgs e)
            => await EventsLogic.StartMusicPlaybackAsync(0, true);

        private void Grid_PointerEntered(object sender, PointerRoutedEventArgs e)
            => EventsLogic.FocusSong(ref _song, e);

        private void Grid_PointerExited(object sender, PointerRoutedEventArgs e)
            => EventsLogic.UnfocusSong(ref _song, e);

        private void Album_Click(Hyperlink sender, HyperlinkClickEventArgs args)
            => EventsLogic.GoToAlbum(sender);

        private void Artist_Click(Hyperlink sender, HyperlinkClickEventArgs args)
            => EventsLogic.GoToArtist(sender);
        #endregion

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
            => _navigationHelper.OnNavigatedTo(e);

        protected override void OnNavigatedFrom(NavigationEventArgs e)
            => _navigationHelper.OnNavigatedFrom(e);
        #endregion

        private async void LikeAlbum_Checked(object sender, RoutedEventArgs e)
        {

            List<SongViewModel> songs = new();
            PlaylistViewModel playlist = null;

            try
            {
                playlist = App.MViewModel.Playlists.First(p => p.Title == "Liked");
            }
            catch (InvalidOperationException)
            {

            }

            if (playlist == null)
            {
                playlist = new()
                {
                    Title = $"Liked",
                    Description = "Your liked songs, albums and artists' songs go here.",
                    Icon = "ms-appx:///Assets/NavigationView/PlaylistsPage/blankplaylist.png",
                    Duration = "0"
                };
            }

            for (int i = 0; i < MViewModel.Songs.Count; i++)
            {
                if (MViewModel.Songs[i].Album == SelectedAlbum.Title)
                {
                    songs.Add(MViewModel.Songs[i]);
                }
            }

            // This will automatically save the playlist to the db
            await playlist.AddSongsAsync(songs);
        }

        private async void LikeAlbum_Unchecked(object sender, RoutedEventArgs e)
        {

            List<SongViewModel> songs = new();

            PlaylistViewModel playlist = App.MViewModel.Playlists.First(p => p.Title == "Liked");

            if (playlist == null)
            {
                playlist = new()
                {
                    Title = $"Liked",
                    Description = "Your liked songs, albums and artists' songs go here.",
                    Icon = "ms-appx:///Assets/NavigationView/PlaylistsPage/blankplaylist.png",
                    Duration = "0"
                };
            }

            for (int i = 0; i < MViewModel.Songs.Count; i++)
            {
                if (MViewModel.Songs[i].Album == SelectedAlbum.Title)
                {
                    songs.Add(MViewModel.Songs[i]);
                }
            }

            // This will automatically save the playlist to the db
            await playlist.RemoveSongsAsync(songs);
        }

        private void Root_Loaded(object sender, RoutedEventArgs e)
        {
            AddTo.Items.Clear();
            if (Albums.Count > 0)
            {
                IfAlbumHasMore.Visibility = Visibility.Visible;
            }
            else
            {
                IfAlbumHasMore.Visibility = Visibility.Collapsed;
            }

            MenuFlyoutItem newPlaylistItem = new()
            {
                Text = "New playlist",
                Icon = new FontIcon
                {
                    Glyph = "\uE93F",
                    FontFamily = new Windows.UI.Xaml.Media.FontFamily("ms-appx:///Assets/MediaPlayerIcons.ttf#Media Player Fluent Icons")
                }
            };

            newPlaylistItem.Click += NewPlaylistItem_Click;

            AddTo.Items.Add(newPlaylistItem);

            if (App.MViewModel.Playlists.Count > 0)
            {
                AddTo.Items.Add(new MenuFlyoutSeparator());
            }

            foreach (PlaylistViewModel playlist in App.MViewModel.Playlists)
            {
                MenuFlyoutItem item = new()
                {
                    Text = playlist.Title,
                    Icon = new FontIcon
                    {
                        Glyph = "\uE93F",
                        FontFamily = new Windows.UI.Xaml.Media.FontFamily("ms-appx:///Assets/MediaPlayerIcons.ttf#Media Player Fluent Icons")
                    },
                    Tag = playlist
                };

                item.Click += Item_Click;

                AddTo.Items.Add(item);
            }
        }

        private async void NewPlaylistItem_Click(object sender, RoutedEventArgs e)
        {
            PlaylistViewModel playlist = new()
            {
                Title = $"Untitled Playlist #{App.MViewModel.Playlists.Count + 1}",
                Description = "",
                Icon = "ms-appx:///Assets/NavigationView/PlaylistsPage/blankplaylist.png",
                Duration = "0"
            };

            // This will automatically save the playlist to the db
            await playlist.AddSongAsync(SelectedSong);
        }

        private async void Item_Click(object sender, RoutedEventArgs e)
        {
            PlaylistViewModel playlist = (sender as MenuFlyoutItem).Tag as PlaylistViewModel;
            await playlist.AddSongAsync(SelectedSong);
        }

        private async void ShowinFE_Click(object sender, RoutedEventArgs e)
        {
            string folderlocation = SelectedSong.Location;
            string filename = SelectedSong.Filename;
            string result = folderlocation.Replace(filename, "");
            Debug.WriteLine(result);

            try
            {
                StorageFolder folder = await StorageFolder.GetFolderFromPathAsync(result);
                await Launcher.LaunchFolderAsync(folder);
            }
            catch
            {

            }
        }
    }
}
