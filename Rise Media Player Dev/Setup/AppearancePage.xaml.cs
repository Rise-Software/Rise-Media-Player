using Rise.App.Common;
using Rise.App.ViewModels;
using System;
using System.Collections.Generic;
using Windows.ApplicationModel.Core;
using Windows.UI.Xaml.Controls;


namespace Rise.App.Setup
{
    public sealed partial class AppearancePage : Page
    { 
        private SettingsViewModel ViewModel => App.SViewModel;

        private readonly List<string> Themes = new()
        {
            ResourceLoaders.AppearanceLoader.GetString("Light"),
            ResourceLoaders.AppearanceLoader.GetString("Dark"),
            ResourceLoaders.AppearanceLoader.GetString("System")
        };

        public AppearancePage()
        {
            InitializeComponent();
            ChangeThemeTip.IsOpen = false;


            DataContext = ViewModel;
        }

        private async void ChangeThemeTip_ActionButtonClick(Microsoft.UI.Xaml.Controls.TeachingTip sender, object args)
        {
            await CoreApplication.RequestRestartAsync("Theme changed");
        }

        private void ThemeChange_DropDownClosed(object sender, object e)
        {
            ChangeThemeTip.IsOpen = true;
        }


    }
}
