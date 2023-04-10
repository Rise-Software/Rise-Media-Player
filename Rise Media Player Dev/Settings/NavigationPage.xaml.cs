using Rise.App.ViewModels;
using Rise.Common.Extensions.Markup;
using Rise.Data.Navigation;
using Rise.Data.Sources;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Rise.App.Settings
{
    public sealed partial class NavigationPage : Page
    {
        private NavViewDataSource NavDataSource => App.NavDataSource;
        private SettingsViewModel ViewModel => App.SViewModel;

        private readonly List<IconPack> IconPacks = new()
        {
            new(string.Empty, ResourceHelper.GetString("Default")),
            new("Colorful", ResourceHelper.GetString("Colorful"))
        };

        private readonly List<string> Show = new()
        {
            ResourceHelper.GetString("NoIcons"),
            ResourceHelper.GetString("OnlyIcons"),
            ResourceHelper.GetString("Everything")
        };

        private readonly List<string> Startup = new()
        {
            ResourceHelper.GetString("Home"),
            ResourceHelper.GetString("Playlists"),
            ResourceHelper.GetString("Songs"),
            ResourceHelper.GetString("Artists"),
            ResourceHelper.GetString("Albums"),
            ResourceHelper.GetString("LocalVideos"),
        };

        public NavigationPage()
        {
            InitializeComponent();
        }

        private void IconPackComboBox_Loaded(object sender, RoutedEventArgs e)
        {
            var selected = IconPacks.FirstOrDefault(p => p.Id == ViewModel.IconPack);
            if (selected == null)
                IconPackComboBox.SelectedIndex = 0;
            else
                IconPackComboBox.SelectedItem = selected;
        }

        private void IconPackComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selected = (IconPack)IconPackComboBox.SelectedItem;
            ViewModel.IconPack = selected.Id;
        }
    }
}
