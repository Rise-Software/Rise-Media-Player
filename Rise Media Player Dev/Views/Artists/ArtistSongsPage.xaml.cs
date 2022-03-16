using Microsoft.Toolkit.Uwp.UI;
using Microsoft.Toolkit.Uwp.UI.Animations;
using Rise.App.Converters;
using Rise.App.ViewModels;
using Rise.Common.Constants;
using Rise.Common.Extensions;
using Rise.Common.Helpers;
using Rise.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
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
        private string SortName = "Title";

        private SortDirection CurrentSort = SortDirection.Ascending;
        #endregion

        public ArtistSongsPage()
        {
            InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;

            DataContext = this;
            Current = this;

            _navigationHelper = new(this);
            _navigationHelper.LoadState += NavigationHelper_LoadState;
            _navigationHelper.SaveState += NavigationHelper_SaveState;

            Loaded += ArtistSongsPage_Loaded;
        }
        
        private async void ArtistSongsPage_Loaded(object sender, RoutedEventArgs e)
        {
            SortName = "Title";
            SortButton.Label = "Sort by: " + SortName;
            string name = SelectedArtist.Name;
            bool isNetworkConnected = NetworkInterface.GetIsNetworkAvailable();

            if (name == "Unknown Artist")
            {
                ArtistAbout.Visibility = Visibility.Collapsed;
                LastFMClickables.Visibility = Visibility.Collapsed;
            }
            else if (!isNetworkConnected)
            {
                ArtistAbout.Visibility = Visibility.Collapsed;
                LastFMClickables.Visibility = Visibility.Collapsed;
            }
            else
            {
                ArtistAbout.Visibility = Visibility.Visible;
                LastFMClickables.Visibility = Visibility.Visible;
                try
                {
                    string genre = GetGenre(name);
                    if (SongAlbums.Text.Contains(genre)) { }
                    else
                    {
                        SongAlbums.Text = SongAlbums.Text + ", Genre: " + genre;
                    }

                    ReadMoreAbout.Content = "Read more";
                    TopTracksLis.ItemsSource = await Task.Run(() => GetTopTracks(name));
                    
                }
                catch { }
            }

        }
        public string GetAlbumImage(string track)
        {
            try
            {
                string m_strFilePath = URLs.Deezer + "/search/track/?q=" + track + "&output=xml&limit=1";
                string xmlStr;
                WebClient wc = new();
                xmlStr = wc.DownloadString(m_strFilePath);
                xmlDoc.LoadXml(xmlStr);

                XmlNode node = xmlDoc.DocumentElement.SelectSingleNode("/root/data/track/album/cover_medium");
                if (node != null)
                {
                    string yes = node.InnerText.Replace("<![CDATA[ ", "").Replace(" ]]>", "");
                    return yes;
                }
            }
            catch (Exception)
            {

            }
            return URIs.MusicThumb;
        }

        public Task<List<TopTracks>> GetTopTracks(string artist)
        {
            LFM lfm = null;
            string m_strFilePath = URLs.LastFM + "artist.gettoptracks&artist=" + artist + "&api_key=" + LastFM.key + "&limit=8";
            string xmlStr;
            WebClient wc = new();
            xmlStr = wc.DownloadString(m_strFilePath);
            StringReader stringReader = new(xmlStr);
            XmlSerializer xs = new(typeof(LFM));
            XmlTextReader xmlReader = new(stringReader);
            lfm = (LFM)xs.Deserialize(xmlReader);
            List<Track> track = lfm.Toptracks.Track;
            List<TopTracks> tracks = new();
            foreach (Track trackname in track)
            {
                string imgurl = GetAlbumImage(trackname.Name);
                tracks.Add(
                    new TopTracks(
                        trackname.Name,
                        trackname.Artist.Name,
                        imgurl
                    ));
            }
            return Task.FromResult(tracks);
        }

        /// <summary>
        /// Task to get description about artist.
        /// </summary>
        /// <param name="artist">Artist name.</param>
        /// <returns></returns>
        public string GetArtistInfo(string artist)
        {
            try
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
                return okay;
            }
            catch
            {

            }

            return "No artist info";
        }

        /// <summary>
        /// Task to get the genre of the artist.
        /// </summary>
        /// <param name="artist">Artist name.</param>
        /// <returns></returns>
        public string GetGenre(string artist)
        {
            try
            {
                TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
                string m_strFilePath = URLs.LastFM + "artist.getinfo&artist=" + artist + "&api_key=" + LastFM.key;
                string xmlStr;
                WebClient wc = new();
                xmlStr = wc.DownloadString(m_strFilePath);
                xmlDoc.LoadXml(xmlStr);
                XmlNode node = xmlDoc.DocumentElement.SelectSingleNode("/lfm/artist/tags/tag/name");
                return node != null ? textInfo.ToTitleCase(node.InnerText) : "Unknown Genre";
            }
            catch
            {

            }

            return "Unknown Genre";
        }

        public string GetArtistInfoBig(string artist)
        {
            try
            {
                string m_strFilePath = URLs.LastFM + "artist.getinfo&artist=" + artist + "&api_key=" + LastFM.key + "&autocorrect=1";
                string xmlStr;
                WebClient wc = new();
                xmlStr = wc.DownloadString(m_strFilePath);
                xmlDoc.LoadXml(xmlStr);
                XmlNode node = xmlDoc.DocumentElement.SelectSingleNode("/lfm/artist/url");
                string url = node.InnerText;
                node = xmlDoc.DocumentElement.SelectSingleNode("/lfm/artist/bio/content");
                string okay = node.InnerText.Replace("<a href=\"" + url + "\">Read more on Last.fm</a>", "").Replace(". User-contributed text is available under the Creative Commons By-SA License; additional terms may apply.", "");
                return okay;
            }
            catch
            {

            }

            return "No artist info";
        }

        public string GetMonthlyListeners(string artist)
        {
            try
            {
                string m_strFilePath = URLs.LastFM + "artist.getinfo&artist=" + artist + "&api_key=" + LastFM.key;
                string xmlStr;
                WebClient wc = new();
                xmlStr = wc.DownloadString(m_strFilePath);
                xmlDoc.LoadXml(xmlStr);
                XmlNode node = xmlDoc.DocumentElement.SelectSingleNode("/lfm/artist/stats/listeners");
                _ = long.TryParse(node.InnerText, out long num);
                return $"{FormatNumber.Format(num)} listeners.";
            }
            catch
            {

            }

            return "";
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
            if (e.NavigationParameter is Guid id)
            {
                SelectedArtist = App.MViewModel.Artists.
                    FirstOrDefault(a => a.Model.Id == id);

                Songs.Filter = s => ((SongViewModel)s).Artist == SelectedArtist.Name;
                Albums.Filter = a => ((AlbumViewModel)a).Artist == SelectedArtist.Name;

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

        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
            Frame.SetListDataItemForNextConnectedAnimation(SelectedArtist);
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
                    SortName = "Track number";
                    break;

                case "Title":
                    SortProperty = tag;
                    SortName = "Title";
                    break;

                case "Genres":
                    SortProperty = tag;
                    SortName = "Genre";
                    break;

                case "Year":
                    SortProperty = tag;
                    SortName = "Year";
                    break;

                default:
                    SortProperty = tag;
                    break;
            }

            Songs.SortDescriptions.
                Add(new SortDescription(SortProperty, CurrentSort));
            SortButton.Label = "Sort by: " + SortName;
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
                AlbumFlyout.ShowAt(AlbumsGrid, e.GetPosition(AlbumsGrid));
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

        private void Album_Click(Hyperlink sender, RoutedEventArgs args)
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

        protected override void OnNavigatedFrom(NavigationEventArgs e)
            => _navigationHelper.OnNavigatedFrom(e);
        #endregion

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

        private void Root_Loaded(object sender, RoutedEventArgs e)
        {
            AddToCommand.Items.Clear();

            MenuFlyoutItem newCommandPlaylistItem = new()
            {
                Text = "New playlist",
                Icon = new FontIcon
                {
                    Glyph = "\uE93F",
                    FontFamily = new Windows.UI.Xaml.Media.FontFamily("ms-appx:///Assets/MediaPlayerIcons.ttf#Media Player Fluent Icons")
                }
            };

            newCommandPlaylistItem.Click += NewCommandPlaylistItem_Click;

            AddToCommand.Items.Add(newCommandPlaylistItem);

            if (App.MViewModel.Playlists.Count > 0)
            {
                AddToCommand.Items.Add(new MenuFlyoutSeparator());
            }

            foreach (PlaylistViewModel playlist in App.MViewModel.Playlists)
            {
                MenuFlyoutItem commanditem = new()
                {
                    Text = playlist.Title,
                    Icon = new FontIcon
                    {
                        Glyph = "\uE93F",
                        FontFamily = new Windows.UI.Xaml.Media.FontFamily("ms-appx:///Assets/MediaPlayerIcons.ttf#Media Player Fluent Icons")
                    },
                    Tag = playlist
                };

                commanditem.Click += CommandItem_Click;

                AddToCommand.Items.Add(commanditem);
            }



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

        private async void NewCommandPlaylistItem_Click(object sender, RoutedEventArgs e)
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
                if (MViewModel.Songs[i].Artist == SelectedArtist.Name)
                {
                    songs.Add(MViewModel.Songs[i]);
                }
            }

            // This will automatically save the playlist to the db
            await playlist.AddSongsAsync(songs);
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

        private void ReadMoreAbout_Click(object sender, RoutedEventArgs e)
        {
            string AboutArtistBig = GetArtistInfoBig(SelectedArtist.Name);
            string artist = GetArtistInfo(SelectedArtist.Name);
            string currentinfo = AboutArtist.Text.ToString();
            if (currentinfo == artist)
            {
                AboutArtist.Text = AboutArtistBig;
                ReadMoreAbout.Content = "Read less";
            }
            else
            {
                AboutArtist.Text = artist;
                ReadMoreAbout.Content = "Read more";
            }
        }

        private async void CommandItem_Click(object sender, RoutedEventArgs e)
        {
            List<SongViewModel> songs = new();
            PlaylistViewModel playlist = (sender as MenuFlyoutItem).Tag as PlaylistViewModel;

            for (int i = 0; i < MViewModel.Songs.Count; i++)
            {
                if (MViewModel.Songs[i].Artist == SelectedArtist.Name)
                {
                    songs.Add(MViewModel.Songs[i]);
                }
            }

            await playlist.AddSongsAsync(songs);
        }

        private void HyperlinkButton_Click_1(Hyperlink sender, RoutedEventArgs e)
           => EventsLogic.GoToAlbum(sender);

        private async void PropsHover_Click(object sender, RoutedEventArgs e)
        {
            if ((e.OriginalSource as FrameworkElement).DataContext is SongViewModel song)
            {
                SelectedSong = song;
                await SelectedSong.StartEditAsync();
            }
        }

        private async void PlayAlbumButton_Click(object sender, RoutedEventArgs e)
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

        private async void ShuffleAlbumButton_Click(object sender, RoutedEventArgs e)
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

        private async void PropsAlbum_Click(object sender, RoutedEventArgs e)
        {
            await SelectedAlbum.StartEditAsync();
        }

        private void UpDown_Click(object sender, RoutedEventArgs e)
        {
            if (UpDown.Label == "Expand")
            {
                UpDownIcon.Glyph = "\uE010";
                SortButton.Visibility = Visibility.Visible;
                UpDown.Label = "Collapse";
                Discography.MaxHeight = 50000;
            }
            else
            {
                UpDownIcon.Glyph = "\uE011";
                SortButton.Visibility = Visibility.Collapsed;
                UpDown.Label = "Expand";
                Discography.MaxHeight = 18;
            }
        }

        private async void CustomisePage_Click(object sender, RoutedEventArgs e)
        {
            ContentDialog dialog = new ContentDialog();

            dialog.Title = "Customise this page";
            dialog.PrimaryButtonText = "Save changes";
            dialog.CloseButtonText = "Cancel";

            dialog.DefaultButton = ContentDialogButton.Primary;
            dialog.Content = new Dialogs.LibraryPageEditDialog();

            var result = await dialog.ShowAsync();
        }
    }
}
