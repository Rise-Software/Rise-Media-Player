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
        /// Loads MediaLibrary resources.
        /// </summary>
        public static ResourceLoader MediaLibraryLoader { get; } =
            ResourceLoader.GetForViewIndependentUse("MediaLibrary");

        /// <summary>
        /// Loads Navigation resources.
        /// </summary>
        public static ResourceLoader NavigationLoader { get; } =
            ResourceLoader.GetForViewIndependentUse("Navigation");
    }
}
