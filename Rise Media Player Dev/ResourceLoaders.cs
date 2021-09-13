using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;

namespace RMP.App
{
    /// <summary>
    /// ResourceLoaders for app resources, all for view independent use.
    /// </summary>
    /// <remarks>Usage: ResourceLoaders.[Loader Name].GetString("Resource Name")</remarks>
    public class ResourceLoaders
    {
        /// <summary>
        /// Loads About resources.
        /// </summary>
        public static ResourceLoader AboutLoader =
            ResourceLoader.GetForViewIndependentUse("About");

        /// <summary>
        /// Loads Appearance resources.
        /// </summary>
        public static ResourceLoader AppearanceLoader =
            ResourceLoader.GetForViewIndependentUse("Appearance");

        /// <summary>
        /// Loads Information resources.
        /// </summary>
        public static ResourceLoader InformationLoader =
            ResourceLoader.GetForViewIndependentUse("Information");

        /// <summary>
        /// Loads Language resources.
        /// </summary>
        public static ResourceLoader LanguageLoader =
            ResourceLoader.GetForViewIndependentUse("Language");

        /// <summary>
        /// Loads MediaLibrary resources.
        /// </summary>
        public static ResourceLoader MediaLibraryLoader =
                ResourceLoader.GetForViewIndependentUse("MediaLibrary");

        /// <summary>
        /// Loads Navigation resources.
        /// </summary>
        public static ResourceLoader NavigationLoader =
                ResourceLoader.GetForViewIndependentUse("Navigation");

        /// <summary>
        /// Loads Playback resources.
        /// </summary>
        public static ResourceLoader PlaybackLoader =
                ResourceLoader.GetForViewIndependentUse("Playback");

        /// <summary>
        /// Loads Setup resources.
        /// </summary>
        public static ResourceLoader SetupLoader =
                ResourceLoader.GetForViewIndependentUse("Setup");

        /// <summary>
        /// Loads Sidebar resources.
        /// </summary>
        public static ResourceLoader SidebarLoader =
                ResourceLoader.GetForViewIndependentUse("Sidebar");
    }
}
