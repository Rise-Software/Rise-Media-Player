using Microsoft.Toolkit.Uwp.UI;
using Rise.App.Converters;
using Rise.App.Views;
using System;
using System.Linq;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Media.Casting;
using Windows.Media.Playback;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

namespace Rise.App.UserControls
{
    public sealed partial class VideoNowPlayingBar : UserControl
    {
        #region Variables
        private MediaPlayer _player = App.PViewModel.Player;

        private ViewModels.SongViewModel CurrentSong = App.PViewModel.CurrentSong;
        private CastingDevicePicker castingPicker;

        private string PlayButtonText;
        #endregion

        public VideoNowPlayingBar()
        {
            InitializeComponent();
            _player.PlaybackSession.PlaybackRate = 1;

            DataContext = App.PViewModel;
            Loaded += NowPlayingBar_Loaded;
            castingPicker = new CastingDevicePicker();
            castingPicker.Filter.SupportsVideo = true;
            castingPicker.CastingDeviceSelected += CastingPicker_CastingDeviceSelected;
        }

        #region Events

        private void CastToDevice_Click(object sender, RoutedEventArgs e)
        {
            //Retrieve the location of the casting button
            GeneralTransform transform = CastButton.TransformToVisual(Window.Current.Content);
            Point pt = transform.TransformPoint(new Point(0, 0));

            //Show the picker above our casting button
            castingPicker.Show(new Rect(pt.X - 30, pt.Y - 100, CastButton.ActualWidth, CastButton.ActualHeight),
                Windows.UI.Popups.Placement.Above);
        }

        private async void CastingPicker_CastingDeviceSelected(CastingDevicePicker sender, CastingDeviceSelectedEventArgs args)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                // Create a casting conneciton from our selected casting device
                CastingConnection connection = args.SelectedCastingDevice.CreateCastingConnection();

                // Cast the content loaded in the media element to the selected casting device
                await connection.RequestStartCastingAsync(_player.GetAsCastingSource());
            });
        }

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
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    PlayButtonIcon.Glyph = "\uE62E";
                    ToolTipService.SetToolTip(PlayButton, "Pause");
                });
            }
            else if (sender.PlaybackState == MediaPlaybackState.Paused)
            {
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    PlayButtonIcon.Glyph = "\uF5B0";
                    ToolTipService.SetToolTip(PlayButton, "Play");
                });
            }
            else if (sender.PlaybackState == MediaPlaybackState.Buffering)
            {
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    ToolTipService.SetToolTip(PlayButton, "Buffering...");
                });
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

        private void RepeatButton_Click(object sender, RoutedEventArgs e)
        {
            ToggleButton toggleButton = sender as ToggleButton;
            if ((bool)toggleButton.IsChecked)
            {
                App.PViewModel.PlaybackList.AutoRepeatEnabled = true;
            }
            else
            {
                App.PViewModel.PlaybackList.AutoRepeatEnabled = false;
            }
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

        private void RadioMenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            switch ((sender as MenuFlyoutItem).Text)
            {
                case "0.5x":
                    _player.PlaybackSession.PlaybackRate = 0.5;
                    break;
                case "0.75":
                    _player.PlaybackSession.PlaybackRate = 0.75;
                    break;
                case "1x (Normal)":
                    _player.PlaybackSession.PlaybackRate = 1;
                    break;
                case "1.5x":
                    _player.PlaybackSession.PlaybackRate = 1.5;
                    break;
                case "2x":
                    _player.PlaybackSession.PlaybackRate = 2;
                    break;
                case "2.5x":
                    _player.PlaybackSession.PlaybackRate = 2.5;
                    break;
            }
        }

        private void FullScreen_Click(object sender, RoutedEventArgs e)
        {
            var view = ApplicationView.GetForCurrentView();
            if (view.IsFullScreenMode)
            {
                view.ExitFullScreenMode();
                FullScreenButton.Text = "Full screen";
                FullScreenIcon.Glyph = "\uE740";
                // The SizeChanged event will be raised when the exi`1t from full-screen mode is complete.
            }
            else
            {
                if (view.TryEnterFullScreenMode())
                {
                    FullScreenButton.Text = "Exit full screen";
                    FullScreenIcon.Glyph = "\uE73F";
                    // The SizeChanged event will be raised when the entry to full-screen mode is complete.
                }
            }
        }

        private void ZoomToFill_Click(object sender, RoutedEventArgs e)
        {
            if (sender is ToggleMenuFlyoutItem item)
            {
                if (item.IsChecked)
                {
                    VideoPlaybackPage.Current.PlayerElement.Stretch = Stretch.UniformToFill;
                }
                else
                {
                    VideoPlaybackPage.Current.PlayerElement.Stretch = Stretch.Uniform;
                }
            }
        }
    }
}