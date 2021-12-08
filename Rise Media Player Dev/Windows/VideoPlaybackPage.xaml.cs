using Rise.App.Common;
using Rise.App.ViewModels;
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
    }
}
