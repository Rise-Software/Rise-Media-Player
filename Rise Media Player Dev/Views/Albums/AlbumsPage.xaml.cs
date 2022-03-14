using Microsoft.Toolkit.Uwp.UI;
using Microsoft.Toolkit.Uwp.UI.Animations;
using Microsoft.UI.Xaml.Controls;
using Rise.App.ViewModels;
using Rise.Common.Helpers;
using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

namespace Rise.App.Views
{
    public sealed partial class AlbumsPage : Page
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

        /// <summary>
        /// Gets the app-wide SViewModel instance.
        /// </summary>
        private SettingsViewModel SViewModel => App.SViewModel;

        /// <summary>
        /// Gets the <see cref="NavigationHelper"/> associated with this <see cref="Page"/>.
        /// </summary>
        private readonly NavigationHelper _navigationHelper;

        private readonly string Label = "Albums";

        private static readonly DependencyProperty SelectedAlbumProperty =
            DependencyProperty.Register("SelectedAlbum", typeof(AlbumViewModel), typeof(AlbumsPage), null);

        private AlbumViewModel SelectedAlbum
        {
            get => (AlbumViewModel)GetValue(SelectedAlbumProperty);
            set => SetValue(SelectedAlbumProperty, value);
        }

        private bool IsCtrlPressed;

        private AdvancedCollectionView Albums => MViewModel.FilteredAlbums;
        private AdvancedCollectionView Songs => MViewModel.FilteredSongs;

        public string SortProperty = "Title";
        public SortDirection CurrentSort = SortDirection.Ascending;
        #endregion

        public AlbumsPage()
        {
            InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;

            _navigationHelper = new NavigationHelper(this);
            _navigationHelper.LoadState += NavigationHelper_LoadState;
            Loaded += AlbumsPage_Loaded;
        }

        private void AlbumsPage_Loaded(object sender, RoutedEventArgs e)
        {
            ApplySettingsToView();

            ApplyPlaylistItems(AddTo);
            ApplyPlaylistItems(AddTo123);
        }

        private void ApplyPlaylistItems(MenuFlyout addTo)
        {
            addTo.Items.Clear();

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

            addTo.Items.Add(newPlaylistItem);

            if (App.MViewModel.Playlists.Count > 0)
            {
                addTo.Items.Add(new MenuFlyoutSeparator());
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

                addTo.Items.Add(item);
            }
        }

        private void ApplyPlaylistItems(MenuFlyoutSubItem addTo)
        {
            addTo.Items.Clear();

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

            addTo.Items.Add(newPlaylistItem);

            if (App.MViewModel.Playlists.Count > 0)
            {
                addTo.Items.Add(new MenuFlyoutSeparator());
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

                addTo.Items.Add(item);
            }
        }

