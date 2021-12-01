using Rise.App.ViewModels;
using System;
using Windows.Storage;

namespace Rise.App.Settings
{
    public abstract class SettingsManager : ViewModel
    {
        /// <summary>
        /// Gets an app setting.
        /// </summary>
        /// <param name="store">Setting store name.</param>
        /// <param name="setting">Setting name.</param>
        /// <param name="defaultValue">Default setting value.</param>
        /// <returns>App setting value.</returns>
        /// <remarks>If the store parameter is "Local", a local setting will be returned.</remarks>
        protected T Get<T>(string store, string setting, T defaultValue)
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
                if (!(val is T))
                {
                    throw new ArgumentException("Type mismatch for \"" + setting + "\" in local store. Got " + val.GetType());
                }
                return (T)val;
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
            if (!(value is T))
            {
                throw new ArgumentException("Type mismatch for \"" + setting + "\" in store \"" + store + "\". Current type is " + value.GetType());
            }
            return (T)value;
        }

        /// <summary>
        /// Sets an app setting.
        /// </summary>
        /// <param name="store">Setting store name.</param>
        /// <param name="setting">Setting name.</param>
        /// <param name="newValue">New setting value.</param>
        /// <remarks>If the store parameter is "Local", a local setting will be set.</remarks>
        protected void Set<T>(string store, string setting, T newValue)
        {
            // Try to get the setting, if types don't match, it'll throw an exception
            _ = Get(store, setting, newValue);

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
