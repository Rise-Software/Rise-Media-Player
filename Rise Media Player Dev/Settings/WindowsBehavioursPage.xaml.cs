using Microsoft.Toolkit.Uwp.Helpers;
using Rise.App.ViewModels;
using System;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

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
            InitializeComponent();

            switch (SystemInformation.Instance.OperatingSystemVersion.Build)
            {
                case >= 22000:
                    WindowsLogo.Glyph = "\uE336";
                    WinVer.Text = "You are running Windows 11!";
                    Update.Visibility = Visibility.Collapsed;
                    InfoString.Text = "All settings should be available.";
                    break;
                case 21996:
                    WindowsLogo.Glyph = "\uE336";
                    WinVer.Text = "You are running Windows 11!";
                    Update.Visibility = Visibility.Collapsed;
                    InfoString.Text = "Some settings might be unavailable because you are running RiseMP on the leaked build.";
                    break;
                default:
                    WindowsLogo.Glyph = "\uF144";
                    WinVer.Text = "You are running Windows 10!";
                    Update.Visibility = Visibility.Visible;
                    InfoString.Text = "Some settings might be unavailable. To use them, upgrade your PC to Windows 11.";
                    break;
            }

            VisualStateManager.GoToState(this, GetInfoBarState(), false);
        }

        private async void OpenAtStartup_Toggled(object sender, RoutedEventArgs e)
        {
            await ViewModel.OpenAtStartupAsync();
            VisualStateManager.GoToState(this, GetInfoBarState(), false);
        }

        private string GetInfoBarState() => ViewModel.FLGStartupTask switch
        {
            1 => "DisabledByPolicy",
            2 => "DisabledByUser",
            3 => "EnabledByPolicy",
            _ => "NoRestrictions",
        };

        private async void InfoBarStartupLink_Click(object sender, RoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri(@"ms-settings:startupapps"));
        }

        private async void Update_Click(object sender, RoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri(@"ms-settings:windowsupdate"));
        }
    }
}
