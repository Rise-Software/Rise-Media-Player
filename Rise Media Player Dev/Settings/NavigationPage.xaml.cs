using Rise.App.Common;
using Rise.App.ViewModels;
using System;
using System.Collections.Generic;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Rise.App.Settings
{
    public sealed partial class NavigationPage : Page
    {
        private SettingsViewModel ViewModel => App.SViewModel;

        private readonly List<string> IconPacks = new()
        {
            ResourceLoaders.NavigationLoader.GetString("Default"),
            ResourceLoaders.NavigationLoader.GetString("Colorful")
        };

        private readonly List<string> Show = new()
        {
            ResourceLoaders.NavigationLoader.GetString("NoIcons"),
            ResourceLoaders.NavigationLoader.GetString("OnlyIcons"),
            ResourceLoaders.NavigationLoader.GetString("Everything")
        };

        private readonly List<string> Startup = new()
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

        public NavigationPage()
        {
            InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;

            InfoBar();
        }

        private async void OpenRiseMPinStartup_Toggled(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            await App.SViewModel.OpenFilesAtStartupAsync();
            InfoBar();
        }

        private void InfoBar()
        {
            switch (App.SViewModel.FLGStartupTask)
            {
                case 0:
                    //0 => Enabled / Disabled without restrictions
                    InfoBarStartup.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    InfoBarStartupLink.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    break;

                case 1:
                    //1 => Disabled by Policy
                    InfoBarStartup.Message = "This feature is disabled due to your administrator's current policies. If this is necessary, please contact your system administrator.";
                    InfoBarStartup.Visibility = Windows.UI.Xaml.Visibility.Visible;
                    InfoBarStartupLink.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    break;

                case 2:
                    //2 => Disabled by user
                    InfoBarStartup.Message = "This feature is disabled due to your current Startup settings. Click the link below to modify these settings.";
                    InfoBarStartup.Visibility = Windows.UI.Xaml.Visibility.Visible;
                    InfoBarStartupLink.Visibility = Windows.UI.Xaml.Visibility.Visible;
                    break;

                case 3:
                    //3 => Enabled by policy
                    InfoBarStartup.Message = "This feature is enabled but cannot be modified due to your administrator's current policies. If this is necessary, please contact your system administrator.";
                    InfoBarStartup.Visibility = Windows.UI.Xaml.Visibility.Visible;
                    InfoBarStartupLink.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    break;

                default:
                    break;
            }
        }

        private async void InfoBarStartupLink_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri(@"ms-settings:startupapps"));
        }
    }
}
