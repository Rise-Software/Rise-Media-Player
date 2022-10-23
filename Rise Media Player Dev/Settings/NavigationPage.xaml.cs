using Rise.App.ViewModels;
using Rise.Common;
using Rise.Common.Extensions.Markup;
using Rise.Data.Sources;
using System.Collections.Generic;
using Windows.UI.Xaml.Controls;

namespace Rise.App.Settings
{
    public sealed partial class NavigationPage : Page
    {
        private NavViewDataSource NavDataSource => App.NavDataSource;
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
            ResourceHelper.GetString("Home"),
            ResourceHelper.GetString("Playlists"),
            ResourceHelper.GetString("Songs"),
            ResourceHelper.GetString("Artists"),
            ResourceHelper.GetString("Albums"),
            ResourceHelper.GetString("Genres"),
            ResourceHelper.GetString("LocalVideos"),
        };

        public NavigationPage()
        {
            InitializeComponent();
        }

        private void IconStyle_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            NavDataSource.ChangeIconPack(ViewModel.CurrentPack);
        }
    }
}
