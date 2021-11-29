using Rise.App.Common;
using Rise.App.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Rise.App.Views
{
    public sealed partial class VideoPlaybackPage : Page
    {
        /// <summary>
        /// Gets the <see cref="NavigationHelper"/> associated with this <see cref="Page"/>.
        /// </summary>
        private readonly NavigationHelper _navigationHelper;

        private PlaybackViewModel ViewModel => App.PViewModel;
        private DependencyPropertyWatcher<string> _watcher;

        public VideoPlaybackPage()
        {
            InitializeComponent();
            PlayerElement.SetMediaPlayer(ViewModel.Player);

            _navigationHelper = new NavigationHelper(this);

            Loaded += VideoPlaybackPage_Loaded;
            Unloaded += VideoPlaybackPage_Unloaded;
        }

        private void VideoPlaybackPage_Loaded(object sender, RoutedEventArgs e)
        {
            _ = new ApplicationTitleBar(AppTitleBar);
            Grid panelGrid = MediaControls.FindVisualChild<Grid>("ControlPanelGrid");

            if (panelGrid != null)
            {
                Transform render = panelGrid.RenderTransform;

                _watcher = new DependencyPropertyWatcher<string>(render, "Y");
                _watcher.PropertyChanged += Watcher_PropertyChanged;
            }
        }

        private void VideoPlaybackPage_Unloaded(object sender, RoutedEventArgs e)
        {
            _watcher.PropertyChanged -= Watcher_PropertyChanged;
            _watcher.Dispose();
        }

        private void Watcher_PropertyChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            double val = double.Parse(e.NewValue.ToString());
            if (val == 50)
            {
                TopGrid.Margin = new Thickness(0, -48, 0, 0);
            }
            else if (val == 0.5)
            {
                TopGrid.Margin = new Thickness(0);
            }
        }
    }
}
