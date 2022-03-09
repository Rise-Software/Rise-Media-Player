using Windows.ApplicationModel.Resources;

namespace Rise.Common
{
    /// <summary>
    /// ResourceLoaders for app resources, all for view independent use.
    /// Usage: <see cref="ResourceLoaders"/>.[Loader Name].GetString(<see cref="string"/>)
    /// </summary>
    /// <remarks>Don't add too many! They just won't work if you do due to... reasons.</remarks>
    public class ResourceLoaders
    {
        /// <summary>
        /// Loads Appearance resources.
        /// </summary>
        public static ResourceLoader AppearanceLoader { get; } =
            ResourceLoader.GetForViewIndependentUse("Appearance");

        /// <summary>
        /// Loads MediaLibrary resources.
        /// </summary>
        public static ResourceLoader MediaLibraryLoader { get; } =
            ResourceLoader.GetForViewIndependentUse("MediaLibrary");

        /// <summary>
        /// Loads MediaData resources.
        /// </summary>
        public static ResourceLoader MediaDataLoader { get; } =
            ResourceLoader.GetForViewIndependentUse("MediaData");

        /// <summary>
        /// Loads Navigation resources.
        /// </summary>
        public static ResourceLoader NavigationLoader { get; } =
            ResourceLoader.GetForViewIndependentUse("Navigation");

        /// <summary>
        /// Loads Playback resources.
        /// </summary>
        public static ResourceLoader PlaybackLoader { get; } =
            ResourceLoader.GetForViewIndependentUse("Playback");

        /// <summary>
        /// Loads Setup resources.
        /// </summary>
        public static ResourceLoader SetupLoader { get; } =
            ResourceLoader.GetForViewIndependentUse("Setup");

        /// <summary>
        /// Loads Sidebar resources.
        /// </summary>
        public static ResourceLoader SidebarLoader { get; } =
            ResourceLoader.GetForViewIndependentUse("Sidebar");
    }
}
