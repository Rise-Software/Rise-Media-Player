using Microsoft.Toolkit.Uwp.UI;
using Rise.App.Common;
using Rise.App.ViewModels;
using Rise.Common.Constants;
using Rise.Common.Helpers;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Xml;
using Windows.Storage;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace Rise.App.Views
{
    public sealed partial class ArtistSongsPage : Page
    {
        #region Variables

        public static ArtistSongsPage Current;


        XmlDocument xmlDoc = new();
        /// <summary>
        /// Gets the app-wide MViewModel instance.
        /// </summary>
        public MainViewModel MViewModel => App.MViewModel;

        /// <summary>
        /// Gets the app-wide PViewModel instance.
        /// </summary>
        private PlaybackViewModel PViewModel => App.PViewModel;

        /// <summary>
        /// Gets the <see cref="NavigationHelper"/> associated with this <see cref="Page"/>.
        /// </summary>
        private readonly NavigationHelper _navigationHelper;

        private static readonly DependencyProperty SelectedArtistProperty =
            DependencyProperty.Register("SelectedArtist", typeof(ArtistViewModel), typeof(ArtistSongsPage), null);

        private ArtistViewModel SelectedArtist
        {
            get => (ArtistViewModel)GetValue(SelectedArtistProperty);
            set => SetValue(SelectedArtistProperty, value);
        }

        private SongViewModel _song;
        public SongViewModel SelectedSong
        {
            get => MViewModel.SelectedSong;
            set => MViewModel.SelectedSong = value;
        }

        private AdvancedCollectionView Songs => MViewModel.FilteredSongs;
        private AdvancedCollectionView Albums => MViewModel.FilteredAlbums;

        private string SortProperty = "Title";
        private SortDirection CurrentSort = SortDirection.Ascending;
        private bool SongsVisible = false, AlbumsVisible = true;
        #endregion

        public ArtistSongsPage()
        {
            InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;

            DataContext = this;
            Current = this;
            _navigationHelper = new NavigationHelper(this);
            _navigationHelper.LoadState += NavigationHelper_LoadState;
            Loaded += ArtistSongsPage_Loaded;
        }
        public string name;
        private async void ArtistSongsPage_Loaded(object sender, RoutedEventArgs e)
        {
            name = ArtistName.Text;
            if (ArtistName.Text == "Unknown Artist")
            {
                AboutArtist.Visibility = Visibility.Collapsed;
            }
            else
            {
                AboutArtist.Visibility = Visibility.Visible;
                try
                {
                    string artist = await Task.Run(() => GetArtistInfo(name));
                    if (AboutArtist.Text == artist) { }
                    else
                    {
                        AboutArtist.Text = artist;
                    }
                    string genre = await Task.Run(() => GetGenre(name));
                    if (SongAlbums.Text.Contains(genre)) { }
                    else
                    {
                        SongAlbums.Text = SongAlbums.Text + ", Genre: " + genre;
                    }
                }
                catch { }
            }
        } /// <summary>
          /// Task to get description about artist.
          /// </summary>
          /// <param name="artist">Artist name.</param>
          /// <returns></returns>
        public Task<string> GetArtistInfo(string artist)
        {
            string m_strFilePath = URLs.LastFM + "artist.getinfo&artist=" + artist + "&api_key=" + LastFM.key;
            string xmlStr;
            WebClient wc = new();
            xmlStr = wc.DownloadString(m_strFilePath);
            xmlDoc.LoadXml(xmlStr);
            XmlNode node = xmlDoc.DocumentElement.SelectSingleNode("/lfm/artist/url");
            string url = node.InnerText;
            node = xmlDoc.DocumentElement.SelectSingleNode("/lfm/artist/bio/summary");
            string okay = node.InnerText.Replace("<a href=\"" + url + "\">Read more on Last.fm</a>", "");
            return Task.FromResult(okay);
        }

        /// <summary>
        /// Task to get the genre of the artist.
        /// </summary>
        /// <param name="artist">Artist name.</param>
        /// <returns></returns>
        public string GetGenre(string artist)
        {
            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
            string m_strFilePath = URLs.LastFM + "artist.getinfo&artist=" + artist + "&api_key=" + LastFM.key;
            string xmlStr;
            WebClient wc = new();
            xmlStr = wc.DownloadString(m_strFilePath);
            xmlDoc.LoadXml(xmlStr);
            XmlNode node = xmlDoc.DocumentElement.SelectSingleNode("/lfm/artist/tags/tag/name");
            if (node != null)
            {
                return textInfo.ToTitleCase(node.InnerText);
            }
            return "Unknown Genre";
        }
        public Task<string> GetArtistInfoBig(string artist)
        {
            string m_strFilePath = URLs.LastFM + "artist.getinfo&artist=" + artist + "&api_key=" + LastFM.key;
            string xmlStr;
            WebClient wc = new();
            xmlStr = wc.DownloadString(m_strFilePath);
            xmlDoc.LoadXml(xmlStr);
            XmlNode node = xmlDoc.DocumentElement.SelectSingleNode("/lfm/artist/url");
            string url = node.InnerText;
            node = xmlDoc.DocumentElement.SelectSingleNode("/lfm/artist/bio/content");
            string okay = node.InnerText.Replace("<a href=\"" + url + "\">Read more on Last.fm</a>", "").Replace(". User-contributed text is available under the Creative Commons By-SA License; additional terms may apply.", "");
            return Task.FromResult(okay);
        }

        private static readonly DependencyProperty SelectedAlbumProperty =
            DependencyProperty.Register("SelectedAlbum", typeof(AlbumViewModel), typeof(AlbumSongsPage), null);

        private AlbumViewModel SelectedAlbum
        {
            get => (AlbumViewModel)GetValue(SelectedAlbumProperty);
            set => SetValue(SelectedAlbumProperty, value);
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

            if (e.NavigationParameter is ArtistViewModel artist)
            {
                SelectedArtist = artist;
                Songs.Filter = s => ((SongViewModel)s).Artist == artist.Name;

                Albums.Filter = a => ((AlbumViewModel)a).Artist == artist.Name;
                Albums.SortDescriptions.Clear();
                Albums.SortDescriptions.Add(new SortDescription("Year", SortDirection.Ascending));
            }
            else if (e.NavigationParameter is string str)
            {
                SelectedArtist = App.MViewModel.Artists.FirstOrDefault(a => a.Name == str);
                Songs.Filter = s => ((SongViewModel)s).Artist == str
                    || ((SongViewModel)s).AlbumArtist == str;

            }

            Songs.SortDescriptions.Clear();
            Songs.SortDescriptions.Add(new SortDescription("Title", SortDirection.Ascending));
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

        private void AskDiscy_Click(object sender, RoutedEventArgs e)
        {
            DiscyOnSong.IsOpen = true;
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

        private void ShowAlbum_Click(object sender, RoutedEventArgs e)
            => _ = Frame.Navigate(typeof(AlbumSongsPage), SelectedSong.Album);

        private async void EditButton_Click(object sender, RoutedEventArgs e)
        {
            await SelectedSong.StartEditAsync();
            SelectedSong = null;
        }

        private void SortFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            MenuFlyoutItem item = sender as MenuFlyoutItem;
            Songs.SortDescriptions.Clear();

            string tag = item.Tag.ToString();
            switch (tag)
            {
                case "Ascending":
                    CurrentSort = SortDirection.Ascending;
                    break;

                case "Descending":
                    CurrentSort = SortDirection.Descending;
                    break;

                case "Track":
                    Songs.SortDescriptions.
                        Add(new SortDescription("Disc", CurrentSort));
                    SortProperty = tag;
                    break;

                default:
                    SortProperty = tag;
                    break;
            }

            Songs.SortDescriptions.
                Add(new SortDescription(SortProperty, CurrentSort));
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

        private void MainList_PointerWheelChanged(object sender, PointerRoutedEventArgs e)
        {
            // Check if the list view height is larger than this page's height, if this is true,
            // try to handle scrolling
            if (MainList.Height > Height)
            {
                (MainList.HeaderTemplate.GetChildren<Border>().FirstOrDefault().Background as ImageBrush).Opacity = MainList.ActualOffset.Y;
            }
        }
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

        private void NavigationView_ItemInvoked(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewItemInvokedEventArgs args)
        {
            if (SongsItem.IsSelected)
            {
                MainList.Visibility = Visibility.Visible;
                AlbumsGrid.Visibility = Visibility.Collapsed;
            }
            else if (AlbumsItem.IsSelected)
            {
                MainList.Visibility = Visibility.Collapsed;
                AlbumsGrid.Visibility = Visibility.Visible;
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
            => _navigationHelper.OnNavigatedFrom(e);
        #endregion
        private void Root_Loaded(object sender, RoutedEventArgs e)
        {
            AddTo.Items.Clear();

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

        private async void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            //string name = ArtistName.Text;
            //string AboutArtistBig = await Task.Run(() => GetArtistInfoBig(name));
            //ContentDialog aboutArtist = new ContentDialog();
            //    aboutArtist.Content = new Dialogs.AboutArtist(AboutArtistBig);
            //aboutArtist.CloseButtonText = "Close";
            //await aboutArtist.ShowAsync();
            string name = ArtistName.Text;
            string AboutArtistBig = await Task.Run(() => GetArtistInfoBig(name));
            ContentDialog aboutArtist = new()
            {
                Title = name,
                Content = new ScrollViewer()
                {
                    Content = new TextBlock()
                    {
                        Text = AboutArtistBig,
                        TextWrapping = TextWrapping.WrapWholeWords
                    },
                },
                CloseButtonText = "Close"
            };
            await aboutArtist.ShowAsync();
        }
    }
}
