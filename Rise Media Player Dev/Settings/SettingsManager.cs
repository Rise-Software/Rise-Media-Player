using Windows.Storage;

namespace RMP.App.Settings
{
    public class SettingsManager
    {
        /// <summary>
        /// Gets a bool app setting.
        /// </summary>
        /// <param name="store">Setting store name.</param>
        /// <param name="setting">Setting name.</param>
        /// <param name="defaultValue">Default setting value.</param>
        /// <returns>Bool app setting value.</returns>
        /// <remarks>If the store parameter is "Local", a local setting will be returned.</remarks>
        public static bool GetBoolSetting(string store, string setting, bool defaultValue)
        {
            // If store == "Local", get a local setting
            if (store == "Local")
            {
                // Get app settings
                ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;

                // Check if the setting exists
                if (localSettings.Values[setting] != null)
                {
                    return (bool)localSettings.Values[setting];
                }

                // Set the setting to the desired value and return it
                localSettings.Values[setting] = defaultValue;
                return (bool)localSettings.Values[setting];
            }

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

        /// <summary>
        /// Gets an int app setting.
        /// </summary>
        /// <param name="store">Setting store name.</param>
        /// <param name="setting">Setting name.</param>
        /// <param name="defaultValue">Default setting value.</param>
        /// <returns>Int app setting value.</returns>
        /// <remarks>If the store parameter is "Local", a local setting will be returned.</remarks>
        public static int GetIntSetting(string store, string setting, int defaultValue)
        {
            // If store == "Local", get a local setting
            if (store == "Local")
            {
                // Get app settings
                ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;

                // Check if the setting exists
                if (localSettings.Values[setting] != null)
                {
                    return (int)localSettings.Values[setting];
                }

                // Set the setting to the desired value and return it
                localSettings.Values[setting] = defaultValue;
                return (int)localSettings.Values[setting];
            }

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

        /// <summary>
        /// Sets a bool app setting.
        /// </summary>
        /// <param name="store">Setting store name.</param>
        /// <param name="setting">Setting name.</param>
        /// <param name="newValue">New setting value.</param>
        /// <remarks>If the store parameter is "Local", a local setting will be set.</remarks>
        public static void SetBoolSetting(string store, string setting, bool newValue)
        {
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
        }

        /// <summary>
        /// Sets an int app setting.
        /// </summary>
        /// <param name="store">Setting store name.</param>
        /// <param name="setting">Setting name.</param>
        /// <param name="newValue">New setting value.</param>
        /// <remarks>If the store parameter is "Local", a local setting will be set.</remarks>
        public static void SetIntSetting(string store, string setting, int newValue)
        {
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
        }
    }
}
