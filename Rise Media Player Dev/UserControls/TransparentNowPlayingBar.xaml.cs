using Microsoft.Toolkit.Uwp.UI;
using Rise.App.Converters;
using Rise.App.ViewModels;
using Rise.App.Views;
using System;
using System.Linq;
using Windows.Foundation;
using Windows.Media.Casting;
using Windows.Media.Playback;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

namespace Rise.App.UserControls
{
    public sealed partial class TransparentNowPlayingBar : UserControl
    {
        #region Variables
        private MediaPlayer _player = App.PViewModel.Player;

        private AlbumViewModel CurrentSongAlbum;
        private CastingDevicePicker castingPicker;
        #endregion

        private MainViewModel MViewModel => App.MViewModel;

        private SongViewModel SelectedSong
        {
            get => MViewModel.SelectedSong;
            set => MViewModel.SelectedSong = value;
        }

        public TransparentNowPlayingBar()
        {
            InitializeComponent();
            Overlay1.Glyph = "\uE10C";
            Overlay2.Glyph = "\uE10C";
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
        private void OverlayButton_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            OverlayFlyout.ShowAt(OverlayButton, e.GetPosition(OverlayButton));
        }

        private void UnpinOverlay_Click(object sender, RoutedEventArgs e)
        {
            OverlayMenu.Visibility = Visibility.Visible;
            OverlayButton.Visibility = Visibility.Collapsed;
        }

        private void MiniMenu_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            MiniMenuFlyout.ShowAt(OverlayButton, e.GetPosition(OverlayButton));
        }

        private void MiniMenuFlyout_Click(object sender, RoutedEventArgs e)
        {
            OverlayMenu.Visibility = Visibility.Collapsed;
            OverlayButton.Visibility = Visibility.Visible;
        }

