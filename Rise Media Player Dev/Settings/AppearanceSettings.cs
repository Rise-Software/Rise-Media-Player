namespace RMP.App.Settings
{
    public class AppearanceSettings
    {
        public static int Theme
        {
            get => SettingsManager.GetIntSetting("Appearance", "Theme", 2);
            set => SettingsManager.SetIntSetting("Appearance", "Theme", value);
        }

        public static bool SquareAlbumArt
        {
            get => SettingsManager.GetBoolSetting("MediaLibrary", "SquareAlbumArt", false);
            set => SettingsManager.SetBoolSetting("MediaLibrary", "SquareAlbumArt", value);
        }

        public static int OpenTo
        {
            get => SettingsManager.GetIntSetting("Navigation", "OpenTo", 0);
            set => SettingsManager.SetIntSetting("Navigation", "OpenTo", value);
        }

        public static bool ShowHeaders
        {
            get => SettingsManager.GetBoolSetting("Appearance", "ShowHeaders", true);
            set => SettingsManager.SetBoolSetting("Appearance", "ShowHeaders", value);
        }

        public static bool CompactMode
        {
            get => SettingsManager.GetBoolSetting("Appearance", "CompactMode", false);
            set => SettingsManager.SetBoolSetting("Appearance", "CompactMode", value);
        }

        public static bool CommandBarAccent
        {
            get => SettingsManager.GetBoolSetting("Appearance", "CommandBarAccent", false);
            set => SettingsManager.SetBoolSetting("Appearance", "CommandBarAccent", value);
        }
    }
}
