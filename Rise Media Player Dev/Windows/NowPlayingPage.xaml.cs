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
    /// <summary>
    /// A page that shows the current state of playback.
    /// </summary>
    public sealed partial class NowPlayingPage : Page
    {
        private MediaPlaybackViewModel MPViewModel => App.MPViewModel;
        private SettingsViewModel SViewModel => App.SViewModel;

        /// <summary>
        /// Whether the Now Playing window is large.
        /// </summary>
        private bool IsWindowLarge;

        /// <summary>
        /// Whether the album art should be fully visible.
        /// </summary>
        private bool UseImmersiveArt
        {
            get => (bool)GetValue(UseImmersiveArtProperty);
            set => SetValue(UseImmersiveArtProperty, value);
        }

        public NowPlayingPage()
        {
            InitializeComponent();

            Loaded += OnPageLoaded;
            Unloaded += OnPageUnloaded;
        }

        private void OnPageLoaded(object sender, RoutedEventArgs e)
        {
            if (MPViewModel.PlayerCreated)
                MainPlayer.SetMediaPlayer(MPViewModel.Player);
            else
                MPViewModel.MediaPlayerRecreated += OnMediaPlayerRecreated;

            Debug.Assert(ApplyVisualizer(SViewModel.VisualizerType));
            Debug.Assert(ApplyMode(SViewModel.NowPlayingMode));

            // No need for pointer in events when we're in the main window
            if (SViewModel.NowPlayingMode == 1)
            {
                UseImmersiveArt = false;
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

            SViewModel.PropertyChanged += OnSettingChanged;
        }

        private void OnPageUnloaded(object sender, RoutedEventArgs e)
        {
            PointerEntered -= OnPointerEntered;
            PointerExited -= OnPointerExited;
            PointerCanceled -= OnPointerExited;

            MPViewModel.MediaPlayerRecreated -= OnMediaPlayerRecreated;
            SViewModel.PropertyChanged -= OnSettingChanged;
        }
    }

    // Event handlers
    public sealed partial class NowPlayingPage
    {
        // Buttons
        private void OnBackButtonClick(object sender, RoutedEventArgs e)
        {
            if (Frame.CanGoBack) Frame.GoBack();
        }

        private async void OnExitOverlayClick(object sender, RoutedEventArgs e)
        {
            _ = await ApplicationView.GetForCurrentView().
                TryEnterViewModeAsync(ApplicationViewMode.Default, ViewModePreferences.CreateDefault(ApplicationViewMode.Default));
            if (Frame.CanGoBack) Frame.GoBack();
        }

        // Media playback
        private async void OnMediaPlayerRecreated(object sender, Windows.Media.Playback.MediaPlayer e)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => MainPlayer.SetMediaPlayer(e));
        }

        private void OnShufflingChanged(object sender, bool e)
            => MPViewModel.ShuffleEnabled = e;

        // UI
        private void OnPointerEntered(object sender, PointerRoutedEventArgs e)
            => UpdatePointerStates(true);

        private void OnPointerExited(object sender, PointerRoutedEventArgs e)
            => UpdatePointerStates(false);

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            IsWindowLarge = e.NewSize.Width >= 720 && e.NewSize.Height >= 600;
            UpdatePointerStates(IsWindowLarge);
        }

        private void UpdatePointerStates(bool pointerIn)
        {
            string state = "NoVisualizerState";
            if (pointerIn)
            {
                UseImmersiveArt = false;
            }
            else
            {
                // If the window is large, we don't want the immersive art
                if (IsWindowLarge)
                    state = "LineVisualizerState";
                UseImmersiveArt = !IsWindowLarge;
            }

            if (SViewModel.VisualizerType == 1)
            {
                VisualStateManager.GoToState(this, state, true);
            }
        }

        // Settings
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

    // Dependency properties
    public sealed partial class NowPlayingPage
    {
        private readonly static DependencyProperty UseImmersiveArtProperty =
            DependencyProperty.Register(nameof(UseImmersiveArt), typeof(bool),
                typeof(NowPlayingPage), new PropertyMetadata(true));
    }
}
