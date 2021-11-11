using Rise.App.Settings.ViewModels;
using Windows.UI.Xaml.Controls;

namespace Rise.App.Setup
{
    public sealed partial class PrivacyPage : Page
    {
        private SettingsViewModel ViewModel => App.SViewModel;

        public PrivacyPage()
        {
            InitializeComponent();
            DataContext = ViewModel;
        }
    }
}
