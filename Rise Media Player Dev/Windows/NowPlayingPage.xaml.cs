using Rise.App.Helpers;
using Rise.App.ViewModels;
using Rise.Common.Extensions;
using Rise.Data.ViewModels;
using Rise.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

namespace Rise.App.Views
{
    /// <summary>
    /// A page that shows the current state of playback.
    /// </summary>
    public sealed partial class NowPlayingPage : Page
    {
        private MediaPlaybackViewModel MPViewModel => App.MPViewModel;
        private SettingsViewModel SViewModel => App.SViewModel;
        private bool FullScreenRequested = false;

        private ObservableCollection<SyncedLyricItem> _lyrics;
        private DispatcherTimer _timer;

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
            TitleBar.SetTitleBarForCurrentView();
        }

        private async void OnPageLoaded(object sender, RoutedEventArgs e)
        {
            // No need for pointer in events when we're outside compact overlay
            var mode = ApplicationView.GetForCurrentView().ViewMode;
            if (mode == ApplicationViewMode.Default)
            {
                UpdatePointerStates(true);
            }
            else
            {
                VisualStateManager.GoToState(this, "CompactOverlayState", true);

                PointerEntered += OnPointerEntered;
                PointerExited += OnPointerExited;
                PointerCanceled += OnPointerExited;
            }

            MainPlayer.SetMediaPlayer(MPViewModel.Player);

            if (SViewModel.FetchOnlineData)
            {
                _timer = new()
                {
                    Interval = TimeSpan.FromMilliseconds(150)
                };

                await FetchLyricsForCurrentItemAsync();
                MPViewModel.PlayingItemChanged += MPViewModel_PlayingItemChanged;
            }
        }

        private async void MPViewModel_PlayingItemChanged(object sender, Windows.Media.Playback.MediaPlaybackItem e)
            => await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () => await FetchLyricsForCurrentItemAsync());

        private void OnPageUnloaded(object sender, RoutedEventArgs e)
        {
            PointerEntered -= OnPointerEntered;
            PointerExited -= OnPointerExited;
            PointerCanceled -= OnPointerExited;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is bool fs && fs)
                FullScreenRequested = fs;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            if (FullScreenRequested)
                ApplicationView.GetForCurrentView().ExitFullScreenMode();

            StopTimer();
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

        private void OnTimerTick(object sender, object e)
        {
            var mediaPlayerPosition = MPViewModel.Player.PlaybackSession.Position;

            var lyricsItem = _lyrics.OrderBy(item => Math.Abs(mediaPlayerPosition.TotalSeconds - item.TimeSpan.TotalSeconds)).FirstOrDefault();

            if (lyricsItem != null && mediaPlayerPosition.TotalSeconds - lyricsItem.TimeSpan.TotalSeconds >= 0)
            {
                LyricsList.SelectedItem = lyricsItem;
                LyricsList.ScrollIntoView(lyricsItem);
            }
        }

        private async Task FetchLyricsForCurrentItemAsync()
        {
            StopTimer();

            try
            {
                var lyricsObj = await MusixmatchHelper.GetSyncedLyricsAsync(MPViewModel.PlayingItemProperties.Title, MPViewModel.PlayingItemProperties.Artist);
                var body = lyricsObj.Message.Body;

                if (body != null)
                {
                    _lyrics = await Task.Run(() => new ObservableCollection<SyncedLyricItem>(body.Subtitle.Subtitles.Where(i => !string.IsNullOrWhiteSpace(i.Text))));
                    LyricsList.ItemsSource = _lyrics;

                    _timer.Start();

                    _timer.Tick += OnTimerTick;
                }
            }
            catch (Exception e)
            {
                e.WriteToOutput();
            }
        }

        private void StopTimer()
        {
            _timer?.Stop();
            _lyrics?.Clear();
        }

        // Settings
        private bool ApplyVisualizer(int index) => index switch
        {
            1 => VisualStateManager.GoToState(this, "BloomVisualizerState", false),
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
