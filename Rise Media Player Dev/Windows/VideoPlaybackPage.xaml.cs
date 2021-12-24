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

        private DependencyPropertyWatcher<string> _watcher;

        public VideoPlaybackPage()
        {
            InitializeComponent();

            _navigationHelper = new NavigationHelper(this);

            Loaded += VideoPlaybackPage_Loaded;
            PlayerElement.SetMediaPlayer(App.PViewModel.Player);
            DataContext = ViewModel;
    }

        private void VideoPlaybackPage_Loaded(object sender, RoutedEventArgs e)
        {
            _ = new ApplicationTitleBar(AppTitleBar);
        }

        private void Page_PointerEntered(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            Player.Visibility = Visibility.Visible;
        }

        private async void Page_PointerExited(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            await Task.Run(async () =>
            {
                Thread.Sleep(3500);
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    Player.Visibility = Visibility.Collapsed;
                });
            });
        }
    }
}
