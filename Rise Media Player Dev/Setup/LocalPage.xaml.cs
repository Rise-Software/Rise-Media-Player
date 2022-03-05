using Rise.App.Common;
using Rise.App.ViewModels;
using System.Collections.Generic;
using Windows.UI.Xaml.Controls;

namespace Rise.App.Setup
{
    public sealed partial class LocalPage : Page
    {
        private SettingsViewModel ViewModel => App.SViewModel;
        private List<string> Deletion { get; set; }

        public LocalPage()
        {
            InitializeComponent();
            SetupLocalFrame.Navigate(typeof(Settings.MediaSourcesPage));

            Deletion = new List<string>
            {
                ResourceLoaders.MediaLibraryLoader.GetString("OnlyApp"),
                ResourceLoaders.MediaLibraryLoader.GetString("Device")
            };

            DataContext = ViewModel;
        }

        private async void ExpanderControl_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {

        }
    }
}
