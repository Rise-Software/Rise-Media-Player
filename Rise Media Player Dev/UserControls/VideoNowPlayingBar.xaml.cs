using System;
using System.Linq;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.Media.Playback;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Microsoft.Toolkit.Uwp.UI;
using Rise.App.Converters;

namespace Rise.App.UserControls
{
    public sealed partial class VideoNowPlayingBar : UserControl
    {
        #region Variables
        private MediaPlayer _player = App.PViewModel.Player;

        private ViewModels.SongViewModel CurrentSong = App.PViewModel.CurrentSong;

        private string PlayButtonText;
        #endregion

        public VideoNowPlayingBar()
        {
            this.InitializeComponent();

            DataContext = App.PViewModel;
            Loaded += NowPlayingBar_Loaded;
        }

        #region Listeners

        private void SliderProgress_PointerCaptureLost(object sender, PointerRoutedEventArgs e)
        {
            _player.PlaybackSession.Position = TimeSpan.FromSeconds(SliderProgress.Value);
        }

        private void NowPlayingBar_Loaded(object sender, RoutedEventArgs e)
        {
            _player.PlaybackSession.PositionChanged += PlaybackSession_PositionChanged;
            _player.PlaybackSession.PlaybackStateChanged += PlaybackSession_PlaybackStateChanged;
        }

        private async void PlaybackSession_PlaybackStateChanged(MediaPlaybackSession sender, object args)
        {
            if (sender.PlaybackState == MediaPlaybackState.Playing)
            {
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    PlayButtonIcon.Glyph = "\uE62E";
                    ToolTipService.SetToolTip(PlayButton, "Pause");
                });
            }
            else if (sender.PlaybackState == MediaPlaybackState.Paused)
            {
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    PlayButtonIcon.Glyph = "\uF5B0";
                    ToolTipService.SetToolTip(PlayButton, "Play");
                });
            }
            else if (sender.PlaybackState == MediaPlaybackState.Buffering)
            {
                ToolTipService.SetToolTip(PlayButton, "Buffering...");
            }
        }

        private async void PlaybackSession_PositionChanged(MediaPlaybackSession sender, object args)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                int seconds = (int)sender.NaturalDuration.TotalSeconds;
                int minutes = (int)sender.NaturalDuration.TotalMinutes;
                SliderProgress.Maximum = sender.NaturalDuration.TotalSeconds;
                MediaPlayingDurationRight.Text = TimeSpanToString.Convert(TimeSpan.FromSeconds(seconds));
            });

            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                SliderProgress.Value = sender.Position.TotalSeconds;
                int seconds = (int)sender.Position.TotalSeconds;
                int minutes = (int)sender.Position.TotalMinutes;
                MediaPlayingDurationLeft.Text = TimeSpanToString.Convert(TimeSpan.FromSeconds(seconds));
            });
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            TogglePlayPause();
        }

        private async void OverlayButton_Click(object sender, RoutedEventArgs e)
        {
            FontIcon fontIcon = OverlayButton.FindChildren().First() as FontIcon;
            if (ApplicationView.GetForCurrentView().ViewMode != ApplicationViewMode.CompactOverlay)
            {
                var preferences = ViewModePreferences.CreateDefault(ApplicationViewMode.CompactOverlay);
                preferences.CustomSize = new Size(400, 400);
                _ = await ApplicationView.GetForCurrentView().TryEnterViewModeAsync(ApplicationViewMode.CompactOverlay, preferences);
                fontIcon.Glyph = "\uEE47";
            }
            else
            {
                var preferences = ViewModePreferences.CreateDefault(ApplicationViewMode.CompactOverlay);
                preferences.CustomSize = new Size(600, 700);
                _ = await ApplicationView.GetForCurrentView().TryEnterViewModeAsync(ApplicationViewMode.Default, preferences);
                fontIcon.Glyph = "\uEE49";
            }
        }
        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            FontIcon fontIcon = OverlayButton.FindChildren().First() as FontIcon;
            if (ApplicationView.GetForCurrentView().ViewMode == ApplicationViewMode.CompactOverlay)
            {
                fontIcon.Glyph = "\uEE47";
            }
            else fontIcon.Glyph = "\uEE49";
        }

        private void Back10_Click(object sender, RoutedEventArgs e)
        {
            _player.PlaybackSession.Position = TimeSpan.FromSeconds(((int)_player.PlaybackSession.Position.TotalSeconds) - 10);
        }

        private void Forward30_Click(object sender, RoutedEventArgs e)
        {
            _player.PlaybackSession.Position = TimeSpan.FromSeconds(((int)_player.PlaybackSession.Position.TotalSeconds) + 30);
        }

        #endregion

        public void TogglePlayPause()
        {
            if (_player.PlaybackSession.PlaybackState == MediaPlaybackState.Paused)
            {
                _player.Play();
                PlayButtonIcon.Glyph = "\uF8AE";
            }
            else if (_player.PlaybackSession.PlaybackState == MediaPlaybackState.Playing)
            {
                _player.Pause();
                PlayButtonIcon.Glyph = "\uF5B0";
            }
        }

        private void VideoFullScreen_Click(object sender, RoutedEventArgs e)
        {
            var view = ApplicationView.GetForCurrentView();
            if (view.IsFullScreenMode)
            {
                view.ExitFullScreenMode();
                ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.Auto;
                FullScreenButton.Text = "Full screen";
                FullScreenIcon.Glyph = "\uE740";
                // The SizeChanged event will be raised when the exi`1t from full-screen mode is complete.
            }
            else
            {
                if (view.TryEnterFullScreenMode())
                {
                    ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.FullScreen;
                    FullScreenButton.Text = "Exit full screen";
                    FullScreenIcon.Glyph = "\uE73F";
                    // The SizeChanged event will be raised when the entry to full-screen mode is complete.
                }
            }
        }
    }
}