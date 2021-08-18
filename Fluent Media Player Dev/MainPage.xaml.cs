using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using Windows.ApplicationModel.Core;
using Windows.Media;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Search;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Fluent_Media_Player_Dev.Pages;
using NavigationViewItem = Microsoft.UI.Xaml.Controls.NavigationViewItem;
using NavigationViewItemBase = Microsoft.UI.Xaml.Controls.NavigationViewItemBase;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Fluent_Media_Player_Dev
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    ///

    public sealed partial class MainPage : Page
    {
        #region Variables
        public static MainPage Current;
        public ObservableCollection<Song> Songs { get; }
        public ObservableCollection<Album> Albums { get; set; }
        private List<string> AddedAlbums { get; set; }
        public ObservableCollection<string> Artists { get; set; }
        private List<StorageFile> Files { get; set; }
        public MediaPlaybackList PlaybackList { get; set; }
        private List<string> Properties { get; set; }
        #endregion
        #region Classes
        public class Album
        {
            public string Title { get; set; }
            public string Artist { get; set; }
            public BitmapImage Thumbnail { get; set; }
            public RandomAccessStreamReference StreamThumb { get; set; }
            public string Genre { get; set; }
        }

        public class Song
        {
            public string Title { get; set; }
            public string Album { get; set; }
            public string AlbumArtist { get; set; }
            public string Artist { get; set; }
            public string Duration { get; set; }
            public string Genre { get; set; }
            public uint Track { get; set; }
            public int Cd { get; set; }
        }
        #endregion

        public MainPage()
        {
            this.InitializeComponent();
            ContentFrame.Navigate(typeof(HomePage));
            FinishNavigation();

            Current = this;

            Songs = new ObservableCollection<Song>();
            Albums = new ObservableCollection<Album>();
            AddedAlbums = new List<string>();
            Artists = new ObservableCollection<string>();
            Files = new List<StorageFile>();
            PlaybackList = new MediaPlaybackList();
            Properties = new List<string>
            {
                "System.Music.DiscNumber",
                "System.Music.PartOfSet"
            };

            IndexSongs();

            #region Titlebar
            var titleBar = ApplicationView.GetForCurrentView().TitleBar;

            titleBar.ButtonBackgroundColor = Colors.Transparent;
            titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;

            // Hide default title bar.
            var coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
            coreTitleBar.ExtendViewIntoTitleBar = true;
            UpdateTitleBarLayout(coreTitleBar);

            // Set XAML element as a draggable region.
            Window.Current.SetTitleBar(AppTitleBar);

            // Register a handler for when the size of the overlaid caption control changes.
            // For example, when the app moves to a screen with a different DPI.
            coreTitleBar.LayoutMetricsChanged += CoreTitleBar_LayoutMetricsChanged;

            // Register a handler for when the title bar visibility changes.
            // For example, when the title bar is invoked in full screen mode.
            coreTitleBar.IsVisibleChanged += CoreTitleBar_IsVisibleChanged;

            //Register a handler for when the window changes focus
            Window.Current.Activated += Current_Activated;
            #endregion
        }

        #region Songs
        private async void IndexSongs()
        {
            ContentFrame.Visibility = Visibility.Collapsed;
            IndexInfo.Visibility = Visibility.Visible;

            // Query options
            QueryOptions queryOption = new QueryOptions
                (CommonFileQuery.DefaultQuery, new string[]
                {
                    ".mp3", ".wma", ".wav", ".ogg", ".flac", ".aiff", ".aac", ".m4a"
                })
            {
                FolderDepth = FolderDepth.Deep
            };

            // Add music library files to list
            IReadOnlyList<StorageFile> musicLibrary = await KnownFolders.MusicLibrary.
                CreateFileQueryWithOptions(queryOption).GetFilesAsync();

            Files.AddRange(musicLibrary);

            // Get items from future access list
            Windows.Storage.AccessCache.StorageItemAccessList fa =
                Windows.Storage.AccessCache.StorageApplicationPermissions.FutureAccessList;
            foreach (Windows.Storage.AccessCache.AccessListEntry entry in fa.Entries)
            {
                // Get folder from future access list
                string faToken = entry.Token;
                StorageFolder folder = await fa.GetFolderAsync(faToken);

                // Query files inside folder
                try
                {
                    IReadOnlyList<StorageFile> folderContents = await folder.CreateFileQueryWithOptions(queryOption).GetFilesAsync();
                    Files.AddRange(folderContents);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }

            int filesLength = Files.Count();
            int currentFile = 0;
            foreach (StorageFile file in Files)
            {
                currentFile++;
                LoadingBar.Value = currentFile * 100 / filesLength;
                IndexProgress.Text = "Indexed " + currentFile + " out of " + filesLength + " files";

                // Get file properties
                MusicProperties musicProperties = await file.Properties.GetMusicPropertiesAsync();
                int cd = 1;

                // Get the specified properties through StorageFile.Properties
                IDictionary<string, object> extraProperties = await file.Properties.RetrievePropertiesAsync(Properties);

                if (extraProperties["System.Music.DiscNumber"] != null)
                {
                    try
                    {
                        cd = int.Parse(extraProperties["System.Music.DiscNumber"].ToString());
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("Problem: " + ex.Message);
                        Debug.WriteLine("Problematic disc number: " + extraProperties["System.Music.DiscNumber"].ToString());
                    }
                }
                else if (extraProperties["System.Music.PartOfSet"] != null)
                {
                    try
                    {
                        cd = int.Parse(extraProperties["System.Music.PartOfSet"].ToString());
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("Problem: " + ex.Message);
                        Debug.WriteLine("Problematic part of set: " + extraProperties["System.Music.PartOfSet"].ToString());
                    }
                }

                MediaSource source = MediaSource.CreateFromStorageFile(file);
                MediaPlaybackItem media = new MediaPlaybackItem(source);

                MediaItemDisplayProperties props = media.GetDisplayProperties();
                props.Type = MediaPlaybackType.Music;

                props.MusicProperties.Title = musicProperties.Title.Length > 0
                    ? musicProperties.Title : file.DisplayName;

                props.MusicProperties.Artist = musicProperties.Artist.Length > 0
                    ? musicProperties.Artist : "Unknown Artist";

                props.MusicProperties.AlbumTitle = musicProperties.Album.Length > 0
                    ? musicProperties.Album : "Unknown Album";

                props.MusicProperties.AlbumArtist = musicProperties.AlbumArtist.Length > 0
                    ? musicProperties.AlbumArtist : "Unknown Artist";

                props.MusicProperties.TrackNumber = musicProperties.TrackNumber > 0
                    ? musicProperties.TrackNumber : 0;

                props.MusicProperties.AlbumTrackCount = (uint)(cd > 1
                    ? cd : 1);

                string genre = musicProperties.Genre.FirstOrDefault() != null
                    ? musicProperties.Genre.First() : "Unknown";

                // Add song
                Song currentSong = new Song()
                {
                    Title = props.MusicProperties.Title,
                    Artist = props.MusicProperties.Artist,
                    Album = props.MusicProperties.AlbumTitle,
                    AlbumArtist = props.MusicProperties.AlbumArtist,
                    Duration = musicProperties.Duration.ToString("mm\\:ss"),
                    Genre = genre,
                    Track = props.MusicProperties.TrackNumber,
                    Cd = cd
                };
                Songs.Add(currentSong);

                media.ApplyDisplayProperties(props);

                // Add song to queue
                PlaybackList.Items.Add(media);

                // Add album to album list
                if (!AddedAlbums.Contains(currentSong.Album))
                {
                    AddedAlbums.Add(currentSong.Album);

                    // Get file thumbnail
                    StorageItemThumbnail thumbnail = await file.GetThumbnailAsync(ThumbnailMode.MusicView, 200);

                    BitmapImage thumb = new BitmapImage();
                    if (thumbnail != null && thumbnail.Type == ThumbnailType.Image)
                    {
                        thumb.SetSource(thumbnail);
                    }

                    Albums.Add(new Album()
                    {
                        StreamThumb = RandomAccessStreamReference.CreateFromStream(thumbnail),
                        Thumbnail = thumb,
                        Title = currentSong.Album,
                        Artist = currentSong.AlbumArtist,
                        Genre = currentSong.Genre
                    });
                }

                // Add artist to artist list
                if (!Artists.Contains(currentSong.Artist))
                {
                    Artists.Add(currentSong.Artist);
                }

                if (!Artists.Contains(currentSong.AlbumArtist))
                {
                    Artists.Add(currentSong.AlbumArtist);
                }
            }

            // Show the main UI
            IndexInfo.Visibility = Visibility.Collapsed;
            IndexMessage.IsOpen = false;
            ContentFrame.Visibility = Visibility.Visible;
        }
        #endregion

        #region Titlebar
        private void CoreTitleBar_LayoutMetricsChanged(CoreApplicationViewTitleBar sender, object args)
        {
            UpdateTitleBarLayout(sender);
        }

        private void UpdateTitleBarLayout(CoreApplicationViewTitleBar coreTitleBar)
        {
            // Update title bar control size as needed to account for system size changes.
            AppTitleBar.Height = coreTitleBar.Height;

            // Ensure the custom title bar does not overlap window caption controls
            Thickness currMargin = AppTitleBar.Margin;
            AppTitleBar.Margin = new Thickness(currMargin.Left, currMargin.Top, coreTitleBar.SystemOverlayRightInset, currMargin.Bottom);
        }

        private void CoreTitleBar_IsVisibleChanged(CoreApplicationViewTitleBar sender, object args)
        {
            if (sender.IsVisible)
            {
                AppTitleBar.Visibility = Visibility.Visible;
            }
            else
            {
                AppTitleBar.Visibility = Visibility.Collapsed;
            }
        }

        // Update the TitleBar based on the inactive/active state of the app
        private void Current_Activated(object sender, Windows.UI.Core.WindowActivatedEventArgs e)
        {
            SolidColorBrush defaultForegroundBrush = (SolidColorBrush)Application.Current.Resources["TextFillColorPrimaryBrush"];
            SolidColorBrush inactiveForegroundBrush = (SolidColorBrush)Application.Current.Resources["TextFillColorDisabledBrush"];

            if (e.WindowActivationState == Windows.UI.Core.CoreWindowActivationState.Deactivated)
            {
                AppTitle.Foreground = inactiveForegroundBrush;
            }
            else
            {
                AppTitle.Foreground = defaultForegroundBrush;
            }
        }

        // Update the TitleBar content layout depending on NavigationView DisplayMode
        private void NavigationViewControl_DisplayModeChanged(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewDisplayModeChangedEventArgs args)
        {
            const int topIndent = 16;
            const int expandedIndent = 48;
            int minimalIndent = 104;

            Thickness currMargin = AppTitleBar.Margin;

            // Set the TitleBar margin dependent on NavigationView display mode
            if (sender.PaneDisplayMode == Microsoft.UI.Xaml.Controls.NavigationViewPaneDisplayMode.Top)
            {
                AppTitleBar.Margin = new Thickness(topIndent, currMargin.Top, currMargin.Right, currMargin.Bottom);
            }
            else if (sender.DisplayMode == Microsoft.UI.Xaml.Controls.NavigationViewDisplayMode.Minimal)
            {
                AppTitleBar.Margin = new Thickness(minimalIndent, currMargin.Top, currMargin.Right, currMargin.Bottom);
            }
            else
            {
                AppTitleBar.Margin = new Thickness(expandedIndent, currMargin.Top, currMargin.Right, currMargin.Bottom);
            }
        }
        #endregion

        #region Navigation
        private void NavView_ItemInvoked(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewItemInvokedEventArgs args)
        {
            if (IndexInfo.Visibility == Visibility.Visible)
            {
                FinishNavigation();
                IndexMessage.IsOpen = true;
                return;
            }

            string navTo = args.InvokedItemContainer.Tag.ToString();
            if (args.IsSettingsInvoked)
            {
                ContentFrame.Navigate(typeof(SettingsPage));
            }
            else
            {
                if (navTo != null)
                {
                    switch (navTo)
                    {
                        case "AlbumsPage":
                            ContentFrame.Navigate(typeof(AlbumsPage));
                            break;

                        case "ArtistsPage":
                            ContentFrame.Navigate(typeof(ArtistsPage));
                            break;

                        case "DevicesPage":
                            ContentFrame.Navigate(typeof(DevicesPage));
                            break;

                        case "DiscyPage":
                            ContentFrame.Navigate(typeof(DiscyPage));
                            break;

                        case "HomePage":
                            ContentFrame.Navigate(typeof(HomePage));
                            break;

                        case "LocalVideosPage":
                            ContentFrame.Navigate(typeof(LocalVideosPage));
                            break;

                        case "NowPlayingPage":
                            ContentFrame.Navigate(typeof(NowPlayingPage));
                            break;

                        case "PlaylistsPage":
                            ContentFrame.Navigate(typeof(PlaylistsPage));
                            break;

                        case "SongsPage":
                            ContentFrame.Navigate(typeof(SongsPage));
                            break;

                        default:
                            break;
                    }

                }
            }
            FinishNavigation();
        }

        private void NavView_BackRequested(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewBackRequestedEventArgs args)
        {
            ContentFrame.GoBack();
            FinishNavigation();
        }

        private void FinishNavigation()
        {
            string type = ContentFrame.CurrentSourcePageType.ToString();
            string tag = type.Split('.').Last();

            if (tag == "SettingsPage")
            {
                NavView.SelectedItem = NavView.SettingsItem;
                NavView.Header = "Settings";
            }

            foreach (NavigationViewItemBase item in NavView.MenuItems)
            {
                if (item is NavigationViewItem && item.Tag.ToString() == tag)
                {
                    NavView.SelectedItem = item;
                    NavView.Header = item.Content;
                    break;
                }
            }

            foreach (NavigationViewItemBase item in NavView.FooterMenuItems)
            {
                if (item is NavigationViewItem && item.Tag.ToString() == tag)
                {
                    NavView.SelectedItem = item;
                    NavView.Header = item.Content;
                    break;
                }
            }
        }
        #endregion
    }
}