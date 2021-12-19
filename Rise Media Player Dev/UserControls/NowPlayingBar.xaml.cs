using System;
using System.Linq;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI;
using Windows.Media.Playback;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Microsoft.Toolkit.Uwp.UI;
using Windows.Storage.Streams;
using Windows.Graphics.Imaging;
using ColorThiefDotNet;
using Rise.App.Converters;
using Rise.App.ViewModels;
using Rise.App.Common;
using Rise.App.Views;
using System.Diagnostics;

namespace Rise.App.UserControls
{
    public sealed partial class NowPlayingBar : UserControl
    {
        #region Variables
        private MediaPlayer _player = App.PViewModel.Player;
        private byte _tintOpacity = 180;

        private AlbumViewModel CurrentSongAlbum;
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
            set
            {
                SetValue(BackgroundStylesProperty, value);
            }
        }

        public static readonly DependencyProperty IsInNowPlayingPageProperty = DependencyProperty.Register("IsInNowPlayingPage", typeof(bool), typeof(NowPlayingBar), new PropertyMetadata(null));

        private long _isInNowPlayingPageToken;

        public bool IsInNowPlayingPage
        {
            get => (bool)GetValue(IsInNowPlayingPageProperty);
            set
            {
                SetValue(IsInNowPlayingPageProperty, value);
            }
        }

        public static readonly DependencyProperty OverlayBtnVisibilityProperty = DependencyProperty.Register("OverlayBtnVisibility", typeof(Visibility), typeof(NowPlayingBar), new PropertyMetadata(null));

        private long _overlayBtnVisibilityPropertyToken;

        public Visibility OverlayBtnVisibility
        {
            get => (Visibility)GetValue(OverlayBtnVisibilityProperty);
            set
            {
                SetValue(OverlayBtnVisibilityProperty, value);
            }
        }

        #endregion

        public NowPlayingBar()
        {
            this.InitializeComponent();

            DataContext = App.PViewModel;
            Loaded += NowPlayingBar_Loaded;
            Unloaded += NowPlayingBar_Unloaded;
            UISettings uiSettings = new UISettings();
            uiSettings.ColorValuesChanged += UISettings_ColorValuesChanged;
        }

        private void UISettings_ColorValuesChanged(UISettings sender, object args)
        {
            HandleColorStyles();
        }

