using System;
using System.ComponentModel;
using System.Diagnostics;
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

            Debug.Assert(ApplyVisualizer(SViewModel.VisualizerType));
            Debug.Assert(ApplyMode(SViewModel.NowPlayingMode));

            // No need for pointer in events when we're in the main window
            if (SViewModel.NowPlayingMode == 1)
            {
                _isHovered = true;
                VisualStateManager.GoToState(this, "PointerInState", true);
            }
            else
            {
                PointerEntered += OnPointerEntered;
                PointerExited += OnPointerExited;
                PointerCanceled += OnPointerExited;
            }

            if (SViewModel.VisualizerType == 1 && SViewModel.NowPlayingMode != 1)
            {
                VisualStateManager.GoToState(this, "NoVisualizerState", true);
            }

            MPViewModel.MediaPlayerRecreated += OnMediaPlayerRecreated;
            SViewModel.PropertyChanged += OnSettingChanged;
            Unloaded += OnPageUnloaded;
        }

        private async void OnMediaPlayerRecreated(object sender, Windows.Media.Playback.MediaPlayer e)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => MainPlayer.SetMediaPlayer(MPViewModel.Player));
        }

        private void OnShufflingChanged(object sender, bool e)
            => MPViewModel.ShuffleEnabled = e;

        private void OnPointerEntered(object sender, PointerRoutedEventArgs e)
        {
            _isHovered = true;
            VisualStateManager.GoToState(this, "PointerInState", true);

            if (SViewModel.VisualizerType == 1)
            {
                VisualStateManager.GoToState(this, "LineVisualizerState", true);
            }
        }

        private void OnPointerExited(object sender, PointerRoutedEventArgs e)
        {
            _isHovered = false;
            VisualStateManager.GoToState(this, "PointerOutState", true);

            if (SViewModel.VisualizerType == 1)
            {
                VisualStateManager.GoToState(this, "NoVisualizerState", true);
            }
        }

        private void OnPageUnloaded(object sender, RoutedEventArgs e)
        {
            PointerEntered -= OnPointerEntered;
            PointerExited -= OnPointerExited;
            PointerCanceled -= OnPointerExited;

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
            switch (e.PropertyName)
            {
                case nameof(SViewModel.VisualizerType):
                    Debug.Assert(ApplyVisualizer(SViewModel.VisualizerType));
                    break;
            }
        }

        private bool ApplyMode(int index) => index switch
        {
            1 => VisualStateManager.GoToState(this, "MainWindowState", true),
            2 => VisualStateManager.GoToState(this, "CompactOverlayState", true),
            _ => VisualStateManager.GoToState(this, "SeparateWindowState", true),
        };

        private bool ApplyVisualizer(int index) => index switch
        {
            1 => VisualStateManager.GoToState(this, "LineVisualizerState", false),
            2 => VisualStateManager.GoToState(this, "BloomVisualizerState", false),
            _ => VisualStateManager.GoToState(this, "NoVisualizerState", false),
        };
    }
}
