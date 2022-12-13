using CommunityToolkit.Mvvm.Input;
using Rise.App.Converters;
using Rise.App.UserControls;
using Rise.App.ViewModels;
using Rise.Common.Constants;
using Rise.Common.Extensions.Markup;
using Rise.Common.Helpers;
using Rise.Data.Json;
using Rise.Data.ViewModels;
using Rise.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Windows.Globalization;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.Web.Http;

namespace Rise.App.Views
{
    public sealed partial class ArtistSongsPage : MediaPageBase
    {
        private JsonBackendController<PlaylistViewModel> PBackend
            => App.MViewModel.PBackend;
        private MainViewModel MViewModel => App.MViewModel;

        private MediaPlaybackViewModel MPViewModel => App.MPViewModel;
        private SettingsViewModel SViewModel => App.SViewModel;

        private readonly MediaCollectionViewModel AlbumsViewModel;

        public static readonly DependencyProperty SelectedAlbumProperty =
            DependencyProperty.Register("SelectedAlbum", typeof(AlbumViewModel),
                typeof(ArtistSongsPage), new PropertyMetadata(null));

        public SongViewModel SelectedItem
        {
            get => (SongViewModel)GetValue(SelectedItemProperty);
            set => SetValue(SelectedItemProperty, value);
        }

        public AlbumViewModel SelectedAlbum
        {
            get => (AlbumViewModel)GetValue(SelectedAlbumProperty);
            set => SetValue(SelectedAlbumProperty, value);
        }

        private ArtistViewModel SelectedArtist;

        // These handle the way artist biography is displayed
        private bool ShowingSummarized = true;
        private string ShortBio;
        private string LongBio;

        public ArtistSongsPage()
            : base("Title", App.MViewModel.Songs, App.MViewModel.Playlists)
        {
            AlbumsViewModel = new("Year", MViewModel.Albums, MViewModel.Songs, MPViewModel);

            InitializeComponent();

            NavigationHelper.LoadState += NavigationHelper_LoadState;
            NavigationHelper.SaveState += NavigationHelper_SaveState;

            PlaylistHelper.AddPlaylistsToSubItem(AddToList, AddSelectedItemToPlaylistCommand);
            PlaylistHelper.AddPlaylistsToFlyout(AddToBar, AddMediaItemsToPlaylistCommand);
            PlaylistHelper.AddPlaylistsToSubItem(AddTo, AddAlbumToPlaylistCommand);
        }

        private async void OnPageLoaded(object sender, RoutedEventArgs e)
        {
            string name = SelectedArtist.Name;
            if (!SViewModel.FetchOnlineData ||
                !WebHelpers.IsInternetAccessAvailable() ||
                name == ResourceHelper.GetString("UnknownArtistResource"))
            {
                VisualStateManager.GoToState(this, "Unavailable", true);
            }
            else
            {
                string genre = await GetGenreFromArtistAsync(name);
                SongAlbums.Text = SongAlbums.Text + ", Genre: " + genre;

                TopTracks.ItemsSource = await GetTopTracksAsync(name);
                NoListeners.Text = await GetMonthlyListenersAsync(name);

                ShortBio = await GetArtistBioAsync(name, true);
                AboutArtist.Text = ShortBio;

                if (!string.IsNullOrWhiteSpace(ShortBio))
                    ArtistAbout.Visibility = Visibility.Visible;
            }
        }

