using Rise.Common.Helpers;
using Rise.Data.ViewModels;
using Windows.UI.Xaml.Controls;

namespace Rise.App.Views
{
    public sealed partial class VideoPlaybackPage : Page
    {
        /// <summary>
        /// Gets the <see cref="NavigationHelper"/> associated with this <see cref="Page"/>.
        /// </summary>
        private readonly NavigationHelper _navigationHelper;

        private MediaPlaybackViewModel ViewModel => App.MPViewModel;

        public VideoPlaybackPage()
        {
            InitializeComponent();
            _navigationHelper = new NavigationHelper(this);

            TitleBar.SetTitleBarForCurrentView();
            Player.SetMediaPlayer(ViewModel.Player);
        }
    }
}
