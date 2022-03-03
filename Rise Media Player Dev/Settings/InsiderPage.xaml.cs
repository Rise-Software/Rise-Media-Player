using Rise.App.Common;
using Rise.App.Dialogs;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Rise.App.Settings
{
    public sealed partial class InsiderPage : Page
    {
        public InsiderPage()
        {
            InitializeComponent();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
            => _ = await URLs.Insider.LaunchAsync();

        private void ExpanderControl_Click(object sender, RoutedEventArgs e)
        {
            AllSettingsPage.Current.GOBACKPAGE.Visibility = Visibility.Visible;
            AllSettingsPage.Current.MainSettingsHeader.Text = "Wallpapers";
            AllSettingsPage.Current.MainSettingsHeaderIcon.Glyph = "\uE8B9";
            AllSettingsPage.Current.SettingsMainFrame.Navigate(typeof(InsiderWallpapers));
        }
    }
}
