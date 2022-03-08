using Rise.App.ViewModels;
using Rise.Common;
using System.Collections.Generic;
using Windows.UI.Xaml.Controls;

namespace Rise.App.Setup
{
    public sealed partial class LocalPage : Page
    {
        private SettingsViewModel ViewModel => App.SViewModel;
        private List<string> Deletion { get; set; }
        public static LocalPage Current;


        public LocalPage()
        {
            InitializeComponent();
            Current = this;
            SetupLocalFrame.Navigate(typeof(Settings.MediaSourcesPage));

            Deletion = new List<string>
            {
                ResourceLoaders.MediaLibraryLoader.GetString("OnlyApp"),
                ResourceLoaders.MediaLibraryLoader.GetString("Device")
            };

            DataContext = ViewModel;
        }

        private void ExpanderControl_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {

        }
    }
}
