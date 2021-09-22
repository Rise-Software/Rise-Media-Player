namespace RMP.App.Settings
{
    public class LanguageSettings
    {
        public static int Language
        {
            get => SettingsManager.GetIntSetting("Language", "Language", 0);
            set => SettingsManager.SetIntSetting("Language", "Language", value);
        }
    }
}
