namespace RMP.App.Settings
{
    public class SetupSettings
    {
        public static bool SetupCompleted
        {
            get => SettingsManager.GetBoolSetting("Local", "SetupCompleted", false);
            set => SettingsManager.SetBoolSetting("Local", "SetupCompleted", value);
        }

        public static int SetupProgress
        {
            get => SettingsManager.GetIntSetting("Local", "SetupProgress", 0);
            set => SettingsManager.SetIntSetting("Local", "SetupProgress", value);
        }
    }
}
