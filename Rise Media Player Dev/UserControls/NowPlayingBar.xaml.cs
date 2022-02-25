using ColorThiefDotNet;
using Microsoft.Toolkit.Uwp.UI;
using Rise.App.Converters;
using Rise.App.Helpers;
using Rise.App.ViewModels;
using Rise.App.Views;
using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Graphics.Imaging;
using Windows.Media.Casting;
using Windows.Media.Playback;
using Windows.Storage.Streams;
using Windows.System;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

namespace Rise.App.UserControls
{
    public sealed partial class NowPlayingBar : UserControl
    {

        private MainViewModel MViewModel => App.MViewModel;

        private SongViewModel SelectedSong
        {
            get => MViewModel.SelectedSong;
            set => MViewModel.SelectedSong = value;
        }

        #region Variables
        private readonly MediaPlayer _player = App.PViewModel.Player;
        private byte _tintOpacity = 100;
        private byte lightThemeAdditions = 85;

        private AlbumViewModel CurrentSongAlbum;
        private readonly CastingDevicePicker castingPicker;
        #endregion

        #region Properties

        public static readonly DependencyProperty ShowArtist = DependencyProperty.Register("IsArtistShown", typeof(bool), typeof(NowPlayingBar), new PropertyMetadata(null));

        public bool IsArtistShown
        {
            get => (bool)GetValue(ShowArtist);
            set => SetValue(ShowArtist, value);
        }

        public static readonly DependencyProperty BackgroundStylesProperty = DependencyProperty.Register("BackgroundStyle", typeof(NowPlayingBarBackgroundStyles), typeof(NowPlayingBar), new PropertyMetadata(null));

        private long _backgroundStylesPropertyToken;

        public NowPlayingBarBackgroundStyles BackgroundStyle
        {
            get => (NowPlayingBarBackgroundStyles)GetValue(BackgroundStylesProperty);
            set => SetValue(BackgroundStylesProperty, value);
        }

        public static readonly DependencyProperty IsInNowPlayingPageProperty = DependencyProperty.Register("IsInNowPlayingPage", typeof(bool), typeof(NowPlayingBar), new PropertyMetadata(null));

        private long _isInNowPlayingPageToken;

        public bool IsInNowPlayingPage
        {
            get => (bool)GetValue(IsInNowPlayingPageProperty);
            set => SetValue(IsInNowPlayingPageProperty, value);
        }

        public static readonly DependencyProperty OverlayBtnVisibilityProperty = DependencyProperty.Register("OverlayBtnVisibility", typeof(Visibility), typeof(NowPlayingBar), new PropertyMetadata(null));

        private long _overlayBtnVisibilityPropertyToken;

        public Visibility OverlayBtnVisibility
        {
            get => (Visibility)GetValue(OverlayBtnVisibilityProperty);
            set => SetValue(OverlayBtnVisibilityProperty, value);
        }

        #endregion

        public NowPlayingBar()
        {
            InitializeComponent();

            DataContext = App.PViewModel;
            Loaded += NowPlayingBar_Loaded;
            _player.PlaybackSession.PlaybackRate = 1;
            Set1.IsChecked = true;
            Unloaded += NowPlayingBar_Unloaded;
            UISettings uiSettings = new();
            uiSettings.ColorValuesChanged += UISettings_ColorValuesChanged;

            castingPicker = new CastingDevicePicker();
            castingPicker.Filter.SupportsVideo = true;
            castingPicker.CastingDeviceSelected += CastingPicker_CastingDeviceSelected;

            CoreWindow.GetForCurrentThread().KeyDown += NowPlayingBar_KeyDown;
            CoreWindow.GetForCurrentThread().KeyUp += NowPlayingBar_KeyUp;

            Visibility = Visibility.Collapsed;
        }

