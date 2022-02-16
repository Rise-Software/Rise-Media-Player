using Microsoft.Toolkit.Uwp.UI;
using Rise.App.Common;
using Rise.App.Helpers;
using Rise.App.ViewModels;
using Rise.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Xml;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

namespace Rise.App.Views
{
    public sealed partial class ArtistsPage : Page
    {
        private readonly XmlDocument xmlDoc = new();
        private readonly List<string> artistnames = new();
        private readonly List<string> imagelinks = new();

        /// <summary>
        /// Gets the app-wide MViewModel instance.
        /// </summary>
        private MainViewModel MViewModel => App.MViewModel;

        private AdvancedCollectionView Artists => MViewModel.FilteredArtists;

        private static readonly DependencyProperty SelectedArtistProperty =
            DependencyProperty.Register("SelectedArtist", typeof(ArtistViewModel), typeof(ArtistsPage), null);

        private ArtistViewModel SelectedArtist
        {
            get => (ArtistViewModel)GetValue(SelectedArtistProperty);
            set => SetValue(SelectedArtistProperty, value);
        }

        private static readonly DependencyProperty SelectedArtistItemProperty =
            DependencyProperty.Register("SelectedArtistItem", typeof(ArtistViewModel), typeof(ArtistsPage), null);

        private ArtistViewModel SelectedArtistItem
        {
            get => (ArtistViewModel)GetValue(SelectedArtistItemProperty);
            set => SetValue(SelectedArtistItemProperty, value);
        }

        /// <summary>
        /// Gets the <see cref="NavigationHelper"/> associated with this <see cref="Page"/>.
        /// </summary>
        private readonly NavigationHelper _navigationHelper;

        public ArtistsPage()
        {
            InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;

            _navigationHelper = new NavigationHelper(this);
            Loaded += ArtistsPage_Loaded;
            SetArtistPictures();
        }

        private void ArtistsPage_Loaded(object sender, RoutedEventArgs e)
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
                    songs.Add(songs[i]);
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
                if (MViewModel.Songs[i].Artist == SelectedArtist.Name)
                {
                    songs.Add(songs[i]);
                }
            }

            await playlist.AddSongsAsync(songs);
        }

        private async Task SetArtistPictures()
        {
            string image;
            foreach (ArtistViewModel artist in Artists)
            {
                if (artist.Name != "Unknown Artist")
                {
                    artistnames.Add(artist.Name);
                }
            }
            foreach (string artistname in artistnames)
            {
                image = await Task.Run(() => getartistimg(artistname));
                imagelinks.Add(artistname + " - " + image);
            }
            foreach (string imagel in imagelinks)
            {
                foreach (ArtistViewModel artist in Artists)
                {
                    // Get images from database
                    IEnumerable<Artist> artists = await App.Repository.Artists.GetAsync();
                    StorageFile file = null;

                    try
                    {
                        file = await StorageFile.GetFileFromApplicationUriAsync(new Uri($"ms-appdata:///local/modified-artist-{artist.Name}.png"));
                    } catch (Exception)
                    {

                    }

                    if (artist.Name == "Unknown Artist") 
                    {
                        artist.Picture = "ms-appx:///Assets/BlankArtist.png";
                    }
                    else if (file != null)
                    {
                        artist.Picture = $@"ms-appdata:///local/modified-artist-{artist.Name}.png";
                    }
                    else if (imagel.Contains(artist.Name))
                    {
                        artist.Picture = imagel.Replace(artist.Name + " - ", "");
                    }
                }
            }
        }
        public string getartistimg(string artist)
        {
            try
            {
                string m_strFilePath = URLs.Deezer + "/search/artist/?q=" + artist + "&output=xml";
                string xmlStr;
                WebClient wc = new();
                xmlStr = wc.DownloadString(m_strFilePath);
                xmlDoc.LoadXml(xmlStr);

                XmlNode node = xmlDoc.DocumentElement.SelectSingleNode("/root/data/artist/picture_medium");
                if (node != null)
                {
                    string yes = node.InnerText.Replace("<![CDATA[ ", "").Replace(" ]]>", "");
                    return yes;
                }
            } catch (Exception)
            {

            }
            return "ms-appx:///Assets/BlankArtist.png";
        }

        #region Event handlers
        private void GridView_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (!KeyboardHelpers.IsCtrlPressed())
            {
                if ((e.OriginalSource as FrameworkElement).DataContext is ArtistViewModel artist)
                {
                    _ = Frame.Navigate(typeof(ArtistSongsPage), artist);
                    SelectedArtist = null;
                }
            } else
            {
                if ((e.OriginalSource as FrameworkElement).DataContext is ArtistViewModel artist)
                {
                    SelectedArtist = artist;
                }
            }
        }

        private void MainGrid_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            if ((e.OriginalSource as FrameworkElement).DataContext is ArtistViewModel artist)
            {
                SelectedArtist = artist;
                ArtistFlyout.ShowAt(MainGrid, e.GetPosition(MainGrid));
            }
        }

        private void AskDiscy_Click(object sender, RoutedEventArgs e)
        {
            DiscyOnArtist.IsOpen = true;
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

        private async void PlayItem_Click(object sender, RoutedEventArgs e)
        {
            SongViewModel song = App.MViewModel.Songs.FirstOrDefault(s => s.Artist == SelectedArtist.Name);
            await EventsLogic.StartMusicPlaybackAsync(App.MViewModel.Songs.IndexOf(song), false);
        }

        private async void ShuffleItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                List<SongViewModel> songs = new();
                lock (MViewModel.Songs)
                {
                    foreach (SongViewModel song in MViewModel.Songs)
                    {
                        if (song.Artist == SelectedArtist.Name)
                        {
                            songs.Add(song);
                        }
                    }
                }
                await App.PViewModel.StartMusicPlaybackAsync(songs.GetEnumerator(), new Random().Next(0, songs.Count), songs.Count, true);
            } catch (Exception)
            {
                SongViewModel song = App.MViewModel.Songs.FirstOrDefault(s => s.Artist == SelectedArtist.Name);
                await EventsLogic.StartMusicPlaybackAsync(App.MViewModel.Songs.IndexOf(song), false);
            }
        }

        private async void ChngArtImg_Click(object sender, RoutedEventArgs e)
        {
            var picker = new Windows.Storage.Pickers.FileOpenPicker
            {
                ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail,
                SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary
            };
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".jpeg");
            picker.FileTypeFilter.Add(".png");

            StorageFile file = await picker.PickSingleFileAsync();

            if (file != null)
            {
                // Get file thumbnail and make a PNG out of it.
                StorageItemThumbnail thumbnail = await file.GetThumbnailAsync(ThumbnailMode.MusicView, 200);
                await FileHelpers.SaveBitmapFromThumbnailWithReplaceAsync(thumbnail, $@"modified-artist-{SelectedArtist.Name}.png");

                thumbnail.Dispose();
                if (SelectedArtist != null)
                {
                    SelectedArtist.Picture = $@"ms-appdata:///local/modified-artist-{SelectedArtist.Name}.png";
                    await SelectedArtist.SaveAsync();
                }
            }
        }

        private void Grid_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            SelectedArtist = (e.OriginalSource as FrameworkElement).DataContext as ArtistViewModel;
            System.Diagnostics.Debug.WriteLine(SelectedArtist.Name);
        }

        private void SelectArtist_Click(object sender, RoutedEventArgs e)
        {
            SelectedArtistItem = (e.OriginalSource as FrameworkElement).DataContext as ArtistViewModel;
        }
    }
}
