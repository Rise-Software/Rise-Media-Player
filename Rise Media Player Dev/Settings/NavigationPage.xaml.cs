using RMP.App.Settings.ViewModels;
using System.Collections.Generic;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace RMP.App.Settings
{
    public sealed partial class NavigationPage : Page
    {
        private SettingsViewModel ViewModel => App.SViewModel;
        private readonly List<string> IconPacks = new List<string>
        {
            ResourceLoaders.NavigationLoader.GetString("Default"),
            ResourceLoaders.NavigationLoader.GetString("Colorful")
        };

        private readonly List<string> Show = new List<string>
        {
            ResourceLoaders.NavigationLoader.GetString("NoIcons"),
            ResourceLoaders.NavigationLoader.GetString("OnlyIcons"),
            ResourceLoaders.NavigationLoader.GetString("Everything")
        };

        public NavigationPage()
        {
            InitializeComponent();
            DataContext = ViewModel;

            NavigationCacheMode = NavigationCacheMode.Enabled;
        }
    }
}
