using System;
using Windows.UI.Xaml.Controls;
using Rise.App.ViewModels;
using Windows.System.Profile;
using System.Diagnostics;
using Microsoft.Toolkit.Uwp.Helpers;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Rise.App.Settings
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class WindowsBehavioursPage : Page
    {
        private SettingsViewModel ViewModel => App.SViewModel;

        public WindowsBehavioursPage()
        {
            this.InitializeComponent();
            var v = Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamilyVersion;
            var build = (Convert.ToUInt64(v) & Convert.ToUInt64(0x00000000FFFF0000)) >> 16;
            build = 19000;
            Debug.WriteLine(build);

            if (build >= 22000)
            {
                WindowsLogo.Glyph = "\uE336";
                WinVer.Text = "You are running Windows 11!";
                Update.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                InfoString.Text = "All settings should be available.";
            }
            else
            {
                WindowsLogo.Glyph = "\uF144";
                WinVer.Text = "You are running Windows 10!";
                Update.Visibility = Windows.UI.Xaml.Visibility.Visible;
                InfoString.Text = "Some settings are unavailable. To use them, upgrade your PC to Windows 11.";
            }

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
                    InfoBarStartup.Message = "This feature is disabled due to your current startup settings. Click the link below to modify these settings, then restart the app.";
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

        private async void Update_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri(@"ms-settings:windowsupdate"));
        }
    }
}
