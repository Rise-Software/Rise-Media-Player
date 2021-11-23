using Rise.App.Views;

namespace Rise.App.Settings.ViewModels
{
    public class SettingsViewModel : SettingsManager
    {
        // Empty constructor.
        public SettingsViewModel() { }

        public string[] OpenLocations = new string[10]
        {
            "HomePage", "PlaylistsPage", "DevicesPage", "SongsPage", "ArtistsPage",
            "AlbumsPage", "GenresPage", "LocalVideosPage", "StreamingPage", "NowPlayingPage"
        };

        #region Appearance
        public int Theme
        {
            get => (int)Get("Appearance", nameof(Theme), 2);
            set => Set("Appearance", nameof(Theme), value);
        }

        public int ColorTheme
        {
            get => (int)Get("ColorTheme", nameof(ColorTheme), -2);
            set => Set("ColorTheme", nameof(ColorTheme), value);
        }

        public int Color
        {
            get => (int)Get("Color", nameof(Color), 0);
            set => Set("Color", nameof(Color), value);
        }

        public bool SquareAlbumArt
        {
            get => (bool)Get("Appearance", nameof(SquareAlbumArt), false);
            set => Set("Appearance", nameof(SquareAlbumArt), value);
        }

        public int OpenTo
        {
            get => (int)Get("Appearance", nameof(OpenTo), 0);
            set => Set("Appearance", nameof(OpenTo), value);
        }

        public string Open =>
            OpenLocations[(int)Get("Appearance", nameof(OpenTo), 0)];

        public bool PickUp
        {
            get => (bool)Get("Appearance", nameof(PickUp), false);
            set => Set("Appearance", nameof(PickUp), value);
        }

        public bool CompactMode
        {
            get => (bool)Get("Appearance", nameof(CompactMode), true);
            set => Set("Appearance", nameof(CompactMode), value);
        }

        public bool ColoredSettingsIcons
        {
            get => (bool)Get("Appearance", nameof(ColoredSettingsIcons), true);
            set => Set("Appearance", nameof(ColoredSettingsIcons), value);
        }

        public bool TrackHistory
        {
            get => (bool)Get("Appearance", nameof(TrackHistory), false);
            set => Set("Appearance", nameof(TrackHistory), value);
        }

        public bool CuratedPlaylists
        {
            get => (bool)Get("Appearance", nameof(CuratedPlaylists), false);
            set => Set("Appearance", nameof(CuratedPlaylists), value);
        }
        #endregion

        #region Media Library
        public int Deletion
        {
            get => (int)Get("MediaLibrary", nameof(Deletion), 0);
            set => Set("MediaLibrary", nameof(Deletion), value);
        }

        public bool SeparateLocal
        {
            get => (bool)Get("MediaLibrary", nameof(SeparateLocal), false);
            set => Set("MediaLibrary", nameof(SeparateLocal), value);
        }

        public bool DisableOnline
        {
            get => (bool)Get("MediaLibrary", nameof(DisableOnline), false);
            set => Set("MediaLibrary", nameof(DisableOnline), value);
        }

        public bool FilterByNameOnly
        {
            get => (bool)Get("MediaLibrary", nameof(FilterByNameOnly), false);
            set => Set("MediaLibrary", nameof(FilterByNameOnly), value);
        }
        #endregion

        #region Navigation
        public int Resize
        {
            get => (int)Get("Navigation", nameof(Resize), 0);
            set => Set("Navigation", nameof(Resize), value);
        }

        public bool Hamburger
        {
            get => (bool)Get("Navigation", nameof(Hamburger), false);
            set => Set("Navigation", nameof(Hamburger), value);
        }

        // TODO: Fix this terrible code to properly support icon packs.
        public int IconPack
        {
            get => (int)Get("Local", nameof(IconPack), 0);
            set
            {
                Set("Local", nameof(IconPack), value);
                MainPage.Current.UpdateIconColor(value);
            }
        }

        public bool ShowAllGeneral
        {
            get => ShowAtAGlance || ShowDevices || ShowPlaylists || ShowHelpCentre || ShowNowPlaying;
            set
            {
                ShowAtAGlance = value;
                ShowDevices = value;
                ShowPlaylists = value;
                ShowHelpCentre = value;
                ShowNowPlaying = value;
            }
        }

        public bool ShowAllMusic
        {
            get => ShowSongs || ShowArtists || ShowAlbums || ShowGenres;
            set
            {
                ShowMusicHeader = value;
                ShowSongs = value;
                ShowArtists = value;
                ShowAlbums = value;
                ShowGenres = value;
            }
        }

        public bool ShowAllVideo
        {
            get => ShowLocalVideos || ShowStreaming;
            set
            {
                ShowVideoHeader = value;
                ShowLocalVideos = value;
                ShowStreaming = value;
            }
        }

