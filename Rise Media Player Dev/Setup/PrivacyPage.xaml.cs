using Microsoft.UI.Xaml.Controls;
using System;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Rise.App.Setup
{
    public sealed partial class PrivacyPage : Page
    {
        public PrivacyPage()
        {
            InitializeComponent();
        }

        private async void OnIconLoaded(object sender, RoutedEventArgs e)
        {
            await Task.Delay(60);

            var player = (AnimatedVisualPlayer)sender;
            await player.PlayAsync(0, 0.5, false);
        }
    }
}
