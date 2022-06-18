using Rise.Data.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Rise.App.Settings
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class OnlineServicesPage : Page
    {
        private LastFMViewModel ViewModel => App.LMViewModel;

        public OnlineServicesPage()
        {
            InitializeComponent();
        }

        private async void LastFmFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            bool result = await ViewModel.TryAuthenticateAsync();
            LastFMStatus.IsEnabled = !result;

            if (result)
                ViewModel.SaveCredentialsToVault("RiseMP - LastFM account");
        }
    }
}
