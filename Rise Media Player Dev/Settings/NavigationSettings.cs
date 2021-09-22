using RMP.App.Dialogs;

namespace RMP.App.Settings
{
    public class NavigationSettings
    {
        public static int Resize
        {
            get => SettingsManager.GetIntSetting("Navigation", "Resize", 0);
            set => SettingsManager.SetIntSetting("Navigation", "Resize", value);
        }

        public static bool Hamburger
        {
            get => SettingsManager.GetBoolSetting("Navigation", "Hamburger", false);
            set => SettingsManager.SetBoolSetting("Navigation", "Hamburger", value);
        }

        public static bool ShowLabels
        {
            get => SettingsManager.GetBoolSetting("Navigation", "ShowLabels", false);
            set => SettingsManager.SetBoolSetting("Navigation", "ShowLabels", value);
        }

        public static bool ColorfulIcons
        {
            get => SettingsManager.GetBoolSetting("Navigation", "ColorfulIcons", false);
            set
            {
                SettingsManager.SetBoolSetting("Navigation", "ColorfulIcons", value);
                MainPage.Current.UpdateIconColor(value);
                SettingsDialog.Current.UpdateIconColor(value);
            }
        }

        public static bool ShowAtAGlance
        {
            get => SettingsManager.GetBoolSetting("Navigation", "ShowAtAGlance", true);
            set
            {
                SettingsManager.SetBoolSetting("Navigation", "ShowAtAGlance", value);
                MainPage.Current.UpdateSidebarItems(value, "Home");
            }
        }

        public static bool ShowPlaylists
        {
            get => SettingsManager.GetBoolSetting("Navigation", "ShowPlaylists", true);
            set
            {
                SettingsManager.SetBoolSetting("Navigation", "ShowPlaylists", value);
                MainPage.Current.UpdateSidebarItems(value, "Playlists");
            }
        }

        public static bool ShowDevices
        {
            get => SettingsManager.GetBoolSetting("Navigation", "ShowDevices", true);
            set
            {
                SettingsManager.SetBoolSetting("Navigation", "ShowDevices", value);
                MainPage.Current.UpdateSidebarItems(value, "Devices");
            }
        }

        public static bool ShowSongs
        {
            get => SettingsManager.GetBoolSetting("Navigation", "ShowSongs", true);
            set
            {
                SettingsManager.SetBoolSetting("Navigation", "ShowSongs", value);
                MainPage.Current.UpdateSidebarItems(value, "Songs");
            }
        }

        public static bool ShowArtists
        {
            get => SettingsManager.GetBoolSetting("Navigation", "ShowArtists", true);
            set
            {
                SettingsManager.SetBoolSetting("Navigation", "ShowArtists", value);
                MainPage.Current.UpdateSidebarItems(value, "Artists");
            }
        }

        public static bool ShowAlbums
        {
            get => SettingsManager.GetBoolSetting("Navigation", "ShowAlbums", true);
            set
            {
                SettingsManager.SetBoolSetting("Navigation", "ShowAlbums", value);
                MainPage.Current.UpdateSidebarItems(value, "Albums");
            }
        }

        public static bool ShowGenres
        {
            get => SettingsManager.GetBoolSetting("Navigation", "ShowGenres", true);
            set
            {
                SettingsManager.SetBoolSetting("Navigation", "ShowGenres", value);
                MainPage.Current.UpdateSidebarItems(value, "Genres");
            }
        }

        public static bool ShowLocalVideos
        {
            get => SettingsManager.GetBoolSetting("Navigation", "ShowLocalVideos", true);
            set
            {
                SettingsManager.SetBoolSetting("Navigation", "ShowLocalVideos", value);
                MainPage.Current.UpdateSidebarItems(value, "LocalVideos");
            }
        }

        public static bool ShowStreaming
        {
            get => SettingsManager.GetBoolSetting("Navigation", "ShowStreaming", true);
            set
            {
                SettingsManager.SetBoolSetting("Navigation", "ShowStreaming", value);
                MainPage.Current.UpdateSidebarItems(value, "Streaming");
            }
        }
    }
}
