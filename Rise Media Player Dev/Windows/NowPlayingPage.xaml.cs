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

            MPViewModel.MediaPlayerRecreated += OnMediaPlayerRecreated;
            SViewModel.PropertyChanged += OnSettingChanged;
            Unloaded += OnPageUnloaded;

            if (SViewModel.NowPlayingMode == 1)
            {
                _isHovered = true;
                VisualStateManager.GoToState(this, "PointerInState", true);
                OverlayExitButton.Visibility = Visibility.Collapsed;
                BackButton.Visibility = Visibility.Visible;
            } else if (SViewModel.NowPlayingMode == 2)
            {
                OverlayExitButton.Visibility = Visibility.Visible;
                BackButton.Visibility = Visibility.Collapsed;
            }
        }

        private async void OnMediaPlayerRecreated(object sender, Windows.Media.Playback.MediaPlayer e)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => MainPlayer.SetMediaPlayer(MPViewModel.Player));
        }

        private void OnShufflingChanged(object sender, bool e)
            => MPViewModel.ShuffleEnabled = e;

        private void OnPointerEntered(object sender, PointerRoutedEventArgs e)
        {
            if (SViewModel.NowPlayingMode != 1)
            {
                _isHovered = true;
                VisualStateManager.GoToState(this, "PointerInState", true);
            }
        }

        private void OnPointerExited(object sender, PointerRoutedEventArgs e)
        {
            if (SViewModel.NowPlayingMode != 1)
            {
                _isHovered = false;
                VisualStateManager.GoToState(this, "PointerOutState", true);
            }
        }

        private void OnPageUnloaded(object sender, RoutedEventArgs e)
        {
            PointerEntered -= OnPointerEntered;
            PointerExited -= OnPointerExited;

            MPViewModel.MediaPlayerRecreated -= OnMediaPlayerRecreated;
            SViewModel.PropertyChanged -= OnSettingChanged;
        }

        private async void OnExitOverlayClick(object sender, RoutedEventArgs e)
        {
            _ = await ApplicationView.GetForCurrentView().TryEnterViewModeAsync(ApplicationViewMode.Default, ViewModePreferences.CreateDefault(ApplicationViewMode.Default));

            if ((Window.Current.Content as Frame).CanGoBack)
                (Window.Current.Content as Frame).GoBack();
        }

        private void OnBackButtonClick(object sender, RoutedEventArgs e)
        {
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
