using Windows.Storage;
using Fluent_Media_Player_Dev;

namespace Fluent_Media_Player_Dev.Settings
{
    public class Settings
    {
        #region Appearance
        public int Theme
        {
            get => GetIntSetting("Appearance", "Theme", 2);
            set => SetIntSetting("Appearance", "Theme", value);
        }

        public bool ShowHeaders
        {
            get => GetBoolSetting("Appearance", "ShowHeaders", true);
            set => SetBoolSetting("Appearance", "ShowHeaders", value);
        }

        public bool CompactMode
        {
            get => GetBoolSetting("Appearance", "CompactMode", false);
            set => SetBoolSetting("Appearance", "CompactMode", value);
        }

        public bool CommandBarAccent
        {
            get => GetBoolSetting("Appearance", "CommandBarAccent", false);
            set => SetBoolSetting("Appearance", "CommandBarAccent", value);
        }
        #endregion

        #region Media Library
        public int Deletion
        {
            get => GetIntSetting("MediaLibrary", "Deletion", 0);
            set => SetIntSetting("MediaLibrary", "Deletion", value);
        }

        public bool SquareAlbumArt
        {
            get => GetBoolSetting("MediaLibrary", "SquareAlbumArt", false);
            set => SetBoolSetting("MediaLibrary", "SquareAlbumArt", value);
        }

        public bool MatchAlbumArt
        {
            get => GetBoolSetting("MediaLibrary", "MatchAlbumArt", true);
            set => SetBoolSetting("MediaLibrary", "MatchAlbumArt", value);
        }

        public bool MergeAM
        {
            get => GetBoolSetting("MediaLibrary", "MergeAM", false);
            set => SetBoolSetting("MediaLibrary", "MergeAM", value);
        }

        public bool MergeDZ
        {
            get => GetBoolSetting("MediaLibrary", "MergeDZ", false);
            set => SetBoolSetting("MediaLibrary", "MergeDZ", value);
        }

        public bool MergeSPT
        {
            get => GetBoolSetting("MediaLibrary", "MergeSPT", false);
            set => SetBoolSetting("MediaLibrary", "MergeSPT", value);
        }

        public bool MergeYTM
        {
            get => GetBoolSetting("MediaLibrary", "MergeYTM", false);
            set => SetBoolSetting("MediaLibrary", "MergeYTM", value);
        }

        public bool IndexMP3
        {
            get => GetBoolSetting("MediaLibrary", "IndexMP3", true);
            set => SetBoolSetting("MediaLibrary", "IndexMP3", value);
        }

        public bool IndexOGG
        {
            get => GetBoolSetting("MediaLibrary", "IndexOGG", true);
            set => SetBoolSetting("MediaLibrary", "IndexOGG", value);
        }

        public bool IndexAAC
        {
            get => GetBoolSetting("MediaLibrary", "IndexAAC", true);
            set => SetBoolSetting("MediaLibrary", "IndexAAC", value);
        }

        public bool IndexFLAC
        {
            get => GetBoolSetting("MediaLibrary", "IndexFLAC", true);
            set => SetBoolSetting("MediaLibrary", "IndexFLAC", value);
        }

        public bool IndexWAV
        {
            get => GetBoolSetting("MediaLibrary", "IndexWAV", true);
            set => SetBoolSetting("MediaLibrary", "IndexWAV", value);
        }

        public bool IndexAIFF
        {
            get => GetBoolSetting("MediaLibrary", "IndexAIFF", true);
            set => SetBoolSetting("MediaLibrary", "IndexAIFF", value);
        }

        public bool IndexM4A
        {
            get => GetBoolSetting("MediaLibrary", "IndexM4A", true);
            set => SetBoolSetting("MediaLibrary", "IndexM4A", value);
        }

        public bool IndexWMA
        {
            get => GetBoolSetting("MediaLibrary", "IndexWMA", true);
            set => SetBoolSetting("MediaLibrary", "IndexWMA", value);
        }
        #endregion

        #region Playback
        public int PresetEQ
        {
            get => GetIntSetting("Playback", "PresetEQ", 0);
            set => SetIntSetting("Playback", "PresetEQ", value);
        }

        public bool Crossfade
        {
            get => GetBoolSetting("Playback", "Crossfade", false);
            set => SetBoolSetting("Playback", "Crossfade", value);
        }

        public bool Gapless
        {
            get => GetBoolSetting("Playback", "Gapless", false);
            set => SetBoolSetting("Playback", "Gapless", value);
        }


        public int MusicQuality
        {
            get => GetIntSetting("Playback", "MusicQuality", 1);
            set => SetIntSetting("Playback", "MusicQuality", value);
        }

        public int VideoQuality
        {
            get => GetIntSetting("Playback", "VideoQuality", 1);
            set => SetIntSetting("Playback", "VideoQuality", value);
        }

        public bool ReplaceFlyouts
        {
            get => GetBoolSetting("Playback", "ReplaceFlyouts", false);
            set => SetBoolSetting("Playback", "ReplaceFlyouts", value);
        }

        public bool GoTrack
        {
            get => GetBoolSetting("Playback", "GoTrack", false);
            set => SetBoolSetting("Playback", "GoTrack", value);
        }

        public bool GoDevice
        {
            get => GetBoolSetting("Playback", "GoDevice", false);
            set => SetBoolSetting("Playback", "GoDevice", value);
        }

        public bool ShowSuggestions
        {
            get => GetBoolSetting("Playback", "ShowSuggestions", true);
            set => SetBoolSetting("Playback", "ShowSuggestions", value);
        }

