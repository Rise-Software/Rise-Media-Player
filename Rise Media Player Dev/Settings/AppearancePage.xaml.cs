using Rise.App.Common;
using Rise.App.Dialogs;
using Rise.App.ViewModels;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Rise.App.Settings
{
    public sealed partial class AppearancePage : Page
    {
        private SettingsViewModel ViewModel => App.SViewModel;

        private readonly List<string> Themes = new List<string>
        {
            ResourceLoaders.AppearanceLoader.GetString("Light"),
            ResourceLoaders.AppearanceLoader.GetString("Dark"),
            ResourceLoaders.AppearanceLoader.GetString("System")
        };

        private readonly List<string> Startup = new List<string>
        {
            ResourceLoaders.AppearanceLoader.GetString("Home"),
            ResourceLoaders.AppearanceLoader.GetString("NowPlaying"),
            ResourceLoaders.AppearanceLoader.GetString("Playlists"),
            ResourceLoaders.AppearanceLoader.GetString("Songs"),
            ResourceLoaders.AppearanceLoader.GetString("Artists"),
            ResourceLoaders.AppearanceLoader.GetString("Albums"),
            ResourceLoaders.AppearanceLoader.GetString("Genres"),
            ResourceLoaders.AppearanceLoader.GetString("LocalVideos"),
        };

        public AppearancePage()
        {
            InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;
        }

        private void SidebarCustomize_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(NavigationPage));

            SettingsDialogContainer.Breadcrumbs.
                Add(ResourceLoaders.AppearanceLoader.GetString("Sidebar"));
        }
    }
}
