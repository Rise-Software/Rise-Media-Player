using Rise.App.ViewModels;
using Rise.Data.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace Rise.App.Views
{
    public sealed partial class NowPlayingPage : Page
    {
        private MediaPlaybackViewModel MPViewModel => App.MPViewModel;

        private bool _isHovered;

        public NowPlayingPage()
        {
            InitializeComponent();

            switch (App.SViewModel.VisualizerType)
            {
                case 0:
                    LineVis.Opacity = 0;
                    BloomVis.Visibility = Visibility.Collapsed;
                    break;
                case 1:
                    LineVis.Opacity = 1;
                    BloomVis.Visibility = Visibility.Collapsed;
                    break;
                case 2:
                    BloomVis.Visibility = Visibility.Visible;
                    LineVis.Opacity = 0;
                    break;
            }
        }

        private void Page_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            _isHovered = true;
            VisualStateManager.GoToState(this, "PointerInState", true);
        }

        private void Page_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            _isHovered = false;
            VisualStateManager.GoToState(this, "PointerOutState", true);
        }
    }
}