        private void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            if (e.NavigationParameter is Guid id)
            {
                SelectedArtist = App.MViewModel.Artists.
                    FirstOrDefault(a => a.Model.Id == id);

                MediaViewModel.Items.Filter = s => ((SongViewModel)s).Artist == SelectedArtist.Name;
                AlbumsViewModel.Items.Filter = a => ((AlbumViewModel)a).Artist == SelectedArtist.Name;
            }
            else if (e.NavigationParameter is string str)
            {
                SelectedArtist = App.MViewModel.Artists.
                    FirstOrDefault(a => a.Name == str);

                MediaViewModel.Items.Filter = s => ((SongViewModel)s).Artist == str || ((SongViewModel)s).AlbumArtist == str;
            }
        }

        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
            AlbumsViewModel.Dispose();
        }
    }

    // Playlists
    public sealed partial class ArtistSongsPage
    {
        [RelayCommand]
        private Task AddAlbumToPlaylistAsync(PlaylistViewModel playlist)
        {
            var name = SelectedAlbum.Title;
            var items = new List<SongViewModel>();

            foreach (var itm in MViewModel.Songs)
                if (itm.Album == name)
                    items.Add(itm);

            if (playlist == null)
            {
                return PlaylistHelper.CreateNewPlaylistAsync(items);
            }
            else
            {
                playlist.AddItems(items);
                return PBackend.SaveAsync();
            }
        }
    }

    // Event handlers
    public sealed partial class ArtistSongsPage
    {
        private void MainList_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            if ((e.OriginalSource as FrameworkElement).DataContext is SongViewModel song)
                MediaViewModel.PlayFromItemCommand.Execute(song);
        }

        private void MainGrid_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem is AlbumViewModel album && !KeyboardHelpers.IsCtrlPressed())
                _ = Frame.Navigate(typeof(AlbumSongsPage), album.Model.Id);
        }

        private void SongFlyout_Opening(object sender, object e)
        {
            var fl = sender as MenuFlyout;
            var cont = MainList.ItemFromContainer(fl.Target);

            if (cont == null)
                fl.Hide();
            else
                SelectedItem = (SongViewModel)cont;
        }

        private void AlbumsFlyout_Opening(object sender, object e)
        {
            var fl = sender as MenuFlyout;
            var cont = MainGrid.ItemFromContainer(fl.Target);

            if (cont == null)
                fl.Hide();
            else
                SelectedAlbum = (AlbumViewModel)cont;
        }

        private void AskDiscy_Click(object sender, RoutedEventArgs e)
        {
            DiscyOnSong.IsOpen = true;
        }

        private void NavigationView_ItemInvoked(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewItemInvokedEventArgs args)
        {
            if (AlbumsItem.IsSelected)
            {
                MainList.Visibility = Visibility.Visible;
                MainGrid.Visibility = Visibility.Collapsed;
            }
            else if (SongsItem.IsSelected)
            {
                MainList.Visibility = Visibility.Collapsed;
                MainGrid.Visibility = Visibility.Visible;
            }
        }

        private async void ReadMoreAbout_Click(object sender, RoutedEventArgs e)
        {
            if (ShowingSummarized)
            {
                if (string.IsNullOrEmpty(LongBio))
                    LongBio = await GetArtistBioAsync(SelectedArtist.Name, false);

                AboutArtist.Text = LongBio;
                ReadMoreAbout.Content = "Read less";
            }
            else
            {
                AboutArtist.Text = ShortBio;
                ReadMoreAbout.Content = "Read more";
            }

            ShowingSummarized = !ShowingSummarized;
        }

        private void UpDown_Click(object sender, RoutedEventArgs e)
        {
            if (UpDown.Label == "Expand")
                VisualStateManager.GoToState(this, "Expanded", true);
            else
                VisualStateManager.GoToState(this, "Collapsed", true);
        }

        private async Task<List<Track>> GetTopTracksAsync(string artist)
        {
            try
            {
                string lfmUrl = URLs.LastFM + "artist.gettoptracks&artist=" + artist + "&api_key=" + LastFM.Key + "&limit=8";
                using (var client = new HttpClient())
                {
                    var result = await client.TryGetStringAsync(new(lfmUrl));
                    if (result.Succeeded)
                    {
                        StringReader strReader = new(result.Value);
                        XmlSerializer serializer = new(typeof(LFM));
                        XmlTextReader xmlReader = new(strReader);

                        var lfm = (LFM)serializer.Deserialize(xmlReader);
                        return lfm.Toptracks.Track;
                    }
                }
            }
            catch { }

            return null;
        }

        private async Task<string> GetGenreFromArtistAsync(string artist)
        {
            try
            {
                var textInfo = new CultureInfo(ApplicationLanguages.PrimaryLanguageOverride).TextInfo;
                string lfmUrl = URLs.LastFM + "artist.getinfo&artist=" + artist + "&api_key=" + LastFM.Key;

                using (var client = new HttpClient())
                {
                    var res = await client.TryGetStringAsync(new(lfmUrl));
                    if (res.Succeeded)
                    {
                        var doc = new XmlDocument();
                        doc.LoadXml(res.Value);

                        var node = doc.DocumentElement.SelectSingleNode("/lfm/artist/tags/tag/name");
                        if (node != null)
                            return textInfo.ToTitleCase(node.InnerText);
                    }
                }
            }
            catch { }

            return ResourceHelper.GetString("UnknownGenreResource");
        }

        private async Task<string> GetArtistBioAsync(string artist, bool summarized)
        {
            try
            {
                var textInfo = new CultureInfo(ApplicationLanguages.PrimaryLanguageOverride).TextInfo;
                string lfmUrl = URLs.LastFM + "artist.getinfo&artist=" + artist + "&api_key=" + LastFM.Key + "&autocorrect=1";

                using (var client = new HttpClient())
                {
                    var res = await client.TryGetStringAsync(new(lfmUrl));
                    if (res.Succeeded)
                    {
                        var doc = new XmlDocument();
                        doc.LoadXml(res.Value);

                        XmlNode node;
                        if (summarized)
                            node = doc.DocumentElement.SelectSingleNode("/lfm/artist/bio/summary");
                        else
                            node = doc.DocumentElement.SelectSingleNode("/lfm/artist/bio/content");

                        if (node != null)
                        {
                            var url = doc.DocumentElement.SelectSingleNode("/lfm/artist/url");
                            var noUrl = node.InnerText.
                                Replace("<a href=\"" + url.InnerText + "\">Read more on Last.fm</a>", string.Empty);

                            if (summarized)
                                return noUrl;

                            return noUrl.Replace(". User-contributed text is available under the Creative Commons By-SA License; additional terms may apply.", string.Empty);
                        }
                    }
                }
            }
            catch { }

            return "No artist info.";
        }

        private async Task<string> GetMonthlyListenersAsync(string artist)
        {
            try
            {
                var textInfo = new CultureInfo(ApplicationLanguages.PrimaryLanguageOverride).TextInfo;
                string lfmUrl = URLs.LastFM + "artist.getinfo&artist=" + artist + "&api_key=" + LastFM.Key;

                using (var client = new HttpClient())
                {
                    var res = await client.TryGetStringAsync(new(lfmUrl));
                    if (res.Succeeded)
                    {
                        var doc = new XmlDocument();
                        doc.LoadXml(res.Value);

                        var node = doc.DocumentElement.SelectSingleNode("/lfm/artist/stats/listeners");
                        if (node != null && long.TryParse(node.InnerText, out long num))
                            return $"{FormatNumber.Format(num)} listeners.";
                    }
                }
            }
            catch { }

            return string.Empty;
        }
    }
}