        private void NowPlayingBar_KeyUp(CoreWindow sender, KeyEventArgs args)
        {
            if (args.VirtualKey == VirtualKey.Space && !App.MViewModel.IsSearchActive)
            {
                TogglePlayPause();
            }
        }

        private void NowPlayingBar_KeyDown(CoreWindow sender, KeyEventArgs args)
        {
            if (IsCtrlPressed() && args.VirtualKey == VirtualKey.Right)
            {
                if ((App.PViewModel.PlayingSongs.IndexOf(App.PViewModel.CurrentSong) + 1) < App.PViewModel.PlayingSongs.Count && App.PViewModel.CurrentSong != null)
                {
                    _ = App.PViewModel.PlaybackList.MoveTo((uint)App.PViewModel.PlayingSongs.IndexOf(App.PViewModel.CurrentSong) + 1);
                }
            }
            else if (IsCtrlPressed() && args.VirtualKey == VirtualKey.Left)
            {
                if ((App.PViewModel.PlayingSongs.IndexOf(App.PViewModel.CurrentSong) - 1) > 0 && App.PViewModel.CurrentSong != null)
                {
                    _ = App.PViewModel.PlaybackList.MoveTo((uint)App.PViewModel.PlayingSongs.IndexOf(App.PViewModel.CurrentSong) - 1);
                }
            }
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
                _ = await connection.RequestStartCastingAsync(_player.GetAsCastingSource());
            });
        }

        private void UISettings_ColorValuesChanged(UISettings sender, object args)
        {
            HandleColorStyles();
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
            if (!PinMiniPlayer.IsChecked)
            {
                OverlayMenu.Visibility = Visibility.Collapsed;
                OverlayButton.Visibility = Visibility.Visible;
            }

            else
            {
                OverlayMenu.Visibility = Visibility.Collapsed;
                OverlayButton.Visibility = Visibility.Visible;
            }
        }

        private void SliderProgress_PointerCaptureLost(object sender, PointerRoutedEventArgs e)
        {
            _player.PlaybackSession.Position = TimeSpan.FromSeconds(SliderProgress.Value);
        }

        private void NowPlayingBar_Unloaded(object sender, RoutedEventArgs e)
        {
            UnregisterPropertyChangedCallback(BackgroundStylesProperty, _backgroundStylesPropertyToken);
            UnregisterPropertyChangedCallback(IsInNowPlayingPageProperty, _isInNowPlayingPageToken);
            UnregisterPropertyChangedCallback(OverlayBtnVisibilityProperty, _overlayBtnVisibilityPropertyToken);
        }

        private void NowPlayingBar_Loaded(object sender, RoutedEventArgs e)
        {
            HandleColorStyles();
            _player.PlaybackSession.PositionChanged += PlaybackSession_PositionChanged;
            _player.PlaybackSession.PlaybackStateChanged += PlaybackSession_PlaybackStateChanged;
            App.PViewModel.CurrentVideoChanged += PViewModel_CurrentVideoChanged;
            App.PViewModel.CurrentSongChanged += PViewModel_CurrentSongChanged;
            _backgroundStylesPropertyToken = RegisterPropertyChangedCallback(BackgroundStylesProperty, (sender1, dependencyObject) =>
            {
                if (dependencyObject == BackgroundStylesProperty)
                {
                    switch ((NowPlayingBarBackgroundStyles)sender1.GetValue(BackgroundStylesProperty))
                    {
                        case NowPlayingBarBackgroundStyles.Transparent:
                            Grid.Background = new SolidColorBrush(Colors.Transparent);
                            Effects.SetShadow(Parent1, EmptyDropShadow);
                            break;
                        case NowPlayingBarBackgroundStyles.Acrylic:
                        case NowPlayingBarBackgroundStyles.UseAlbumArt:
                            Grid.Background = BackgroundAcrylicBrush;
                            Effects.SetShadow(Parent1, DropShadow);
                            break;
                        default:
                            break;
                    }
                }
            });
            _isInNowPlayingPageToken = RegisterPropertyChangedCallback(IsInNowPlayingPageProperty, (sender1, dependencyObject) =>
            {
                if (dependencyObject == IsInNowPlayingPageProperty)
                {
                    if (IsInNowPlayingPage)
                    {
                        ShuffleButton.Margin = new Thickness(0, 10, 0, 10);
                        RepeatButton.Margin = new Thickness(0, 10, 0, 10);
                    }
                    else
                    {
                        ShuffleButton.Margin = new Thickness(10);
                        RepeatButton.Margin = new Thickness(10);
                    }
                }
            });
            _overlayBtnVisibilityPropertyToken = RegisterPropertyChangedCallback(OverlayBtnVisibilityProperty, (sender1, dependencyObject) =>
            {
                if (dependencyObject == OverlayBtnVisibilityProperty)
                {
                    if (OverlayBtnVisibility == Visibility.Collapsed)
                    {
                        OverlayButton.Visibility = Visibility.Collapsed;
                        OverlayButton1.Visibility = Visibility.Collapsed;
                    }
                }
            });
        }

        private async void PViewModel_CurrentSongChanged(object sender, EventArgs e)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                if ((NowPlayingBarBackgroundStyles)GetValue(BackgroundStylesProperty) == NowPlayingBarBackgroundStyles.UseAlbumArt && App.PViewModel.CurrentSong != null)
                {
                    try
                    {
                        CurrentSongAlbum = App.MViewModel.Albums.First(album => album.Title == App.PViewModel.CurrentSong.Album);
                        Uri imageUri = new(CurrentSongAlbum.Thumbnail);
                        RandomAccessStreamReference random = RandomAccessStreamReference.CreateFromUri(imageUri);
                        using IRandomAccessStream stream = await random.OpenReadAsync();
                        BitmapDecoder decoder = await BitmapDecoder.CreateAsync(stream);
                        ColorThief colorThief = new();
                        QuantizedColor color = await colorThief.GetColor(decoder);
                        BackgroundAcrylicBrush.TintOpacity = 100;
                        if (Application.Current.RequestedTheme == ApplicationTheme.Dark)
                        {
                            BackgroundAcrylicBrush.TintColor = Windows.UI.Color.FromArgb(_tintOpacity, (byte)(color.Color.R - lightThemeAdditions), (byte)(color.Color.G - lightThemeAdditions), (byte)(color.Color.B - lightThemeAdditions));
                        }
                        else
                        {
                            BackgroundAcrylicBrush.TintColor = Windows.UI.Color.FromArgb(_tintOpacity, (byte)(color.Color.R + lightThemeAdditions), (byte)(color.Color.G + lightThemeAdditions), (byte)(color.Color.B + lightThemeAdditions));
                        }
                        
                    } catch (InvalidOperationException)
                    {

                    }
                }

                RestoreVideoButton.Visibility = Visibility.Collapsed;
                Visibility = Visibility.Visible;
                SongArtist.Visibility = Visibility.Visible;

                await Task.Delay(TimeSpan.FromSeconds(30));

                LastFMHelper.ScrobbleTrack(SongArtist.Text, SongTitle.Text, App.LMViewModel.SessionKey, (s) =>
                {

                });
                AlbumArt.Stretch = Stretch.Uniform;
            });

        }

        private async void PViewModel_CurrentVideoChanged(object sender, EventArgs e)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                RestoreVideoButton.Visibility = Visibility.Visible;
                Visibility = Visibility.Visible;
                SongArtist.Visibility = Visibility.Collapsed;
                AlbumArt.Stretch = Stretch.UniformToFill;
            });
        }

        private void ShuffleButton_Click(object sender, RoutedEventArgs e)
        {
            App.PViewModel.PlaybackList.ShuffleEnabled = (bool)ShuffleButton.IsChecked;
        }

        private void RepeatButton_Click(object sender, RoutedEventArgs e)
        {
            App.PViewModel.PlaybackList.AutoRepeatEnabled = (bool)RepeatButton.IsChecked;
        }

        private async void PlaybackSession_PlaybackStateChanged(MediaPlaybackSession sender, object args)
        {
            if (sender.PlaybackState == MediaPlaybackState.Playing)
            {
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    PlayButtonIcon.Glyph = "\uE62E";
                    ToolTipService.SetToolTip(PlayButton, "Pause");
                    BufferingProgressRing.Visibility = Visibility.Collapsed;
                });
            }
            else if (sender.PlaybackState == MediaPlaybackState.Paused)
            {
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    PlayButtonIcon.Glyph = "\uF5B0";
                    ToolTipService.SetToolTip(PlayButton, "Play");
                    BufferingProgressRing.Visibility = Visibility.Collapsed;
                });
            }
            else if (sender.PlaybackState == MediaPlaybackState.Buffering)
            {
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    ToolTipService.SetToolTip(PlayButton, "Buffering...");
                    BufferingProgressRing.Visibility = Visibility.Visible;
                });
            }
            else if (sender.PlaybackState == MediaPlaybackState.Opening)
            {
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    ToolTipService.SetToolTip(PlayButton, "Opening...");
                    BufferingProgressRing.Visibility = Visibility.Visible;
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
                ViewModePreferences preferences = ViewModePreferences.CreateDefault(ApplicationViewMode.CompactOverlay);
                preferences.CustomSize = new Size(400, 400);
                _ = await ApplicationView.GetForCurrentView().TryEnterViewModeAsync(ApplicationViewMode.CompactOverlay, preferences);
                fontIcon.Glyph = "\uEE47";
            }
            else
            {
                ViewModePreferences preferences = ViewModePreferences.CreateDefault(ApplicationViewMode.CompactOverlay);
                preferences.CustomSize = new Size(600, 700);
                _ = await ApplicationView.GetForCurrentView().TryEnterViewModeAsync(ApplicationViewMode.Default, preferences);
                fontIcon.Glyph = "\uEE49";
            }
        }

        private async void OverlayButton1_Click(object sender, RoutedEventArgs e)
        {
            FontIcon fontIcon = OverlayButton1.FindChildren().First() as FontIcon;
            if (ApplicationView.GetForCurrentView().ViewMode != ApplicationViewMode.CompactOverlay)
            {
                MainPage.Current.AppTitleBar.Visibility = Visibility.Collapsed;
                ViewModePreferences preferences = ViewModePreferences.CreateDefault(ApplicationViewMode.CompactOverlay);
                preferences.CustomSize = new Size(400, 400);
                _ = await ApplicationView.GetForCurrentView().TryEnterViewModeAsync(ApplicationViewMode.CompactOverlay, preferences);
                fontIcon.Glyph = "\uEE49";
            }
            else
            {
                MainPage.Current.AppTitleBar.Visibility = Visibility.Visible;
                ViewModePreferences preferences = ViewModePreferences.CreateDefault(ApplicationViewMode.CompactOverlay);
                preferences.CustomSize = new Size(600, 700);
                _ = await ApplicationView.GetForCurrentView().TryEnterViewModeAsync(ApplicationViewMode.Default, preferences);
                fontIcon.Glyph = "\uEE47";
            }
        }

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.NewSize.Width >= 900)
            {
                DefaultVolumeControl.Visibility = Visibility.Visible;
                VolumeFlyoutButton.Visibility = Visibility.Collapsed;
                AlbumArtContainer.Visibility = Visibility.Visible;
                if (IsArtistShown)
                {
                    Grid.ColumnDefinitions[0].Width = new GridLength(0.45, GridUnitType.Star);
                }
                Grid.ColumnDefinitions[2].Width = new GridLength(0.5, GridUnitType.Star);
                VolumeFlyoutButton1.Visibility = Visibility.Collapsed;
                OverlayButton1.Visibility = Visibility.Collapsed;
            }
            else if (e.NewSize.Width >= 600)
            {
                DefaultVolumeControl.Visibility = Visibility.Visible;
                VolumeFlyoutButton.Visibility = Visibility.Collapsed;
                AlbumArtContainer.Visibility = Visibility.Visible;
                if (IsArtistShown)
                {
                    Grid.ColumnDefinitions[0].Width = new GridLength(0.45, GridUnitType.Star);
                }

                Grid.ColumnDefinitions[2].Width = new GridLength(0.5, GridUnitType.Star);
                VolumeFlyoutButton1.Visibility = Visibility.Collapsed;
                OverlayButton1.Visibility = Visibility.Collapsed;
            }
            else if (e.NewSize.Width >= 500)
            {
                DefaultVolumeControl.Visibility = Visibility.Collapsed;
                VolumeFlyoutButton.Visibility = Visibility.Visible;
                AlbumArtContainer.Visibility = Visibility.Collapsed;
                if (IsArtistShown)
                {
                    Grid.ColumnDefinitions[0].Width = new GridLength(0.45, GridUnitType.Star);
                }

                Grid.ColumnDefinitions[2].Width = new GridLength(0.5, GridUnitType.Star);
                VolumeFlyoutButton1.Visibility = Visibility.Collapsed;
                OverlayButton1.Visibility = Visibility.Collapsed;
            }
            else if (e.NewSize.Width >= 450)
            {
                DefaultVolumeControl.Visibility = Visibility.Collapsed;
                VolumeFlyoutButton.Visibility = Visibility.Visible;
                AlbumArtContainer.Visibility = Visibility.Collapsed;
                Grid.ColumnDefinitions[0].Width = new GridLength(0, GridUnitType.Star);
                Grid.ColumnDefinitions[2].Width = new GridLength(0.5, GridUnitType.Star);
                VolumeFlyoutButton1.Visibility = Visibility.Collapsed;
                OverlayButton1.Visibility = Visibility.Collapsed;
            }
            else
            {
                DefaultVolumeControl.Visibility = Visibility.Collapsed;
                VolumeFlyoutButton.Visibility = Visibility.Visible;
                Grid.ColumnDefinitions[0].Width = new GridLength(0, GridUnitType.Star);
                Grid.ColumnDefinitions[2].Width = new GridLength(0, GridUnitType.Star);
                AlbumArtContainer.Visibility = Visibility.Collapsed;
                VolumeFlyoutButton1.Visibility = Visibility.Visible;

                OverlayButton1.Visibility = Visibility.Visible;
                FontIcon fontIcon = OverlayButton1.FindChildren().First() as FontIcon;
                fontIcon.Glyph = ApplicationView.GetForCurrentView().ViewMode == ApplicationViewMode.CompactOverlay ? "\uEE47" : "\uEE49";
            }

            FontIcon fontIcon1 = OverlayButton.FindChildren().First() as FontIcon;
            fontIcon1.Glyph = ApplicationView.GetForCurrentView().ViewMode == ApplicationViewMode.CompactOverlay ? "\uEE47" : "\uEE49";
        }

        private void PlayButton_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            Button button = sender as Button;
            Border border = (button.Parent as Grid).Parent as Border;
            border.BorderBrush = PlayBorderBrush;
        }

        private void PlayButton_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            Button button = sender as Button;
            Border border = (button.Parent as Grid).Parent as Border;
            border.BorderBrush = new SolidColorBrush();
        }

        private void PreviousButton_Click(object sender, RoutedEventArgs e)
        {
            if ((App.PViewModel.PlayingSongs.IndexOf(App.PViewModel.CurrentSong) - 1) > 0 && App.PViewModel.CurrentSong != null)
            {
                _ = App.PViewModel.PlaybackList.MoveTo((uint)App.PViewModel.PlayingSongs.IndexOf(App.PViewModel.CurrentSong) - 1);
            }
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            if ((App.PViewModel.PlayingSongs.IndexOf(App.PViewModel.CurrentSong) + 1) < App.PViewModel.PlayingSongs.Count && App.PViewModel.CurrentSong != null)
            {
                _ = App.PViewModel.PlaybackList.MoveTo((uint)App.PViewModel.PlayingSongs.IndexOf(App.PViewModel.CurrentSong) + 1);
            }
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
                default:
                    break;
            }
        }

        private void FullScreen_Click(object sender, RoutedEventArgs e)
        {
            ApplicationView view = ApplicationView.GetForCurrentView();
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
                    FullScreenButton.Text = "Exit full screen";
                    FullScreenIcon.Glyph = "\uE73F";
                    // The SizeChanged event will be raised when the entry to full-screen mode is complete.
                }
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

        private async void HandleColorStyles()
        {
            if (Application.Current.RequestedTheme == ApplicationTheme.Light)
            {

                _tintOpacity = 130;
                BackgroundAcrylicBrush.TintLuminosityOpacity = 130;

                //_tintOpacity = 130;
                //BackgroundAcrylicBrush.TintLuminosityOpacity = 130;
            }
            else if (Application.Current.RequestedTheme == ApplicationTheme.Dark)
            {

                _tintOpacity = 130;
                BackgroundAcrylicBrush.TintLuminosityOpacity = 130;
                //_tintOpacity = 100;
                //BackgroundAcrylicBrush.TintLuminosityOpacity = 0;
            }
        }

        private bool IsCtrlPressed()
        {
            CoreVirtualKeyStates state = CoreWindow.GetForCurrentThread().GetKeyState(VirtualKey.Control);
            return (state & CoreVirtualKeyStates.Down) == CoreVirtualKeyStates.Down;
        }

        public enum NowPlayingBarBackgroundStyles
        {
            Transparent,
            Acrylic,
            UseAlbumArt
        }

        private void VolumeIcon_Click(object sender, RoutedEventArgs e)
        {
            FontIcon volumeIcon = VolumeIconViewBoxMediaControl.Content as FontIcon;
            if (!_player.IsMuted)
            {
                _player.IsMuted = true;
                VolumeSlider.Value = 0;
                volumeIcon.Glyph = "\uE74F";
            }
            else
            {
                _player.IsMuted = false;
                VolumeSlider.Value = VolumeSlider.Maximum;
                volumeIcon.Glyph = "\uE767";
            }
        }

        private void VolumeSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            FontIcon volumeIcon = VolumeIconViewBoxMediaControl.Content as FontIcon;

            switch (VolumeProgressText.Text)
            {
                case "0":
                    volumeIcon.Glyph = "\uE74F";
                    break;
                case "1":
                case "10":
                    volumeIcon.Glyph = "\uE993";
                    break;
                case "34":
                    volumeIcon.Glyph = "\uE994";
                    break;
                case "67":
                    volumeIcon.Glyph = "\uE995";
                    break;
                default:
                    break;
            }
        }

        private void PlaybackInfo_Click(object sender, RoutedEventArgs e)
        {
            if (!App.PViewModel.CurrentPlaybackItem.IsVideo)
            {
                _ = MainPage.Current.ContentFrame.Navigate(typeof(AlbumSongsPage), CurrentSongAlbum);
            }
            else
            {
                if (Window.Current.Content is Frame rootFrame)
                {
                    _ = rootFrame.Navigate(typeof(VideoPlaybackPage));
                }
            }
        }

        private async void Props_Click(object sender, RoutedEventArgs e)
        {
            SelectedSong = App.PViewModel.CurrentSong;
            await SelectedSong.StartEdit();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            QueueFrame.Navigate(typeof(NPBarQueuePage));
        }
    }
}
