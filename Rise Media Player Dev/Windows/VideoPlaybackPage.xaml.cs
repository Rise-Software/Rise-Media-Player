using Rise.App.ViewModels;
using Windows.Media.Playback;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Rise.App.Views
{
    public sealed partial class VideoPlaybackPage : Page
    {
        private VideoPlaybackViewModel ViewModel => App.VPViewModel;

        public VideoPlaybackPage()
        {
            InitializeComponent();
            PlayerElement.SetMediaPlayer(ViewModel.Player);

            NavigationCacheMode = NavigationCacheMode.Enabled;
        }
    }
}
