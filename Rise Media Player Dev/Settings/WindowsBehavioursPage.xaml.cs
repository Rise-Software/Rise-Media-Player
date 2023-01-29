using Microsoft.Toolkit.Uwp.Helpers;
using Rise.App.ViewModels;
using Rise.Common.Extensions.Markup;
using System;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Rise.App.Settings
{
    public sealed partial class WindowsBehavioursPage : Page
    {
        private SettingsViewModel ViewModel => App.SViewModel;

        public WindowsBehavioursPage()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            LoadInfoBarState();
            LoadWindowsVersionState(SystemInformation.Instance.OperatingSystemVersion.Build);
        }

        private async void OpenAtStartup_Toggled(object sender, RoutedEventArgs e)
        {
            await ViewModel.OpenAtStartupAsync();
            LoadInfoBarState();
        }

        private async void InfoBarStartupLink_Click(object sender, RoutedEventArgs e)
            => _ = await Launcher.LaunchUriAsync(new Uri(@"ms-settings:startupapps"));

        private async void Update_Click(object sender, RoutedEventArgs e)
            => _ = await Launcher.LaunchUriAsync(new Uri(@"ms-settings:windowsupdate"));

        private void LoadInfoBarState()
        {
            switch (ViewModel.FLGStartupTask)
            {
                case 1:
                    //DisabledByPolicy
                    InfoBarStartup.Visibility = Visibility.Visible;
                    InfoBarStartup.Message = ResourceHelper.GetString("/Settings/SystemBehaviorsDisabledByPolicy");
                    InfoBarStartupLink.Visibility = Visibility.Collapsed;
                    break;

                case 2:
                    //DisabledByUser
                    InfoBarStartup.Visibility = Visibility.Visible;
                    InfoBarStartup.Message = ResourceHelper.GetString("/Settings/SystemBehaviorsDisabledByUser");
                    InfoBarStartupLink.Visibility = Visibility.Visible;
                    break;

                case 3:
                    //EnabledByPolicy
                    InfoBarStartup.Visibility = Visibility.Visible;
                    InfoBarStartup.Message = ResourceHelper.GetString("/Settings/SystemBehaviorsEnabledByPolicy");
                    InfoBarStartupLink.Visibility = Visibility.Visible;
                    break;

                default:
                    //NoRestrictions
                    InfoBarStartup.Visibility = Visibility.Collapsed;
                    InfoBarStartupLink.Visibility = Visibility.Visible;
                    break;
            }
        }

        private void LoadWindowsVersionState(ushort version)
        {
            switch (version)
            {
                case >= 22000:
                    //Windows 11
                    WindowsLogo.Glyph = "\xE336";
                    WinVer.Text = ResourceHelper.GetString("/Settings/SystemBehaviorsRunningWin11H");
                    InfoString.Text = ResourceHelper.GetString("/Settings/SystemBehaviorsRunningWin11Desc");
                    Update.Visibility = Visibility.Collapsed;
                    break;

                case >= 21996:
                    //Leaked Build
                    WindowsLogo.Glyph = "\xE336";
                    WinVer.Text = ResourceHelper.GetString("/Settings/SystemBehaviorsRunningWin11H");
                    InfoString.Text = ResourceHelper.GetString("/Settings/SystemBehaviorsRunningLeakedDesc");
                    Update.Visibility = Visibility.Visible;
                    break;

                default:
                    //Windows 10
                    WindowsLogo.Glyph = "\xF23F";
                    WinVer.Text = ResourceHelper.GetString("/Settings/SystemBehaviorsRunningWin10H");
                    InfoString.Text = ResourceHelper.GetString("/Settings/SystemBehaviorsRunningWin10Desc");
                    Update.Visibility = Visibility.Visible;
                    break;
            }
        }
    }
}
