using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Rise.App.ViewModels;

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
                    InfoBarStartup.Message = "This feature is disabled due to your current Startup settings. Click the link below to modify these settings, then restart the app.";
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
