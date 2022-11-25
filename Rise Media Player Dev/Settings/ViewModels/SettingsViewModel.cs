using Rise.Common.Enums;
using Rise.Common.Extensions;
using Rise.Data.Sources;
using Rise.Data.ViewModels;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Storage;
using Windows.UI;

namespace Rise.App.ViewModels
{
    public sealed partial class SettingsViewModel : ViewModel
    {
        private NavViewDataSource SBViewModel => App.NavDataSource;

        public SettingsViewModel()
        {
            UpdateStartupTaskInfo();
        }

        public string[] OpenLocations = new string[7]
        {
            "HomePage", "PlaylistsPage", "SongsPage",
            "ArtistsPage", "AlbumsPage", "GenresPage", "LocalVideosPage"
        };

        private readonly string[] _iconPacks = new string[2]
        {
            "Default", "Colorful"
        };

        public List<string> GeneralTags = new()
        {
            "HomePage",
            "PlaylistsPage"
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
        public const string StartupTaskId = "6VQ93204-N7OY-0258-54G3-385B9X0FUHIB";
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
            var task = await StartupTask.GetAsync(StartupTaskId);
            bool isEnabled = task.State switch
            {
                StartupTaskState.Enabled => true,
                StartupTaskState.EnabledByPolicy => true,
                _ => false,
            };

            if (!isEnabled)
                _ = await task.RequestEnableAsync();
            else
                task.Disable();

            SetOpenAtStartupInfo(task.State);
        }

        public void UpdateStartupTaskInfo()
        {
            var task = StartupTask.GetAsync(StartupTaskId).Get();
            SetOpenAtStartupInfo(task.State);
        }

        private void SetOpenAtStartupInfo(StartupTaskState state)
        {
            OpenInLogin = state switch
            {
                StartupTaskState.Enabled => true,
                StartupTaskState.EnabledByPolicy => true,
                _ => false,
            };

            CanOpenInLogin = state switch
            {
                StartupTaskState.Disabled => true,
                StartupTaskState.Enabled => true,
                _ => false,
            };

            FLGStartupTask = state switch
            {
                StartupTaskState.DisabledByPolicy => 1,
                StartupTaskState.DisabledByUser => 2,
                StartupTaskState.EnabledByPolicy => 3,
                _ => 0,
            };
        }

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

        public GlazeTypes SelectedGlaze
        {
            get => (GlazeTypes)Get<byte>(0, "Appearance");
            set => Set((byte)value, "Appearance");
        }

        public Color GlazeColors
        {
            get
            {
                var col = Get(new byte[4] { 0, 255, 255, 255 }, "Appearance");
                return Color.FromArgb(col[0], col[1], col[2], col[3]);
            }
            set => Set(new byte[4] { value.A, value.R, value.G, value.B }, "Appearance");
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

        public AlbumViewMode AlbumViewMode
        {
            get => (AlbumViewMode)Get<byte>(1, "Appearance");
            set => Set((byte)value, "Appearance");
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

        public bool IndexingTimerEnabled
        {
            get => Get(true, "MediaLibrary");
            set => Set(value, "MediaLibrary");
        }

        public bool IndexingAtStartupEnabled
        {
            get => Get(true, "MediaLibrary");
            set => Set(value, "MediaLibrary");
        }

        public bool IndexingFileTrackingEnabled
        {
            get => Get(true, "MediaLibrary");
            set => Set(value, "MediaLibrary");
        }

        public uint IndexingTimerInterval
        {
            get => Get((uint)5, "MediaLibrary");
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
            set => Set(value, "Local");
        }

        public bool ShowAllGeneral
        {
            get => ShowAtAGlance || ShowPlaylists || ShowHelpCentre;
            set
            {
                ShowAtAGlance = value;
                ShowPlaylists = value;
                ShowHelpCentre = value;

                OnPropertyChanged(nameof(ShowAtAGlance));
                OnPropertyChanged(nameof(ShowPlaylists));
                OnPropertyChanged(nameof(ShowHelpCentre));
            }
        }

        public bool ShowAllMusic
        {
            get
            {
                bool vis = ShowSongs || ShowArtists || ShowAlbums;
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

                OnPropertyChanged(nameof(ShowSongs));
                OnPropertyChanged(nameof(ShowArtists));
                OnPropertyChanged(nameof(ShowAlbums));
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

        /*
         * Visualizer types:
         * 
         * 0: None (don't show it)
         * 1: Bloom
        */

        public int VisualizerType
        {
            get => Get(0, "Playback");
            set => Set(value, "Playback");
        }

        public bool EqualizerEnabled
        {
            get => Get(false, "Local");
            set => Set(value, "Local");
        }

        private static float[] _defaultGain =
            new float[10] { 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f };

        public float[] EqualizerGain
        {
            get => Get(_defaultGain, "Local");
            set => Set(value, "Local");
        }

        public int SelectedEqualizerPreset
        {
            get => Get(0, "Local");
            set => Set(value, "Local");
        }

        public double Volume
        {
            get
            {
                var value = Get(100, "Playback");

                if (App.MPViewModel.Player.Volume != value)
                    App.MPViewModel.Player.Volume = value;

                return value;
            }
            set
            {
                Set(value, "Playback");
                App.MPViewModel.Player.Volume = value;
            }
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
    }

    // Getting and setting app settings
    public sealed partial class SettingsViewModel : ViewModel
    {
        /// <summary>
        /// Gets an app setting.
        /// </summary>
        /// <param name="defaultValue">Default setting value.</param>
        /// <param name="store">Setting store name.</param>
        /// <param name="setting">Setting name.</param>
        /// <returns>App setting value.</returns>
        /// <remarks>If the store parameter is "Local", a local setting will be returned.</remarks>
        private Type Get<Type>(Type defaultValue, string store = "Local", [CallerMemberName] string setting = null)
        {
            // If store == "Local", get a local setting
            if (store == "Local")
            {
                // Get app settings
                ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;

                // Check if the setting exists
                if (localSettings.Values[setting] == null)
                    localSettings.Values[setting] = defaultValue;

                object val = localSettings.Values[setting];

                // Return the setting if type matches
                if (val is not Type)
                {
                    string format = "Type mismatch for \"{0}\" in local store. Got {1}";
                    string message = string.Format(format, setting, val.GetType());

                    throw new ArgumentException(message);
                }

                return (Type)val;
            }

            // Get desired composite value
            ApplicationDataContainer roamingSettings = ApplicationData.Current.RoamingSettings;
            ApplicationDataCompositeValue composite = (ApplicationDataCompositeValue)roamingSettings.Values[store];

            // If the store exists, check if the setting does as well
            composite ??= new ApplicationDataCompositeValue();

            if (composite[setting] == null)
            {
                composite[setting] = defaultValue;
                roamingSettings.Values[store] = composite;
            }

            object value = composite[setting];

            // Return the setting if type matches
            if (value is not Type)
            {
                string format = "Type mismatch for \"{0}\" in local store. Current type is {1}";
                string message = string.Format(format, setting, value.GetType());

                throw new ArgumentException(message);
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

                OnPropertyChanged(setting);
                return;
            }

            // Get desired composite value
            ApplicationDataContainer roamingSettings = ApplicationData.Current.RoamingSettings;
            ApplicationDataCompositeValue composite = (ApplicationDataCompositeValue)roamingSettings.Values[store];

            // Store doesn't exist, create it
            composite ??= new ApplicationDataCompositeValue();

            // Set the setting to the desired value
            composite[setting] = newValue;
            roamingSettings.Values[store] = composite;

            OnPropertyChanged(setting);
        }
    }
}
