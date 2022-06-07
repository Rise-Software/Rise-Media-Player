using AudioVisualizer;
using Rise.App.ViewModels;
using Rise.Data.ViewModels;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Media.Playback;
using Windows.UI.Core;
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
        /// Whether the album art should be fully visible.
        /// </summary>
        private bool UseImmersiveArt
        {
            get => (bool)GetValue(UseImmersiveArtProperty);
            set => SetValue(UseImmersiveArtProperty, value);
        }

        private PlaybackSource PlaybackSource = null;
        public IVisualizationSource VisualizerSource
        {
            get => (IVisualizationSource)GetValue(VisualizerSourceProperty);
            set => SetValue(VisualizerSourceProperty, value);
        }

        public NowPlayingPage()
        {
            InitializeComponent();

            Loaded += OnPageLoaded;
            Unloaded += OnPageUnloaded;
        }

        private async void OnPageLoaded(object sender, RoutedEventArgs e)
        {
            // No need for pointer in events when we're outside compact overlay
            var mode = ApplicationView.GetForCurrentView().ViewMode;
            if (mode == ApplicationViewMode.Default)
            {
                UseImmersiveArt = false;
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    UpdatePointerStates(true);
                });
            }
            else
            {
                Debug.Assert(VisualStateManager.GoToState(this, "CompactOverlayState", true));

                PointerEntered += OnPointerEntered;
                PointerExited += OnPointerExited;
                PointerCanceled += OnPointerExited;
            }

            if (MPViewModel.PlayerCreated)
                await UpdateSourcesAsync(MPViewModel.Player);
            else
                MPViewModel.MediaPlayerRecreated += OnMediaPlayerRecreated;
        }

        private void OnPageUnloaded(object sender, RoutedEventArgs e)
        {
            PointerEntered -= OnPointerEntered;
            PointerExited -= OnPointerExited;
            PointerCanceled -= OnPointerExited;

            MPViewModel.MediaPlayerRecreated -= OnMediaPlayerRecreated;

            if (PlaybackSource != null)
                PlaybackSource.SourceChanged -= OnPlaybackSourceChanged;

            if (MPViewModel.PlayerCreated)
                MPViewModel.Player.RemoveAllEffects();

            Bindings.StopTracking();
        }
    }

    // Event handlers
    public sealed partial class NowPlayingPage
    {
        // Buttons
        private async void OnExitButtonClick(object sender, RoutedEventArgs e)
        {
            if (Frame.CanGoBack) Frame.GoBack();

            var curr = ApplicationView.GetForCurrentView();
            if (curr.ViewMode == ApplicationViewMode.CompactOverlay)
                _ = await curr.TryEnterViewModeAsync(ApplicationViewMode.Default,
                    ViewModePreferences.CreateDefault(ApplicationViewMode.Default));
        }

        // Media playback
        private async void OnMediaPlayerRecreated(object sender, MediaPlayer e)
            => await UpdateSourcesAsync(e);

        private async Task UpdateSourcesAsync(MediaPlayer player)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                MainPlayer.SetMediaPlayer(player);

                // Source is expensive, only create when necessary
                if (SViewModel.VisualizerType == 1)
                {
                    PlaybackSource = PlaybackSource.CreateFromMediaPlayer(player);
                    VisualizerSource = PlaybackSource.Source;
                    PlaybackSource.SourceChanged += OnPlaybackSourceChanged;
                }
            });
        }

        private async void OnPlaybackSourceChanged(object sender, IVisualizationSource args)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => VisualizerSource = args);
        }

        private void OnShufflingChanged(object sender, bool e)
            => MPViewModel.ShuffleEnabled = e;

        // UI
        private void OnPointerEntered(object sender, PointerRoutedEventArgs e)
            => UpdatePointerStates(true);

        private void OnPointerExited(object sender, PointerRoutedEventArgs e)
            => UpdatePointerStates(false);

        private void UpdatePointerStates(bool pointerIn)
        {
            UseImmersiveArt = !pointerIn;

            if (SViewModel.VisualizerType != 0)
                if (pointerIn)
                    _ = ApplyVisualizer(SViewModel.VisualizerType);
                else
                    _ = ApplyVisualizer(0);
        }

        // Settings
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

        private readonly static DependencyProperty VisualizerSourceProperty =
            DependencyProperty.Register(nameof(VisualizerSource), typeof(IVisualizationSource),
                typeof(NowPlayingPage), new PropertyMetadata(null));
    }
}