        public bool ScaleToWindow
        {
            get => GetBoolSetting("Playback", "ScaleToWindow", false);
            set => SetBoolSetting("Playback", "ScaleToWindow", value);
        }

        public bool Visualiser
        {
            get => GetBoolSetting("Playback", "Visualiser", true);
            set => SetBoolSetting("Playback", "Visualiser", value);
        }

        public bool QueueButton
        {
            get => GetBoolSetting("Playback", "QueueButton", true);
            set => SetBoolSetting("Playback", "QueueButton", value);
        }

        public bool AlwaysShowControls
        {
            get => GetBoolSetting("Playback", "AlwaysShowControls", false);
            set => SetBoolSetting("Playback", "AlwaysShowControls", value);
        }
        #endregion

        #region Sidebar
        public bool ShowLabels
        {
            get => GetBoolSetting("Sidebar", "ShowLabels", false);
            set => SetBoolSetting("Sidebar", "ShowLabels", value);
        }

        public bool ColorfulIcons
        {
            get => GetBoolSetting("Sidebar", "ColorfulIcons", false);
            set => SetBoolSetting("Sidebar", "ColorfulIcons", value);
        }

        public bool ShowAtAGlance
        {
            get => GetBoolSetting("Sidebar", "ShowAtAGlance", true);
            set
            {
                SetBoolSetting("Sidebar", "ShowAtAGlance", value);
                MainPage.Current.UpdateSidebarItems(value, "Home");
            }
        }

        public bool ShowPlaylists
        {
            get => GetBoolSetting("Sidebar", "ShowPlaylists", true);
            set
            {
                SetBoolSetting("Sidebar", "ShowPlaylists", value);
                MainPage.Current.UpdateSidebarItems(value, "Playlists");
            }
        }

        public bool ShowDevices
        {
            get => GetBoolSetting("Sidebar", "ShowDevices", true);
            set
            {
                SetBoolSetting("Sidebar", "ShowDevices", value);
                MainPage.Current.UpdateSidebarItems(value, "Devices");
            }
        }

        public bool ShowSongs
        {
            get => GetBoolSetting("Sidebar", "ShowSongs", true);
            set
            {
                SetBoolSetting("Sidebar", "ShowSongs", value);
                MainPage.Current.UpdateSidebarItems(value, "Songs");
            }
        }

        public bool ShowArtists
        {
            get => GetBoolSetting("Sidebar", "ShowArtists", true);
            set
            {
                SetBoolSetting("Sidebar", "ShowArtists", value);
                MainPage.Current.UpdateSidebarItems(value, "Artists");
            }
        }

        public bool ShowAlbums
        {
            get => GetBoolSetting("Sidebar", "ShowAlbums", true);
            set
            {
                SetBoolSetting("Sidebar", "ShowAlbums", value);
                MainPage.Current.UpdateSidebarItems(value, "Albums");
            }
        }

        public bool ShowGenres
        {
            get => GetBoolSetting("Sidebar", "ShowGenres", true);
            set
            {
                SetBoolSetting("Sidebar", "ShowGenres", value);
                MainPage.Current.UpdateSidebarItems(value, "Genres");
            }
        }

        public bool ShowLocalVideos
        {
            get => GetBoolSetting("Sidebar", "ShowLocalVideos", true);
            set
            {
                SetBoolSetting("Sidebar", "ShowLocalVideos", value);
                MainPage.Current.UpdateSidebarItems(value, "LocalVideos");
            }
        }

        public bool ShowStreaming
        {
            get => GetBoolSetting("Sidebar", "ShowStreaming", true);
            set
            {
                SetBoolSetting("Sidebar", "ShowStreaming", value);
                MainPage.Current.UpdateSidebarItems(value, "Streaming");
            }
        }

        public int OpenTo
        {
            get => GetIntSetting("Sidebar", "OpenTo", 0);
            set => SetIntSetting("Sidebar", "OpenTo", value);
        }
        #endregion

        public static bool GetBoolSetting(string store, string setting, bool defaultValue)
        {
            // Get desired composite value
            ApplicationDataContainer roamingSettings = ApplicationData.Current.RoamingSettings;
            ApplicationDataCompositeValue composite = (ApplicationDataCompositeValue)roamingSettings.Values[store];

            // If the store exists, check if the setting does as well
            if (composite != null)
            {
                // Setting exists, return it
                if (composite[setting] != null)
                {
                    return (bool)composite[setting];
                }
            }
            else
            {
                // Store doesn't exist, create it
                composite = new ApplicationDataCompositeValue();
            }

            // Set the setting to the desired value and return it
            composite[setting] = defaultValue;
            roamingSettings.Values[store] = composite;
            return (bool)composite[setting];
        }

        public int GetIntSetting(string store, string setting, int defaultValue)
        {
            // Get desired composite value
            ApplicationDataContainer roamingSettings = ApplicationData.Current.RoamingSettings;
            ApplicationDataCompositeValue composite = (ApplicationDataCompositeValue)roamingSettings.Values[store];

            // If the store exists, check if the setting does as well
            if (composite != null)
            {
                // Setting exists, return it
                if (composite[setting] != null)
                {
                    return (int)composite[setting];
                }
            }
            else
            {
                // Store doesn't exist, create it
                composite = new ApplicationDataCompositeValue();
            }

            // Set the setting to the desired value and return it
            composite[setting] = defaultValue;
            roamingSettings.Values[store] = composite;
            return (int)composite[setting];
        }

        public void SetBoolSetting(string store, string setting, bool newValue)
        {
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
        }

        public void SetIntSetting(string store, string setting, int newValue)
        {
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
        }
    }
}