        private void PinMiniPlayer_Click(object sender, RoutedEventArgs e)
        {

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

        private void ShuffleButton_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)ShuffleButton.IsChecked)
            {
                App.PViewModel.PlaybackList.ShuffleEnabled = true;
            }
            else
            {
                App.PViewModel.PlaybackList.ShuffleEnabled = false;
            }
        }

        private void RepeatButton_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)RepeatButton.IsChecked)
            {
                App.PViewModel.PlaybackList.AutoRepeatEnabled = true;
            }
            else
            {
                App.PViewModel.PlaybackList.AutoRepeatEnabled = false;
            }
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
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                int seconds = (int)sender.NaturalDuration.TotalSeconds;
                int minutes = (int)sender.NaturalDuration.TotalMinutes;
                SliderProgress.Maximum = sender.NaturalDuration.TotalSeconds;
                MediaPlayingDurationRight.Text = TimeSpanToString.Convert(TimeSpan.FromSeconds(seconds));
            });

            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
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
                fontIcon.Glyph = "\uE10C";
            }
            else
            {

            }
        }

        private async void OverlayButton1_Click(object sender, RoutedEventArgs e)
        {
            FontIcon fontIcon = OverlayButton1.FindChildren().First() as FontIcon;
            if (ApplicationView.GetForCurrentView().ViewMode != ApplicationViewMode.CompactOverlay)
            {
                var preferences = ViewModePreferences.CreateDefault(ApplicationViewMode.CompactOverlay);
                preferences.CustomSize = new Size(400, 400);
                _ = await ApplicationView.GetForCurrentView().TryEnterViewModeAsync(ApplicationViewMode.CompactOverlay, preferences);
                fontIcon.Glyph = "\uE10C";
            }
            else
            {

            }
        }

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.NewSize.Width >= 900)
            {
                OverlayMediaButtons.HorizontalAlignment = HorizontalAlignment.Left;
                DefaultVolumeControl.Visibility = Visibility.Visible;
                VolumeFlyoutButton.Visibility = Visibility.Collapsed;
                Grid.ColumnDefinitions[1].Width = new GridLength(0.5, GridUnitType.Star);
                VolumeFlyoutButton1.Visibility = Visibility.Collapsed;
                OverlayButton1.Visibility = Visibility.Collapsed;
                Back10.Visibility = Visibility.Visible;
                Forward30.Visibility = Visibility.Visible;
            }
            else if (e.NewSize.Width >= 600)
            {
                OverlayMediaButtons.HorizontalAlignment = HorizontalAlignment.Left;
                DefaultVolumeControl.Visibility = Visibility.Visible;
                VolumeFlyoutButton.Visibility = Visibility.Collapsed;
                Grid.ColumnDefinitions[1].Width = new GridLength(0.5, GridUnitType.Star);
                VolumeFlyoutButton1.Visibility = Visibility.Collapsed;
                OverlayButton1.Visibility = Visibility.Collapsed;
                Back10.Visibility = Visibility.Visible;
                Forward30.Visibility = Visibility.Visible;
            }
            else if (e.NewSize.Width >= 480)
            {
                OverlayMediaButtons.HorizontalAlignment = HorizontalAlignment.Left;
                DefaultVolumeControl.Visibility = Visibility.Visible;
                VolumeFlyoutButton.Visibility = Visibility.Collapsed;
                Grid.ColumnDefinitions[1].Width = new GridLength(0.5, GridUnitType.Star);
                VolumeFlyoutButton1.Visibility = Visibility.Collapsed;
                OverlayButton1.Visibility = Visibility.Collapsed;
                Back10.Visibility = Visibility.Visible;
                Forward30.Visibility = Visibility.Visible;
            }
            else if (e.NewSize.Width >= 400)
            {
                OverlayMediaButtons.HorizontalAlignment = HorizontalAlignment.Left;
                DefaultVolumeControl.Visibility = Visibility.Visible;
                VolumeFlyoutButton.Visibility = Visibility.Collapsed;
                Grid.ColumnDefinitions[1].Width = new GridLength(0.5, GridUnitType.Star);
                VolumeFlyoutButton1.Visibility = Visibility.Collapsed;
                OverlayButton1.Visibility = Visibility.Collapsed;
                Back10.Visibility = Visibility.Collapsed;
                Forward30.Visibility = Visibility.Collapsed;
            }
            else
            {
                OverlayMediaButtons.HorizontalAlignment = HorizontalAlignment.Center;
                DefaultVolumeControl.Visibility = Visibility.Collapsed;
                VolumeFlyoutButton.Visibility = Visibility.Visible;
                Grid.ColumnDefinitions[1].Width = new GridLength(0, GridUnitType.Star);
                VolumeFlyoutButton1.Visibility = Visibility.Visible;
                Back10.Visibility = Visibility.Collapsed;
                Forward30.Visibility = Visibility.Collapsed;

                OverlayButton1.Visibility = Visibility.Visible;
                FontIcon fontIcon = OverlayButton1.FindChildren().First() as FontIcon;
                if (ApplicationView.GetForCurrentView().ViewMode == ApplicationViewMode.CompactOverlay)
                {
                    OverlayButton.Visibility = Visibility.Collapsed;
                }
                else fontIcon.Glyph = "\uEE49";
            }

            FontIcon fontIcon1 = OverlayButton.FindChildren().First() as FontIcon;
            if (ApplicationView.GetForCurrentView().ViewMode == ApplicationViewMode.CompactOverlay)
            {
                OverlayButton.Visibility = Visibility.Collapsed;
            }
            else fontIcon1.Glyph = "\uEE49";
        }

        private void PlayButton_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            Button button = sender as Button;
            Border border = button.Parent as Border;
            if (Application.Current.RequestedTheme == ApplicationTheme.Dark)
            {
                border.BorderBrush = PlayBorderBrushDark;
            }
            else
            {
                border.BorderBrush = PlayBorderBrushLight;
            }

        }

        private void PlayButton_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            Button button = sender as Button;
            Border border = button.Parent as Border;
            border.BorderBrush = new SolidColorBrush();
        }

        private void PreviousButton_Click(object sender, RoutedEventArgs e)
        {
            App.PViewModel.PlaybackList.MovePrevious();
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            App.PViewModel.PlaybackList.MoveNext();
        }

        private void AlbumArtContainer_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            _ = MainPage.Current.ContentFrame.Navigate(typeof(AlbumSongsPage), CurrentSongAlbum);
        }

        private void RestoreVideoButton_Click(object sender, RoutedEventArgs e)
        {
            if (Window.Current.Content is Frame rootFrame)
            {
                _ = rootFrame.Navigate(typeof(VideoPlaybackPage));
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

        #endregion

        private void TogglePlayPause()
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

        public enum NowPlayingBarBackgroundStyles
        {
            Transparent,
            Acrylic,
            UseAlbumArt
        }

        private void FullScreen_Click(object sender, RoutedEventArgs e)
        {
            var view = ApplicationView.GetForCurrentView();
            if (view.IsFullScreenMode)
            {
                view.ExitFullScreenMode();
                FullScreenButton.Text = "Full screen";
                FullScreenIcon.Glyph = "\uE740";
                // The SizeChanged event will be raised when the exit from full-screen mode is complete.
            }
            else
            {
                if (view.TryEnterFullScreenMode())
                {
                    if (Window.Current.Content is Frame rootFrame)
                    {
                        _ = rootFrame.Navigate(typeof(FullNowPlayingPage));
                    }
                    FullScreenButton.Text = "Exit full screen";
                    FullScreenIcon.Glyph = "\uE73F";
                    // The SizeChanged event will be raised when the entry to full-screen mode is complete.
                }
            }
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (_player.PlaybackSession.PlaybackState == MediaPlaybackState.Playing)
            {
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    PlayButtonIcon.Glyph = "\uE62E";
                    ToolTipService.SetToolTip(PlayButton, "Pause");
                });
            }
            else if (_player.PlaybackSession.PlaybackState == MediaPlaybackState.Paused)
            {
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    PlayButtonIcon.Glyph = "\uF5B0";
                    ToolTipService.SetToolTip(PlayButton, "Play");
                });
            }
            else if (_player.PlaybackSession.PlaybackState == MediaPlaybackState.Buffering)
            {
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    ToolTipService.SetToolTip(PlayButton, "Buffering...");
                });
            }
        }

        private void Forward30_Click(object sender, RoutedEventArgs e)
        {
            _player.PlaybackSession.Position = TimeSpan.FromSeconds(((int)_player.PlaybackSession.Position.TotalSeconds) + 30);
        }

        private void Back10_Click(object sender, RoutedEventArgs e)
        {
            _player.PlaybackSession.Position = TimeSpan.FromSeconds(((int)_player.PlaybackSession.Position.TotalSeconds) - 10);
        }

        private async void Props_Click(object sender, RoutedEventArgs e)
        {
            if (!App.PViewModel.CurrentSong.IsOnline)
            {
                SelectedSong = App.PViewModel.CurrentSong;
                await App.PViewModel.CurrentSong.StartEditAsync();
            }
        }
    }
}