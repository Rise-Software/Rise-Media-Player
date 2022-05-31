using Microsoft.Toolkit.Mvvm.Input;
using Rise.App.Views;
using Rise.Data.Sources;
using Rise.Data.ViewModels;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.ApplicationModel;
using Windows.Storage;

namespace Rise.App.ViewModels
{
    public class SettingsViewModel : ViewModel
    {
        private NavViewDataSource SBViewModel => App.NavDataSource;
        public ICommand OpenFilesAtStartupCommand { get; }

        public SettingsViewModel()
        {
            OpenFilesAtStartupCommand = new AsyncRelayCommand(OpenAtStartupAsync);

            _ = DetectOpenAtStartupAsync();
        }

        public string[] OpenLocations = new string[8]
        {
            "HomePage", "NowPlayingPage", "PlaylistsPage", "SongsPage",
            "ArtistsPage", "AlbumsPage", "GenresPage", "LocalVideosPage"
        };

        private readonly string[] _iconPacks = new string[2]
        {
            "Default", "Colorful"
        };

        public List<string> GeneralTags = new()
        {
            "HomePage",
            "PlaylistsPage",
            "ConnectedDevicesPage",
            "NowPlayingPage"
        };

        public List<string> MusicTags = new()
        {
            "SongsPage",
            "ArtistsPage",
            "AlbumsPage",
            "GenresPage"
        };

        public List<string> VideoTags = new()
        {
            "LocalVideosPage",
            "StreamingPage"
        };

        #region Startup
        public bool OpenInLogin
        {
            get => Get(false, "WindowsBehaviours");
            set => Set(value, "WindowsBehaviours");
        }

        public bool CanOpenInLogin
        {
            get => Get(true, "WindowsBehaviours");
            set => Set(value, "WindowsBehaviours");
        }

        public int FLGStartupTask
        {
            get => Get(0, "WindowsBehaviours");
            set => Set(value, "WindowsBehaviours");
        }

        public async Task OpenAtStartupAsync()
        {
            var stateMode = await ReadStateAsync();

            bool state = stateMode switch
            {
                StartupTaskState.Enabled => true,
                StartupTaskState.EnabledByPolicy => true,
                StartupTaskState.DisabledByPolicy => false,
                StartupTaskState.DisabledByUser => false,
                _ => false,
            };

            if (state != OpenInLogin)
            {
                StartupTask startupTask = await StartupTask.GetAsync("6VQ93204-N7OY-0258-54G3-385B9X0FUHIB");
                if (OpenInLogin)
                {
                    await startupTask.RequestEnableAsync();
                }
                else
                {
                    startupTask.Disable();
                }
                await DetectOpenAtStartupAsync();
            }
        }

        public async Task DetectOpenAtStartupAsync()
        {
            var stateMode = await ReadStateAsync();

            switch (stateMode)
            {
                case StartupTaskState.Disabled:
                    CanOpenInLogin = true;
                    OpenInLogin = false;
                    FLGStartupTask = 0;
                    break;
                case StartupTaskState.Enabled:
                    CanOpenInLogin = true;
                    OpenInLogin = true;
                    FLGStartupTask = 0;
                    break;
                case StartupTaskState.DisabledByPolicy:
                    CanOpenInLogin = false;
                    OpenInLogin = false;
                    FLGStartupTask = 1;
                    break;
                case StartupTaskState.DisabledByUser:
                    CanOpenInLogin = false;
                    OpenInLogin = false;
                    FLGStartupTask = 2;
                    break;
                case StartupTaskState.EnabledByPolicy:
                    CanOpenInLogin = false;
                    OpenInLogin = true;
                    FLGStartupTask = 3;
                    break;
            }
        }

        public async Task<StartupTaskState> ReadStateAsync()
            => (await StartupTask.GetAsync("6VQ93204-N7OY-0258-54G3-385B9X0FUHIB")).State;

        #endregion

        #region Appearance
        public int Theme
        {
            get => Get(2, "Appearance");
            set => Set(value, "Appearance");
        }

        public int ColorTheme
        {
            get => Get(0, "Appearance");
            set => Set(value, "Appearance");
        }

