using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace Rise.Plugins
{
    public class PluginSettings
    {
        public string PluginId { get; }

        /// <summary>
        /// Gets a plugin setting.
        /// </summary>
        /// <param name="defaultValue">Default setting value.</param>
        /// <param name="setting">Setting name.</param>
        /// <returns>App setting value.</returns>
        public Type Get<Type>(Type defaultValue, [CallerMemberName] string setting = null)
        {
            // Get desired composite value
            ApplicationDataContainer roamingSettings = ApplicationData.Current.RoamingSettings;
            ApplicationDataCompositeValue composite = (ApplicationDataCompositeValue)roamingSettings.Values[Common.Constants.PluginStore.PluginStoreName];

            // If the store exists, check if the setting does as well
            composite ??= new ApplicationDataCompositeValue();

            string settingFullName = $"{PluginId}/Settings/{setting}";

            if (composite[settingFullName] == null)
            {
                composite[settingFullName] = defaultValue;
                roamingSettings.Values[Common.Constants.PluginStore.PluginStoreName] = composite;
            }

            object value = composite[settingFullName];

            // Return the setting if type matches
            if (value is not Type)
            {
                string format = "Type mismatch for \"{0}\" in local store. Current type is {1}";
                string message = string.Format(format, settingFullName, value.GetType());

                throw new ArgumentException(message);
            }

            return (Type)value;
        }

        /// <summary>
        /// Sets a plugin setting.
        /// </summary>
        /// <param name="newValue">New setting value.</param>
        /// <param name="setting">Setting name.</param>
        private void Set<Type>(Type newValue, [CallerMemberName] string setting = null)
        {
            // Try to get the setting, if types don't match, it'll throw an exception
            _ = Get(newValue, setting);


            string settingFullName = $"{PluginId}/Settings/{setting}";

            // Get desired composite value
            ApplicationDataContainer roamingSettings = ApplicationData.Current.RoamingSettings;
            ApplicationDataCompositeValue composite = (ApplicationDataCompositeValue)roamingSettings.Values[Common.Constants.PluginStore.PluginStoreName];

            // Store doesn't exist, create it
            composite ??= new ApplicationDataCompositeValue();

            // Set the setting to the desired value
            composite[settingFullName] = newValue;
            roamingSettings.Values[Common.Constants.PluginStore.PluginStoreName] = composite;
        }
    }
}
