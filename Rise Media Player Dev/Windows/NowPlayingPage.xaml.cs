using Rise.App.ViewModels;
using Rise.Data.ViewModels;
using System;
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

            MainPlayer.SetMediaPlayer(MPViewModel.Player);

            MPViewModel.MediaPlayerRecreated += MPViewModel_MediaPlayerRecreated;

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

            Unloaded += NowPlayingPage_Unloaded;
        }

        private async void MPViewModel_MediaPlayerRecreated(object sender, Windows.Media.Playback.MediaPlayer e)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => MainPlayer.SetMediaPlayer(MPViewModel.Player));
        }

        private void PlayerControls_ShufflingChanged(object sender, bool e)
            => MPViewModel.ShuffleEnabled = e;

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

        private void NowPlayingPage_Unloaded(object sender, RoutedEventArgs e)
        {
            PointerEntered -= Page_PointerEntered;
            PointerExited -= Page_PointerExited;

            MPViewModel.MediaPlayerRecreated -= MPViewModel_MediaPlayerRecreated;
        }
    }
}
