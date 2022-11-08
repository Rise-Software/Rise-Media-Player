using CommunityToolkit.Mvvm.Input;
using Rise.Common.Helpers;
using Rise.Data.ViewModels;
using Windows.Media;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Rise.App.Views
{
    public sealed partial class VideoPlaybackPage : Page
    {
        /// <summary>
        /// Gets the <see cref="NavigationHelper"/> associated with this <see cref="Page"/>.
        /// </summary>
        private readonly NavigationHelper _navigationHelper;

        private MediaPlaybackViewModel ViewModel => App.MPViewModel;
        private bool FullScreenRequested = false;

        public VideoPlaybackPage()
        {
            InitializeComponent();
            _navigationHelper = new NavigationHelper(this);

            TitleBar.SetTitleBarForCurrentView();
            Player.SetMediaPlayer(ViewModel.Player);
        }

        [RelayCommand]
        private void EnterFullScreen()
        {
            var view = ApplicationView.GetForCurrentView();

            FullScreenRequested = view.IsFullScreenMode;

            if (!view.IsFullScreenMode)
                view.TryEnterFullScreenMode();
            else
                view.ExitFullScreenMode();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is bool fs && fs)
                FullScreenRequested = fs;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            if (FullScreenRequested)
                ApplicationView.GetForCurrentView().ExitFullScreenMode();
        }
    }
}