        public int Color
        {
            get => Get(-1, "Appearance");
            set => Set(value, "Appearance");
        }

        public bool SquareAlbumArt
        {
            get => Get(false, "Appearance");
            set => Set(value, "Appearance");
        }

        public int OpenTo
        {
            get => Get(0, "Appearance");
            set => Set(value, "Appearance");
        }

        public string Open => OpenLocations[OpenTo];

        public bool PickUp
        {
            get => Get(false, "Appearance");
            set => Set(value, "Appearance");
        }

        public bool CompactMode
        {
            get => Get(true, "Appearance");
            set => Set(value, "Appearance");
        }

        public bool ColoredSettingsIcons
        {
            get => Get(true, "Appearance");
            set => Set(value, "Appearance");
        }

        public bool TrackHistory
        {
            get => Get(false, "Appearance");
            set => Set(value, "Appearance");
        }

        public bool CuratedPlaylists
        {
            get => Get(false, "Appearance");
            set => Set(value, "Appearance");
        }

        public bool IsTilesInAlbumsPage
        {
            get => Get(true, "Appearance");
            set => Set(value, "Appearance");
        }

        public bool IsListInAlbumsPage
        {
            get => Get(false, "Appearance");
            set => Set(value, "Appearance");
        }
        #endregion

        #region Media Library
        public bool FetchOnlineData
        {
            get => Get(true, "MediaLibrary");
            set => Set(value, "MediaLibrary");
        }

        public int Deletion
        {
            get => Get(0, "MediaLibrary");
            set => Set(value, "MediaLibrary");
        }

        public bool AutoIndexingEnabled
        {
            get => Get(true, "MediaLibrary");
            set => Set(value, "MediaLibrary");
        }

        /* 
         
         Indexing modes

         -1: Never
         0: Every 1 minute
         1: Every 5 minutes
         2: Every 10 minutes
         3: Every 30 minutes
         4: Every 1 hour

         */

        public int IndexingMode
        {
            get => Get(1, "MediaLibrary");
            set => Set(value, "MediaLibrary");
        }

        public bool SeparateLocal
        {
            get => Get(false, "MediaLibrary");
            set => Set(value, "MediaLibrary");
        }

        public bool DisableOnline
        {
            get => Get(false, "MediaLibrary");
            set => Set(value, "MediaLibrary");
        }

        public bool ShowArtistInAlbums
        {
            get => Get(true, "MediaLibrary");
            set => Set(value, "MediaLibrary");
        }

        public bool ShowThumbnailInAlbums
        {
            get => Get(true, "MediaLibrary");
            set => Set(value, "MediaLibrary");
        }

        public bool ShowGenreInAlbums
        {
            get => Get(false, "MediaLibrary");
            set => Set(value, "MediaLibrary");
        }

        public bool ShowTitleInAlbums
        {
            get => Get(true, "MediaLibrary");
            set => Set(value, "MediaLibrary");
        }

        public bool ShowReleaseYearInAlbums
        {
            get => Get(false, "MediaLibrary");
            set => Set(value, "MediaLibrary");
        }

        public bool RoundedAlbumArt
        {
            get => Get(true, "MediaLibrary");
            set => Set(value, "MediaLibrary");
        }

        public bool ShowTrackNumberInSongs
        {
            get => Get(true, "MediaLibrary");
            set => Set(value, "MediaLibrary");
        }

        public bool ShowDurationInSongs
        {
            get => Get(true, "MediaLibrary");
            set => Set(value, "MediaLibrary");
        }

        #endregion

        #region Navigation
        public int Resize
        {
            get => Get(0, "Navigation");
            set => Set(value, "Navigation");
        }

        public bool Hamburger
        {
            get => Get(false, "Navigation");
            set => Set(value, "Navigation");
        }

        public string CurrentPack => _iconPacks[IconPack];
        public int IconPack
        {
            get => Get(0, "Local");
            set
            {
                Set(value, "Local");
                MainPage.Current.ChangeIconPack(CurrentPack);
            }
        }

