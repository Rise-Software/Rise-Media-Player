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

        public NowPlayingPage()
        {
            InitializeComponent();

            switch (App.SViewModel.VisualizerType)
            {
                case 0:
                    LineVis.Visibility = Visibility.Collapsed;
                    BloomVis.Visibility = Visibility.Collapsed;
                    break;
                case 1:
                    LineVis.Visibility = Visibility.Visible;
                    BloomVis.Visibility = Visibility.Collapsed;
                    break;
                case 2:
                    BloomVis.Visibility = Visibility.Visible;
                    LineVis.Visibility = Visibility.Collapsed;
                    break;
            }
        }

        private void Page_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, "PointerInState", true);
        }

        private void Page_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, "PointerOutState", true);
        }
    }
}
