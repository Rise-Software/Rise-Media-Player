using RMP.App.Settings.ViewModels;
using System.Collections.Generic;
using Windows.UI.Xaml.Controls;

namespace RMP.App.Settings
{
    public sealed partial class NavigationPage : Page
    {
        private SettingsViewModel ViewModel => App.SViewModel;
        private List<string> IconPacks { get; set; }
        private List<string> Show { get; set; }

        public NavigationPage()
        {
            InitializeComponent();
            Show = new List<string>
            {
                ResourceLoaders.NavigationLoader.GetString("NoIcons"),
                ResourceLoaders.NavigationLoader.GetString("OnlyIcons"),
                ResourceLoaders.NavigationLoader.GetString("Everything")
            };

            IconPacks = new List<string>
            {
                ResourceLoaders.NavigationLoader.GetString("Default"),
                ResourceLoaders.NavigationLoader.GetString("Colorful")
            };

            DataContext = ViewModel;
        }
    }
}
