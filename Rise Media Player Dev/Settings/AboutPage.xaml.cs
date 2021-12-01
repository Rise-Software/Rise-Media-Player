﻿using Rise.App.Common;
using Rise.App.Dialogs;
using Windows.ApplicationModel.DataTransfer;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Rise.App.Settings
{
    public sealed partial class AboutPage : Page
    {
        private readonly DataPackage VersionData = new DataPackage();

        public AboutPage()
        {
            InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;

            VersionData.RequestedOperation = DataPackageOperation.Copy;
            VersionData.SetText("Pre-Alpha 3 - v0.0.13.0");
        }

        private async void ExpanderControl_Click(object sender, RoutedEventArgs e)
            => _ = await URLs.License.LaunchAsync();

        private void CommandBarButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            switch (button.Tag.ToString())
            {
                case "Insider":
                    _ = Frame.Navigate(typeof(InsiderPage));
                    SettingsDialogContainer.Breadcrumbs.Add
                        (ResourceLoaders.SidebarLoader.GetString("Ins"));
                    break;

                case "Version":
                    vTip.IsOpen = true;
                    break;
            }
        }

        private void VTip_CloseButtonClick(Microsoft.UI.Xaml.Controls.TeachingTip sender, object args)
            => Clipboard.SetContent(VersionData);

        private async void VTip_ActionButtonClick(Microsoft.UI.Xaml.Controls.TeachingTip sender, object args)
            => await URLs.Releases.LaunchAsync();
    }
}