        public bool ShowAllGeneral
        {
            get => ShowAtAGlance || ShowPlaylists || ShowHelpCentre || ShowNowPlaying;
            set
            {
                ShowAtAGlance = value;
                ShowPlaylists = value;
                ShowHelpCentre = value;
                ShowNowPlaying = value;

                OnPropertyChanged(nameof(ShowAtAGlance));
                OnPropertyChanged(nameof(ShowPlaylists));
                OnPropertyChanged(nameof(ShowHelpCentre));
                OnPropertyChanged(nameof(ShowNowPlaying));
            }
        }

        public bool ShowAllMusic
        {
            get
            {
                bool vis = ShowSongs || ShowArtists || ShowAlbums || ShowGenres;
                if (!vis)
                {
                    ShowMusicHeader = false;
                }

                OnPropertyChanged(nameof(ShowMusicHeader));
                return vis;
            }
            set
            {
                ShowMusicHeader = value;
                ShowSongs = value;
                ShowArtists = value;
                ShowAlbums = value;
                ShowGenres = value;

                OnPropertyChanged(nameof(ShowSongs));
                OnPropertyChanged(nameof(ShowArtists));
                OnPropertyChanged(nameof(ShowAlbums));
                OnPropertyChanged(nameof(ShowGenres));
            }
        }

        public bool ShowAllVideo
        {
            get
            {
                bool vis = ShowLocalVideos;
                if (!vis)
                {
                    ShowVideoHeader = false;
                }

                OnPropertyChanged(nameof(ShowVideoHeader));
                return vis;
            }
            set
            {
                ShowVideoHeader = value;
                ShowLocalVideos = value;

                OnPropertyChanged(nameof(ShowLocalVideos));
            }
        }

        public bool ShowMusicHeader
        {
            get => SBViewModel.IsHeaderVisible("Music");
            set => SBViewModel.ChangeHeaderVisibility("Music", value);
        }

        public bool ShowVideoHeader
        {
            get => SBViewModel.IsHeaderVisible("Videos");
            set => SBViewModel.ChangeHeaderVisibility("Videos", value);
        }

        public bool ShowAtAGlance
        {
            get => SBViewModel.IsItemVisible("HomePage");
            set => ChangeItemVisibility("HomePage", value);
        }

        public bool ShowFileBrowser
        {
            get => SBViewModel.IsItemVisible("FileBrowserPage");
            set => ChangeItemVisibility("FileBrowserPage", value);
        }

        public bool ShowPlaylists
        {
            get => SBViewModel.IsItemVisible("PlaylistsPage");
            set => ChangeItemVisibility("PlaylistsPage", value);
        }

        public bool ShowSongs
        {
            get => SBViewModel.IsItemVisible("SongsPage");
            set => ChangeItemVisibility("SongsPage", value);
        }

        public bool ShowArtists
        {
            get => SBViewModel.IsItemVisible("ArtistsPage");
            set => ChangeItemVisibility("ArtistsPage", value);
        }

        public bool ShowAlbums
        {
            get => SBViewModel.IsItemVisible("AlbumsPage");
            set => ChangeItemVisibility("AlbumsPage", value);
        }

        public bool ShowGenres
        {
            get => SBViewModel.IsItemVisible("GenresPage");
            set => ChangeItemVisibility("GenresPage", value);
        }

        public bool ShowLocalVideos
        {
            get => SBViewModel.IsItemVisible("LocalVideosPage");
            set => ChangeItemVisibility("LocalVideosPage", value);
        }

        public bool ShowHelpCentre
        {
            get => SBViewModel.IsItemVisible("DiscyPage");
            set => ChangeItemVisibility("DiscyPage", value);
        }

        public bool ShowNowPlaying
        {
            get => SBViewModel.IsItemVisible("NowPlayingPage");
            set => ChangeItemVisibility("NowPlayingPage", value);
        }

        private void ChangeItemVisibility(string tag, bool value)
        {
            SBViewModel.ChangeItemVisibility(tag, value);

            if (GeneralTags.Contains(tag))
            {
                OnPropertyChanged(nameof(ShowAllGeneral));
            }
            else if (MusicTags.Contains(tag))
            {
                OnPropertyChanged(nameof(ShowAllMusic));
            }
            else
            {
                OnPropertyChanged(nameof(ShowAllVideo));
            }
        }
        #endregion