        private async void NewPlaylistItem_Click(object sender, RoutedEventArgs e)
        {
            List<SongViewModel> songs = new();

            PlaylistViewModel playlist = new()
            {
                Title = $"Untitled Playlist #{App.MViewModel.Playlists.Count + 1}",
                Description = "",
                Icon = "ms-appx:///Assets/NavigationView/PlaylistsPage/blankplaylist.png",
                Duration = "0"
            };

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

        private async void Item_Click(object sender, RoutedEventArgs e)
        {
            List<SongViewModel> songs = new();
            PlaylistViewModel playlist = (sender as MenuFlyoutItem).Tag as PlaylistViewModel;

            for (int i = 0; i < MViewModel.Songs.Count; i++)
            {
                if (MViewModel.Songs[i].Album == SelectedAlbum.Title)
                {
                    songs.Add(MViewModel.Songs[i]);
                }
            }

            await playlist.AddSongsAsync(songs);
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
            Albums.Filter = null;
            Albums.SortDescriptions.Clear();
            Albums.SortDescriptions.Add(new SortDescription(SortProperty, CurrentSort));
        }

        #region Event handlers
        private void GridView_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (!KeyboardHelpers.IsCtrlPressed())
            {
                if ((e.OriginalSource as FrameworkElement).DataContext is AlbumViewModel album)
                {
                    Frame.SetListDataItemForNextConnectedAnimation(album);
                    _ = Frame.Navigate(typeof(AlbumSongsPage), album.Model.Id);

                    SelectedAlbum = null;
                }
            }
            else
            {
                if ((e.OriginalSource as FrameworkElement).DataContext is AlbumViewModel album)
                {
                    SelectedAlbum = album;
                }
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

        private void ShowArtist_Click(object sender, RoutedEventArgs e)
        {
            _ = Frame.Navigate(typeof(ArtistSongsPage), SelectedAlbum.Artist);
            SelectedAlbum = null;
        }

        private void AskDiscy_Click(object sender, RoutedEventArgs e)
        {
            DiscyOnAlbum.IsOpen = true;
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
            List<SongViewModel> songs = new();

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
            List<SongViewModel> songs = new();

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

        private void Artist_Click(Hyperlink sender, HyperlinkClickEventArgs args)
            => EventsLogic.GoToArtist(sender);

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
        {
            _navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
            => _navigationHelper.OnNavigatedFrom(e);
        #endregion

        private void ApplySettingsToView()
        {
            if (SViewModel.ShowTitleInAlbums)
            {
                foreach (AlbumViewModel album in Albums)
                {
                    album.IsTitleVisible = true;
                }
            }
            else
            {
                foreach (AlbumViewModel album in Albums)
                {
                    album.IsTitleVisible = false;
                }
            }

            if (SViewModel.ShowGenreInAlbums)
            {
                foreach (AlbumViewModel album in Albums)
                {
                    album.IsGenresVisible = true;
                }
            }
            else
            {
                foreach (AlbumViewModel album in Albums)
                {
                    album.IsGenresVisible = false;
                }
            }

            if (SViewModel.ShowArtistInAlbums)
            {
                foreach (AlbumViewModel album in Albums)
                {
                    album.IsArtistVisible = true;
                }
            }
            else
            {
                foreach (AlbumViewModel album in Albums)
                {
                    album.IsArtistVisible = false;
                }
            }

            if (SViewModel.ShowThumbnailInAlbums)
            {
                foreach (AlbumViewModel album in Albums)
                {
                    album.IsThumbnailVisible = true;
                }
            }
            else
            {
                foreach (AlbumViewModel album in Albums)
                {
                    album.IsThumbnailVisible = false;
                }
            }

            if (SViewModel.RoundedAlbumArt)
            {
                foreach (AlbumViewModel album in Albums)
                {
                    album.HasRoundedAlbumArt = true;
                }
            }
            else
            {
                foreach (AlbumViewModel album in Albums)
                {
                    album.HasRoundedAlbumArt = false;
                }
            }

            if (SViewModel.ShowReleaseYearInAlbums)
            {
                foreach (AlbumViewModel album in Albums)
                {
                    album.IsReleaseYearVisible = true;
                }
            }
            else
            {
                foreach (AlbumViewModel album in Albums)
                {
                    album.IsReleaseYearVisible = false;
                }
            }
        }

        private void ShowArtistName_Click(object sender, RoutedEventArgs e)
        {
            ToggleMenuFlyoutItem item = sender as ToggleMenuFlyoutItem;
            if (item.IsChecked)
            {
                foreach (AlbumViewModel album in Albums)
                {
                    album.IsArtistVisible = true;
                }
            }
            else
            {
                foreach (AlbumViewModel album in Albums)
                {
                    album.IsArtistVisible = false;
                }
            }
        }

        private void ShowThumbnail_Click(object sender, RoutedEventArgs e)
        {
            ToggleMenuFlyoutItem item = sender as ToggleMenuFlyoutItem;
            if (item.IsChecked)
            {
                foreach (AlbumViewModel album in Albums)
                {
                    album.IsThumbnailVisible = true;
                }
                TitleViewOption.IsEnabled = true;
            }
            else
            {
                foreach (AlbumViewModel album in Albums)
                {
                    album.IsThumbnailVisible = false;
                }
                TitleViewOption.IsEnabled = false;
                SViewModel.ShowTitleInAlbums = true;
            }
        }

        private void ShowGenres_Click(object sender, RoutedEventArgs e)
        {
            ToggleMenuFlyoutItem item = sender as ToggleMenuFlyoutItem;
            if (item.IsChecked)
            {
                foreach (AlbumViewModel album in Albums)
                {
                    album.IsGenresVisible = true;
                }
            }
            else
            {
                foreach (AlbumViewModel album in Albums)
                {
                    album.IsGenresVisible = false;
                }
            }
        }

        private void RoundedAlbumArtOption_Click(object sender, RoutedEventArgs e)
        {
            ToggleMenuFlyoutItem item = sender as ToggleMenuFlyoutItem;
            if (item.IsChecked)
            {
                foreach (AlbumViewModel album in Albums)
                {
                    album.HasRoundedAlbumArt = true;
                }
            }
            else
            {
                foreach (AlbumViewModel album in Albums)
                {
                    album.HasRoundedAlbumArt = false;
                }
            }
        }

        private void ShowAlbumTitle_Click(object sender, RoutedEventArgs e)
        {
            ToggleMenuFlyoutItem item = sender as ToggleMenuFlyoutItem;
            if (item.IsChecked)
            {
                foreach (AlbumViewModel album in Albums)
                {
                    album.IsTitleVisible = true;
                }
                ThumbnailViewOption.IsEnabled = true;
            }
            else
            {
                foreach (AlbumViewModel album in Albums)
                {
                    album.IsTitleVisible = false;
                }
                ThumbnailViewOption.IsEnabled = false;
                SViewModel.ShowThumbnailInAlbums = true;
            }
        }

        private void ShowReleaseYear_Click(object sender, RoutedEventArgs e)
        {
            ToggleMenuFlyoutItem item = sender as ToggleMenuFlyoutItem;
            if (item.IsChecked)
            {
                foreach (AlbumViewModel album in Albums)
                {
                    album.IsReleaseYearVisible = true;
                }
            }
            else
            {
                foreach (AlbumViewModel album in Albums)
                {
                    album.IsReleaseYearVisible = false;
                }
            }
        }

        private void Page_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            IsCtrlPressed = e.Key == Windows.System.VirtualKey.Control;
        }

        private void ViewMode_Click(object sender, RoutedEventArgs e)
        {
            if (((string)(sender as RadioMenuFlyoutItem).Tag) == "Tiles")
            {
                SViewModel.IsListInAlbumsPage = false;
                SViewModel.IsTilesInAlbumsPage = true;
                System.Diagnostics.Debug.WriteLine("Tiles");
            }
            else if (((string)(sender as RadioMenuFlyoutItem).Tag) == "Lists")
            {
                SViewModel.IsTilesInAlbumsPage = false;
                SViewModel.IsListInAlbumsPage = true;
                System.Diagnostics.Debug.WriteLine("Lists");
            }
        }

        private void Border_PointerEntered(object sender, PointerRoutedEventArgs e)
        {

        }

        private async void AddFolders_Click(object sender, RoutedEventArgs e)
        {
            ContentDialog dialog = new ContentDialog();
            dialog.Title = "Manage local media folders";
            dialog.CloseButtonText = "Close";
            dialog.Content = new Settings.MediaSourcesPage();
            var result = await dialog.ShowAsync();
        }

        private async void Props_Click(object sender, RoutedEventArgs e)
        {
            await SelectedAlbum.StartEditAsync();
        }
    }
}
