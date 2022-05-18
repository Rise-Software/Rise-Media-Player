using System;
using System.ComponentModel;
using Rise.App.ViewModels;
using Rise.Data.ViewModels;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace Rise.App.Views
{
    public sealed partial class NowPlayingPage : Page
    {
        private MediaPlaybackViewModel MPViewModel => App.MPViewModel;
        private SettingsViewModel SViewModel => App.SViewModel;

        private bool _isHovered;

        public NowPlayingPage()
        {
            InitializeComponent();

            MainPlayer.SetMediaPlayer(MPViewModel.Player);
            _ = ApplyVisualizer(SViewModel.VisualizerType);

            MPViewModel.MediaPlayerRecreated += MPViewModel_MediaPlayerRecreated;
            SViewModel.PropertyChanged += OnSettingChanged;
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
            SViewModel.PropertyChanged -= OnSettingChanged;
        }

        private async void ExitOverlayButton_Click(object sender, RoutedEventArgs e)
        {
            _ = await ApplicationView.GetForCurrentView().TryEnterViewModeAsync(ApplicationViewMode.Default, ViewModePreferences.CreateDefault(ApplicationViewMode.Default));

            if ((Window.Current.Content as Frame).CanGoBack)
                (Window.Current.Content as Frame).GoBack();
        }

        private void OnSettingChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SViewModel.VisualizerType))
                _ = ApplyVisualizer(SViewModel.VisualizerType);
        }

        private bool ApplyVisualizer(int index) => index switch
        {
            1 => VisualStateManager.GoToState(this, "LineVisualizerState", false),
            2 => VisualStateManager.GoToState(this, "BloomVisualizerState", false),
            _ => VisualStateManager.GoToState(this, "NoVisualizerState", false),
        };
    }
}
