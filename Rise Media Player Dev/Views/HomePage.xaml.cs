using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace RMP.App.Views
{
    public sealed partial class HomePage : Page
    {
        public HomePage()
        {
            InitializeComponent();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            _ = await Methods.LaunchURI(URLs.Feedback);
        }
    }
}