        #region Listeners

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
                    } else
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
                    CurrentSongAlbum = App.MViewModel.Albums.First(album => album.Title == App.PViewModel.CurrentSong.Album);
                    Uri imageUri = new Uri(CurrentSongAlbum.Thumbnail);
                    RandomAccessStreamReference random = RandomAccessStreamReference.CreateFromUri(imageUri);
                    using (IRandomAccessStream stream = await random.OpenReadAsync())
                    {
                        var decoder = await BitmapDecoder.CreateAsync(stream);
                        var colorThief = new ColorThief();
                        var color = await colorThief.GetColor(decoder);
                        BackgroundAcrylicBrush.TintOpacity = 100;
                        BackgroundAcrylicBrush.TintColor = Windows.UI.Color.FromArgb(_tintOpacity, color.Color.R, color.Color.G, color.Color.B);
                    }
                }
                RestoreVideoButton.Visibility = Visibility.Collapsed;
            });
        }

        private async void PViewModel_CurrentVideoChanged(object sender, EventArgs e)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                RestoreVideoButton.Visibility = Visibility.Visible;
            });
        }

        private void ShuffleButton_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)ShuffleButton.IsChecked)
            {
                App.PViewModel.PlaybackList.ShuffleEnabled = true;
            } else
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
                ToolTipService.SetToolTip(PlayButton, "Buffering...");
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

        private async void OverlayButton1_Click(object sender, RoutedEventArgs e)
        {
            FontIcon fontIcon = OverlayButton1.FindChildren().First() as FontIcon;
            if (ApplicationView.GetForCurrentView().ViewMode != ApplicationViewMode.CompactOverlay)
            {
                var preferences = ViewModePreferences.CreateDefault(ApplicationViewMode.CompactOverlay);
                preferences.CustomSize = new Size(400, 400);
                _ = await ApplicationView.GetForCurrentView().TryEnterViewModeAsync(ApplicationViewMode.CompactOverlay, preferences);
                fontIcon.Glyph = "\uEE49";
            }
            else
            {
                var preferences = ViewModePreferences.CreateDefault(ApplicationViewMode.CompactOverlay);
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
                DefaultVolumeControl.Visibility = Visibility.Collapsed;
                VolumeFlyoutButton.Visibility = Visibility.Visible;
                AlbumArtContainer.Visibility = Visibility.Visible;
                if (IsArtistShown) Grid.ColumnDefinitions[0].Width = new GridLength(0.45, GridUnitType.Star);
                Grid.ColumnDefinitions[2].Width = new GridLength(0.5, GridUnitType.Star);
                VolumeFlyoutButton1.Visibility = Visibility.Collapsed;
                OverlayButton1.Visibility = Visibility.Collapsed;
            } else if (e.NewSize.Width >= 480)
            {
                DefaultVolumeControl.Visibility = Visibility.Collapsed;
                VolumeFlyoutButton.Visibility = Visibility.Visible;
                AlbumArtContainer.Visibility = Visibility.Collapsed;
                if (IsArtistShown) Grid.ColumnDefinitions[0].Width = new GridLength(0.45, GridUnitType.Star);
                Grid.ColumnDefinitions[2].Width = new GridLength(0.5, GridUnitType.Star);
                VolumeFlyoutButton1.Visibility = Visibility.Collapsed;
                OverlayButton1.Visibility = Visibility.Collapsed;
            }
            else if (e.NewSize.Width >= 400)
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
                if (ApplicationView.GetForCurrentView().ViewMode == ApplicationViewMode.CompactOverlay)
                {
                    fontIcon.Glyph = "\uEE47";
                }
                else fontIcon.Glyph = "\uEE49";
            }

            FontIcon fontIcon1 = OverlayButton.FindChildren().First() as FontIcon;
            if (ApplicationView.GetForCurrentView().ViewMode == ApplicationViewMode.CompactOverlay)
            {
                fontIcon1.Glyph = "\uEE47";
            }
            else fontIcon1.Glyph = "\uEE49";
        }

        private void PlayButton_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            Button button = sender as Button;
            Border border = button.Parent as Border;
            border.BorderBrush = PlayBorderBrush;
        }

        private void PlayButton_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            Button button = sender as Button;
            Border border = button.Parent as Border;
            border.BorderBrush = new SolidColorBrush();
        }

        private void PreviousButton_Click(object sender, RoutedEventArgs e)
        {
            if ((App.PViewModel.PlayingSongs.IndexOf(App.PViewModel.CurrentSong) - 1) > 0 && App.PViewModel.CurrentSong != null)
            {
                App.PViewModel.PlaybackList.MoveTo((uint)App.PViewModel.PlayingSongs.IndexOf(App.PViewModel.CurrentSong) - 1);
            }
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            if ((App.PViewModel.PlayingSongs.IndexOf(App.PViewModel.CurrentSong) + 1) < App.PViewModel.PlayingSongs.Count && App.PViewModel.CurrentSong != null)
            {
                App.PViewModel.PlaybackList.MoveTo((uint)App.PViewModel.PlayingSongs.IndexOf(App.PViewModel.CurrentSong) + 1);
            }
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

        private void HandleColorStyles()
        {
            if (Application.Current.RequestedTheme == ApplicationTheme.Light)
            {
                _tintOpacity = 180;
            }
            else if (Application.Current.RequestedTheme == ApplicationTheme.Dark)
            {
                _tintOpacity = 130;
            }
        }

        public enum NowPlayingBarBackgroundStyles
        {
            Transparent,
            Acrylic,
            UseAlbumArt
        }
    }
}