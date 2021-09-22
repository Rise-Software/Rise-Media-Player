namespace RMP.App.Settings
{
    public class PlaybackSettings
    {
        public static int PresetEQ
        {
            get => SettingsManager.GetIntSetting("Playback", "PresetEQ", 0);
            set => SettingsManager.SetIntSetting("Playback", "PresetEQ", value);
        }

        public static bool Crossfade
        {
            get => SettingsManager.GetBoolSetting("Playback", "Crossfade", false);
            set => SettingsManager.SetBoolSetting("Playback", "Crossfade", value);
        }

        public static int CrossfadeDuration
        {
            get => SettingsManager.GetIntSetting("Playback", "CrossfadeDuration", 0);
            set => SettingsManager.SetIntSetting("Playback", "CrossfadeDuration", value);
        }

        public static bool Gapless
        {
            get => SettingsManager.GetBoolSetting("Playback", "Gapless", false);
            set => SettingsManager.SetBoolSetting("Playback", "Gapless", value);
        }


        public static int MusicQuality
        {
            get => SettingsManager.GetIntSetting("Playback", "MusicQuality", 1);
            set => SettingsManager.SetIntSetting("Playback", "MusicQuality", value);
        }

        public static int VideoQuality
        {
            get => SettingsManager.GetIntSetting("Playback", "VideoQuality", 1);
            set => SettingsManager.SetIntSetting("Playback", "VideoQuality", value);
        }

        public static bool ReplaceFlyouts
        {
            get => SettingsManager.GetBoolSetting("Playback", "ReplaceFlyouts", false);
            set => SettingsManager.SetBoolSetting("Playback", "ReplaceFlyouts", value);
        }

        public static bool GoTrack
        {
            get => SettingsManager.GetBoolSetting("Playback", "GoTrack", false);
            set => SettingsManager.SetBoolSetting("Playback", "GoTrack", value);
        }

        public static bool GoDevice
        {
            get => SettingsManager.GetBoolSetting("Playback", "GoDevice", false);
            set => SettingsManager.SetBoolSetting("Playback", "GoDevice", value);
        }

        public bool ShowSuggestions
        {
            get => SettingsManager.GetBoolSetting("Playback", "ShowSuggestions", true);
            set => SettingsManager.SetBoolSetting("Playback", "ShowSuggestions", value);
        }

        public static bool ScaleToWindow
        {
            get => SettingsManager.GetBoolSetting("Playback", "ScaleToWindow", false);
            set => SettingsManager.SetBoolSetting("Playback", "ScaleToWindow", value);
        }

        public static bool Visualiser
        {
            get => SettingsManager.GetBoolSetting("Playback", "Visualiser", true);
            set => SettingsManager.SetBoolSetting("Playback", "Visualiser", value);
        }

        public static bool QueueButton
        {
            get => SettingsManager.GetBoolSetting("Playback", "QueueButton", true);
            set => SettingsManager.SetBoolSetting("Playback", "QueueButton", value);
        }

        public static bool AlwaysShowControls
        {
            get => SettingsManager.GetBoolSetting("Playback", "AlwaysShowControls", false);
            set => SettingsManager.SetBoolSetting("Playback", "AlwaysShowControls", value);
        }
    }
}
