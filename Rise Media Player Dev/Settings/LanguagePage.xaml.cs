using RMP.App.Common;
using RMP.App.Settings.ViewModels;
using Windows.Globalization;
using Windows.System.UserProfile;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace RMP.App.Settings
{
    public sealed partial class LanguagePage : Page
    {
        private SettingsViewModel ViewModel => App.SViewModel;

        public LanguagePage()
        {
            InitializeComponent();
            DataContext = ViewModel;

            NavigationCacheMode = NavigationCacheMode.Enabled;

            string topUserLanguage = GlobalizationPreferences.Languages[0];
            Language sys = new Language(topUserLanguage);
            SysLang.Text = sys.DisplayName;
        }

        private async void TranslateButton_Click(object sender, RoutedEventArgs e)
            => await Methods.LaunchURIAsync(URLs.Translations);

        private async void ReportButton_Click(object sender, RoutedEventArgs e)
            => await Methods.LaunchURIAsync(URLs.TranslationReports);
    }
}