        public bool ShowMusicHeader
        {
            get => (bool)Get("Navigation", nameof(ShowMusicHeader), true);
            set => Set("Navigation", nameof(ShowMusicHeader), value);
        }

        public bool ShowVideoHeader
        {
            get => (bool)Get("Navigation", nameof(ShowVideoHeader), true);
            set => Set("Navigation", nameof(ShowVideoHeader), value);
        }

        public bool ShowAtAGlance
        {
            get => (bool)Get("Navigation", nameof(ShowAtAGlance), true);
            set => Set("Navigation", nameof(ShowAtAGlance), value);
        }

        public bool ShowPlaylists
        {
            get => (bool)Get("Navigation", nameof(ShowPlaylists), true);
            set => Set("Navigation", nameof(ShowPlaylists), value);
        }

        public bool ShowDevices
        {
            get => (bool)Get("Navigation", nameof(ShowDevices), true);
            set => Set("Navigation", nameof(ShowDevices), value);
        }

        public bool ShowSongs
        {
            get => (bool)Get("Navigation", nameof(ShowSongs), true);
            set => Set("Navigation", nameof(ShowSongs), value);
        }

        public bool ShowArtists
        {
            get => (bool)Get("Navigation", nameof(ShowArtists), true);
            set => Set("Navigation", nameof(ShowArtists), value);
        }

        public bool ShowAlbums
        {
            get => (bool)Get("Navigation", nameof(ShowAlbums), true);
            set => Set("Navigation", nameof(ShowAlbums), value);
        }

        public bool ShowGenres
        {
            get => (bool)Get("Navigation", nameof(ShowGenres), true);
            set => Set("Navigation", nameof(ShowGenres), value);
        }

        public bool ShowLocalVideos
        {
            get => (bool)Get("Navigation", nameof(ShowLocalVideos), true);
            set => Set("Navigation", nameof(ShowLocalVideos), value);
        }

        public bool ShowStreaming
        {
            get => (bool)Get("Navigation", nameof(ShowStreaming), true);
            set => Set("Navigation", nameof(ShowStreaming), value);
        }

        public bool ShowHelpCentre
        {
            get => (bool)Get("Navigation", nameof(ShowHelpCentre), true);
            set => Set("Navigation", nameof(ShowHelpCentre), value);
        }

        public bool ShowNowPlaying
        {
            get => (bool)Get("Navigation", nameof(ShowNowPlaying), true);
            set => Set("Navigation", nameof(ShowNowPlaying), value);
        }
        #endregion

        #region Playback
        public bool Gapless
        {
            get => (bool)Get("Playback", nameof(Gapless), false);
            set => Set("Playback", nameof(Gapless), value);
        }

        public int CrossfadeDuration
        {
            get => (int)Get("Playback", nameof(CrossfadeDuration), 0);
            set => Set("Playback", nameof(CrossfadeDuration), value);
        }

        public int ScaleToWindow
        {
            get => (int)Get("Playback", nameof(ScaleToWindow), 0);
            set => Set("Playback", nameof(ScaleToWindow), value);
        }

        public bool GoDevice
        {
            get => (bool)Get("Playback", nameof(GoDevice), false);
            set => Set("Playback", nameof(GoDevice), value);
        }

        public bool ReplaceFlyouts
        {
            get => (bool)Get("Playback", nameof(ReplaceFlyouts), false);
            set => Set("Playback", nameof(ReplaceFlyouts), value);
        }
        #endregion

        #region Setup
        public bool SetupCompleted
        {
            get => (bool)Get("Local", nameof(SetupCompleted), false);
            set => Set("Local", nameof(SetupCompleted), value);
        }

        public int SetupProgress
        {
            get => (int)Get("Local", nameof(SetupProgress), 0);
            set => Set("Local", nameof(SetupProgress), value);
        }
        #endregion

        public int Language
        {
            get => (int)Get("Local", nameof(Language), 0);
            set => Set("Local", nameof(Language), value);
        }

        #region Methods
        /// <summary>
        /// Changes the visibility of NavigationView headers.
        /// </summary>
        /// <param name="visibilityCheck">1 checks for music, 2 checks for videos.</param>
        public void ChangeHeaderVisibility(int visibilityCheck)
        {
            if (visibilityCheck == 1)
            {
                if (!ShowSongs && !ShowArtists &&
                    !ShowAlbums && !ShowGenres)
                {
                    ShowMusicHeader = false;
                }
                else
                {
                    ShowMusicHeader = true;
                }
            }
            else if (visibilityCheck == 2)
            {
                if (!ShowLocalVideos && !ShowStreaming)
                {
                    ShowVideoHeader = false;
                }
                else
                {
                    ShowVideoHeader = true;
                }
            }
        }
        #endregion
    }
}
