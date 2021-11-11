using Rise.App.Common;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace Rise.App.Dialogs
{
    public sealed partial class UnavailableDialog : ContentDialog
    {
        public UnavailableDialog()
        {
            InitializeComponent();
        }

        public BitmapImage BackgroundImage { get; set; }
        public BitmapImage LeftHero { get; set; }
        public BitmapImage CenterHero { get; set; }
        public BitmapImage RightHero { get; set; }
        public string Header { get; set; }
        public string Description { get; set; }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
            => Hide();

        private async void Button_Click(object sender, RoutedEventArgs e)
            => _ = await URLs.GitHub.LaunchAsync();
    }
}
