using RMP.App.Settings.ViewModels;
using System.Collections.Generic;
using Windows.UI.Xaml.Controls;

namespace RMP.App.Setup
{
    public sealed partial class LocalPage : Page
    {
        private SettingsViewModel ViewModel => App.SViewModel;
        private List<string> Deletion { get; set; }

        public LocalPage()
        {
            InitializeComponent();

            Deletion = new List<string>
            {
                ResourceLoaders.MediaLibraryLoader.GetString("OnlyApp"),
                ResourceLoaders.MediaLibraryLoader.GetString("Device")
            };

            DataContext = ViewModel;
        }
    }
}
