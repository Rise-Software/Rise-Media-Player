using RMP.App.Settings.ViewModels;
using Windows.UI.Xaml.Controls;

namespace RMP.App.Settings
{
    public sealed partial class LanguagePage : Page
    {
        private SettingsViewModel ViewModel => App.SViewModel;

        public LanguagePage()
        {
            InitializeComponent();
            DataContext = ViewModel;
        }
    }
}
