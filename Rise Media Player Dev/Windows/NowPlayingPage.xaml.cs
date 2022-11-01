using Rise.App.Helpers;
using Rise.App.ViewModels;
using Rise.Common.Extensions;
using Rise.Data.ViewModels;
using Rise.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Media.Playback;
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

        private List<SyncedLyricItem> _lyrics;

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

        private void OnPageLoaded(object sender, RoutedEventArgs e)
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
        }

        private void OnPageUnloaded(object sender, RoutedEventArgs e)
        {
            MPViewModel.Player.SeekCompleted -= Player_SeekCompleted;
            MPViewModel.Player.PlaybackSession.PositionChanged -= PlaybackSession_PositionChanged;
            MPViewModel.PlayingItemChanged -= MPViewModel_PlayingItemChanged;

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
        }
    }

    // Event handlers
    public sealed partial class NowPlayingPage
    {
        private async void OnLyricsListLoaded(object sender, RoutedEventArgs e)
        {
            if (SViewModel.FetchOnlineData)
            {
                await FetchLyricsForCurrentItemAsync();

                MPViewModel.Player.SeekCompleted += Player_SeekCompleted;
                MPViewModel.Player.PlaybackSession.PositionChanged += PlaybackSession_PositionChanged;
                MPViewModel.PlayingItemChanged += MPViewModel_PlayingItemChanged;
            }
        }

        private async void OnExitButtonClick(object sender, RoutedEventArgs e)
        {
            if (Frame.CanGoBack) Frame.GoBack();

            var curr = ApplicationView.GetForCurrentView();
            if (curr.ViewMode == ApplicationViewMode.CompactOverlay)
                _ = await curr.TryEnterViewModeAsync(ApplicationViewMode.Default,
                    ViewModePreferences.CreateDefault(ApplicationViewMode.Default));
        }

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

        private bool ApplyVisualizer(int index) => index switch
        {
            1 => VisualStateManager.GoToState(this, "BloomVisualizerState", false),
            _ => VisualStateManager.GoToState(this, "NoVisualizerState", false),
        };
    }

    // Lyrics
    public sealed partial class NowPlayingPage
    {
        private async void MPViewModel_PlayingItemChanged(object sender, MediaPlaybackItem e)
            => await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () => await FetchLyricsForCurrentItemAsync());

        private async void Player_SeekCompleted(MediaPlayer sender, object args)
            => await UpdateCurrentLyricAsync(sender.PlaybackSession.Position);

        private async void PlaybackSession_PositionChanged(MediaPlaybackSession sender, object args)
            => await UpdateCurrentLyricAsync(sender.Position);

        private IAsyncAction UpdateCurrentLyricAsync(TimeSpan playerPosition)
        {
            var lyricsItem = _lyrics?.FirstOrDefault(item => item.TimeSpan.TotalSeconds - playerPosition.TotalSeconds >= 0);

            // The dispatcher call starts here due to wrong thread exceptions when
            // trying to access the lyric list's SelectedItem normally
            return Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                if (lyricsItem != null && lyricsItem != LyricsList.SelectedItem)
                {
                    LyricsList.SelectedItem = lyricsItem;
                    LyricsList.ScrollIntoView(lyricsItem);
                }
            });
        }

        private async Task FetchLyricsForCurrentItemAsync()
        {
            LyricsList.ItemsSource = null;
            try
            {
                var lyricsObj = await MusixmatchHelper.GetSyncedLyricsAsync(MPViewModel.PlayingItemProperties.Title, MPViewModel.PlayingItemProperties.Artist);
                var body = lyricsObj.Message.Body;

                if (body != null)
                {
                    _lyrics = await Task.Run(() => new List<SyncedLyricItem>(body.Subtitle.Subtitles.Where(i => !string.IsNullOrWhiteSpace(i.Text))));
                    LyricsList.ItemsSource = _lyrics;
                }
            }
            catch (Exception e)
            {
                e.WriteToOutput();
            }
        }
    }

    // Dependency properties
    public sealed partial class NowPlayingPage
    {
        private readonly static DependencyProperty UseImmersiveArtProperty =
            DependencyProperty.Register(nameof(UseImmersiveArt), typeof(bool),
                typeof(NowPlayingPage), new PropertyMetadata(true));
    }
}
