using Rise.App.ViewModels;
using Windows.Storage;

namespace Rise.App.Settings
{
    public class SettingsManager : ViewModel
    {
        /// <summary>
        /// Gets an app setting.
        /// </summary>
        /// <param name="store">Setting store name.</param>
        /// <param name="setting">Setting name.</param>
        /// <param name="defaultValue">Default setting value.</param>
        /// <returns>App setting value.</returns>
        /// <remarks>If the store parameter is "Local", a local setting will be returned.</remarks>
        public object Get(string store, string setting, object defaultValue)
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

                // Set the setting to the desired value and return it
                return localSettings.Values[setting];
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

            // Set the setting to the desired value and return it
            return composite[setting];
        }

        /// <summary>
        /// Sets an app setting.
        /// </summary>
        /// <param name="store">Setting store name.</param>
        /// <param name="setting">Setting name.</param>
        /// <param name="newValue">New setting value.</param>
        /// <remarks>If the store parameter is "Local", a local setting will be set.</remarks>
        public void Set(string store, string setting, object newValue)
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

            OnPropertyChanged(setting);
        }
    }
}
