using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace RMP.App.Common
{
    /// <summary>
    /// Methods that don't really fit anywhere else or can be used in multiple
    /// parts of the app, usually simple stuff like launching URIs and whatnot.
    /// </summary>
    public class Methods
    {
        /// <summary>
        /// Launchs an URI from a string.
        /// </summary>
        /// <param name="str">The URI string.</param>
        /// <returns>Whether or not the launch was successful.</returns>
        public static async Task<bool> LaunchURI(string str)
        {
            return await Launcher.LaunchUriAsync(new Uri(str));
        }
    }
}
