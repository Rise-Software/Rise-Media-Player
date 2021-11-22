using Rise.App.Common;
using Rise.App.Dialogs;
using Rise.App.Settings.ViewModels;
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

        private readonly List<string> ColorThemes = new List<string>
        {
            "No tint",
            "Use system accent color",
            "Rise"
        };

        private readonly List<string> Startup = new List<string>
        {
            ResourceLoaders.AppearanceLoader.GetString("Home"),
            ResourceLoaders.AppearanceLoader.GetString("Playlists"),
            ResourceLoaders.AppearanceLoader.GetString("Devices"),
            ResourceLoaders.AppearanceLoader.GetString("Songs"),
            ResourceLoaders.AppearanceLoader.GetString("Artists"),
            ResourceLoaders.AppearanceLoader.GetString("Albums"),
            ResourceLoaders.AppearanceLoader.GetString("Genres"),
            ResourceLoaders.AppearanceLoader.GetString("LocalVideos"),
            ResourceLoaders.AppearanceLoader.GetString("Streaming"),
            ResourceLoaders.AppearanceLoader.GetString("NowPlaying")
        };

        public AppearancePage()
        {
            InitializeComponent();

            DataContext = ViewModel;
            NavigationCacheMode = NavigationCacheMode.Enabled;
        }

        private void SidebarCustomize_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(NavigationPage));

            SettingsDialogContainer.Breadcrumbs.
                Add(ResourceLoaders.AppearanceLoader.GetString("Sidebar"));
        }

        private void ColorThemeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (ColorThemeComboBox.SelectedIndex)
            {
                case 0:
                    RiseColorsStackPanel.Visibility = Visibility.Collapsed;
                    break;
                case 1:
                    RiseColorsStackPanel.Visibility = Visibility.Collapsed;
                    break;
                case 2:
                    RiseColorsStackPanel.Visibility = Visibility.Visible;
                    break;
            }
        }
    }
}
