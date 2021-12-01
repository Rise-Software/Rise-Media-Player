using Rise.App.Common;
using Rise.App.ViewModels;
using System.Collections.Generic;
using Windows.UI.Xaml.Controls;

namespace Rise.App.Setup
{
    public sealed partial class AppearancePage : Page
    {
        private SettingsViewModel ViewModel => App.SViewModel;
        private List<string> Themes { get; set; }

        public AppearancePage()
        {
            InitializeComponent();

            Themes = new List<string>
            {
                ResourceLoaders.AppearanceLoader.GetString("Light"),
                ResourceLoaders.AppearanceLoader.GetString("Dark"),
                ResourceLoaders.AppearanceLoader.GetString("System")
            };

            DataContext = ViewModel;
        }
    }
}
