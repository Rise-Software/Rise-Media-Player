using Rise.App.Common;
using Rise.App.ViewModels;
using System;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Rise.App.Views
{
    public sealed partial class VideoPlaybackPage : Page
    {
        /// <summary>
        /// Gets the <see cref="NavigationHelper"/> associated with this <see cref="Page"/>.
        /// </summary>
        private readonly NavigationHelper _navigationHelper;

        private PlaybackViewModel ViewModel => App.PViewModel;
        private VideoViewModel CurrentVideo => ViewModel.CurrentVideo;

        private bool _currentlyFocusedOnPlaybackControls = false;

        public static VideoPlaybackPage Current;

        public VideoPlaybackPage()
        {
            InitializeComponent();

            _navigationHelper = new NavigationHelper(this);

            Loaded += VideoPlaybackPage_Loaded;
            PlayerElement.SetMediaPlayer(App.PViewModel.Player);
            DataContext = ViewModel;
            Current = this;
        }

        private void VideoPlaybackPage_Loaded(object sender, RoutedEventArgs e)
        {
            _ = new ApplicationTitleBar(AppTitleBar);
        }

        private async void Page_PointerExited(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            if (Player.Visibility == Visibility.Visible && TopGrid.Visibility == Visibility.Visible && !_currentlyFocusedOnPlaybackControls)
            {
                await Task.Run(async () =>
                {
                    Thread.Sleep(3500);
                    await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                    {
                        ExitPointerStoryboard.Begin();
                        Player.Visibility = Visibility.Collapsed;
                        TopGrid.Visibility = Visibility.Collapsed;
                    });
                });
            }
        }

        private async void Page_PointerMoved(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            if (Player.Visibility == Visibility.Collapsed && TopGrid.Visibility == Visibility.Collapsed)
            {
                EnterPointerStoryboard.Begin();
                Player.Visibility = Visibility.Visible;
                TopGrid.Visibility = Visibility.Visible;
                if (!_currentlyFocusedOnPlaybackControls)
                {
                    await Task.Run(async () =>
                    {
                        Thread.Sleep(3500);
                        await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                        {
                            ExitPointerStoryboard.Begin();
                            Player.Visibility = Visibility.Collapsed;
                            TopGrid.Visibility = Visibility.Collapsed;
                        });
                    });
                }
            }
        }

        private void TopGrid_PointerEntered(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            _currentlyFocusedOnPlaybackControls = true;
        }

        private void TopGrid_PointerExited(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            _currentlyFocusedOnPlaybackControls = false;
        }
    }
}