        #region Playback
        public bool Gapless
        {
            get => Get(false, "Playback");
            set => Set(value, "Playback");
        }

        public int CrossfadeDuration
        {
            get => Get(0, "Playback");
            set => Set(value, "Playback");
        }

        public int ScaleToWindow
        {
            get => Get(0, "Playback");
            set => Set(value, "Playback");
        }

        public bool GoDevice
        {
            get => Get(false, "Playback");
            set => Set(value, "Playback");
        }

        public bool ReplaceFlyouts
        {
            get => Get(false, "Playback");
            set => Set(value, "Playback");
        }
        #endregion

        #region Setup
        public bool SetupCompleted
        {
            get => Get(false);
            set => Set(value);
        }

        public int SetupProgress
        {
            get => Get(0);
            set => Set(value);
        }
        #endregion

        public int Language
        {
            get => Get(0);
            set => Set(value);
        }

        #region Methods to get/set app settings
        /// <summary>
        /// Gets an app setting.
        /// </summary>
        /// <param name="defaultValue">Default setting value.</param>
        /// <param name="store">Setting store name.</param>
        /// <param name="setting">Setting name.</param>
        /// <returns>App setting value.</returns>
        /// <remarks>If the store parameter is "Local", a local setting will be returned.</remarks>
        private Type Get<Type>(Type defaultValue,
            string store = "Local",
            [CallerMemberName] string setting = null)
        {
            // If store == "Local", get a local setting
            if (store == "Local")
            {
                // Get app settings
                ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;

                // Check if the setting exists
                if (localSettings.Values[setting] == null)
                {
                    localSettings.Values[setting] = defaultValue;
                }

                object val = localSettings.Values[setting];

                // Return the setting if type matches
                if (val is not Type)
                {
                    throw new ArgumentException("Type mismatch for \"" + setting + "\" in local store. Got " + val.GetType());
                }

                return (Type)val;
            }

            // Get desired composite value
            ApplicationDataContainer roamingSettings = ApplicationData.Current.RoamingSettings;
            ApplicationDataCompositeValue composite = (ApplicationDataCompositeValue)roamingSettings.Values[store];

            // If the store exists, check if the setting does as well
            if (composite == null)
            {
                composite = new ApplicationDataCompositeValue();
            }

            if (composite[setting] == null)
            {
                composite[setting] = defaultValue;
                roamingSettings.Values[store] = composite;
            }

            object value = composite[setting];

            // Return the setting if type matches
            if (value is not Type)
            {
                throw new ArgumentException("Type mismatch for \"" + setting + "\" in store \"" + store + "\". Current type is " + value.GetType());
            }

            return (Type)value;
        }

        /// <summary>
        /// Sets an app setting.
        /// </summary>
        /// <param name="newValue">New setting value.</param>
        /// <param name="store">Setting store name.</param>
        /// <param name="setting">Setting name.</param>
        /// <remarks>If the store parameter is "Local", a local setting will be set.</remarks>
        private void Set<Type>(Type newValue, string store = "Local", [CallerMemberName] string setting = null)
        {
            // Try to get the setting, if types don't match, it'll throw an exception
            _ = Get(newValue, store, setting);

            // If store == "Local", set a local setting
            if (store == "Local")
            {
                // Get app settings
                ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
                localSettings.Values[setting] = newValue;
                return;
            }

            // Get desired composite value
            ApplicationDataContainer roamingSettings = ApplicationData.Current.RoamingSettings;
            ApplicationDataCompositeValue composite = (ApplicationDataCompositeValue)roamingSettings.Values[store];

            // Store doesn't exist, create it
            if (composite == null)
            {
                composite = new ApplicationDataCompositeValue();
            }

            // Set the setting to the desired value
            composite[setting] = newValue;
            roamingSettings.Values[store] = composite;

            OnPropertyChanged(setting);
        }
        #endregion
    }
}
