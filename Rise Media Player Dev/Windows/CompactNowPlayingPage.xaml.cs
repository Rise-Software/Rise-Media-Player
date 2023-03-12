using Rise.Common.Threading;
using Rise.Data.ViewModels;
using System;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

namespace Rise.App.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CompactNowPlayingPage : Page
    {
        private MediaPlaybackViewModel MPViewModel => App.MPViewModel;

        public CompactNowPlayingPage()
        {
            InitializeComponent();
            TitleBar.SetTitleBarForCurrentView();

            _ = VisualStateManager.GoToState(this, nameof(PointerOutState), false);
        }

        private void OnPlayerLoaded(object sender, RoutedEventArgs e)
        {
            MainPlayer.SetMediaPlayer(MPViewModel.Player);
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            _ = await ApplicationView.GetForCurrentView().
                TryEnterViewModeAsync(ApplicationViewMode.CompactOverlay);
        }

        protected override async void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            _ = await ApplicationView.GetForCurrentView().
                TryEnterViewModeAsync(ApplicationViewMode.Default);
        }
    }

    // Event handlers
    public sealed partial class CompactNowPlayingPage
    {
        private void OnExitButtonClick(object sender, RoutedEventArgs e)
        {
            if (Frame.CanGoBack)
                Frame.GoBack();
        }

        private void OnPointerEntered(object sender, PointerRoutedEventArgs e)
        {
            _ = VisualStateManager.GoToState(this, nameof(PointerInState), true);
        }

        private void OnPointerExited(object sender, PointerRoutedEventArgs e)
        {
            _ = VisualStateManager.GoToState(this, nameof(PointerOutState), true);
        }
    }
}
