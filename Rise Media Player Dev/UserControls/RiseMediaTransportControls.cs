using Rise.App.Views;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace Rise.App.UserControls
{
    /// <summary>
    /// Custom media transport controls implementation for RiseMP.
    /// </summary>
    public sealed partial class RiseMediaTransportControls : MediaTransportControls
    {
        private ToggleButton _shuffleButton;
        private AppBarButton _videoRestoreBtn;

        /// <summary>
        /// Gets or sets a value that indicates whether a user
        /// can shuffle the playback of the media.
        /// </summary>
        public bool IsShuffleEnabled
        {
            get => (bool)GetValue(IsShuffleEnabledProperty);
            set => SetValue(IsShuffleEnabledProperty, value);
        }

        /// <summary>
        /// Gets or sets a value that indicates whether the shuffle
        /// button is shown.
        /// </summary>
        public bool IsShuffleButtonVisible
        {
            get => (bool)GetValue(IsShuffleButtonVisibleProperty);
            set => SetValue(IsShuffleButtonVisibleProperty, value);
        }

        /// <summary>
        /// Gets or sets a value that indicates whether a user
        /// can shuffle the playback of the media.
        /// </summary>
        public bool IsVideoRestoreButtonEnabled
        {
            get => (bool)GetValue(IsVideoResButtonEnabledProperty);
            set => SetValue(IsVideoResButtonEnabledProperty, value);
        }

        /// <summary>
        /// Gets or sets a value that indicates whether the shuffle
        /// button is shown.
        /// </summary>
        public bool IsVideoRestoreButtonVisible
        {
            get => (bool)GetValue(IsVideoResButtonVisibleProperty);
            set => SetValue(IsVideoResButtonVisibleProperty, value);
        }

        /// <summary>
        /// Invoked when the shuffle button is clicked. Event arg
        /// corresponds to the IsChecked value of the ToggleButton.
        /// </summary>
        public event EventHandler<bool> ShufflingChanged;

        /// <summary>
        /// The item to display next to the controls. When using
        /// compact mode, it gets hidden.
        /// </summary>
        public object DisplayItem
        {
            get => GetValue(DisplayItemProperty);
            set => SetValue(DisplayItemProperty, value);
        }

        /// <summary>
        /// Gets or sets the <see cref="DisplayItem"/> visibility.
        /// </summary>
        public Visibility DisplayItemVisibility
        {
            get => (Visibility)GetValue(DisplayItemVisibilityProperty);
            set => SetValue(DisplayItemVisibilityProperty, value);
        }

        /// <summary>
        /// The template for <see cref="DisplayItem"/>.
        /// </summary>
        public DataTemplate DisplayItemTemplate
        {
            get => (DataTemplate)GetValue(DisplayItemTemplateProperty);
            set => SetValue(DisplayItemTemplateProperty, value);
        }

        /// <summary>
        /// The template selector for <see cref="DisplayItem"/>.
        /// </summary>
        public DataTemplateSelector DisplayItemTemplateSelector
        {
            get => (DataTemplateSelector)GetValue(DisplayItemTemplateProperty);
            set => SetValue(DisplayItemTemplateProperty, value);
        }
    }

    // Dependency Properties
    public sealed partial class RiseMediaTransportControls : MediaTransportControls
    {
        public readonly static DependencyProperty DisplayItemProperty =
            DependencyProperty.Register(nameof(DisplayItem), typeof(object),
                typeof(RiseMediaTransportControls), new PropertyMetadata(null));

        public readonly static DependencyProperty DisplayItemVisibilityProperty =
            DependencyProperty.Register(nameof(DisplayItemVisibility), typeof(Visibility),
                typeof(RiseMediaTransportControls), new PropertyMetadata(Visibility.Collapsed));

        public readonly static DependencyProperty DisplayItemTemplateProperty =
            DependencyProperty.Register(nameof(DisplayItemTemplate), typeof(DataTemplate),
                typeof(RiseMediaTransportControls), new PropertyMetadata(null));

        public readonly static DependencyProperty DisplayItemTemplateSelectorProperty =
            DependencyProperty.Register(nameof(DisplayItemTemplateSelector), typeof(DataTemplateSelector),
                typeof(RiseMediaTransportControls), new PropertyMetadata(null));

        public readonly static DependencyProperty IsShuffleEnabledProperty =
            DependencyProperty.Register(nameof(IsShuffleEnabled), typeof(bool),
                typeof(RiseMediaTransportControls), new PropertyMetadata(false, OnShuffleEnabledChanged));

        public readonly static DependencyProperty IsShuffleButtonVisibleProperty =
            DependencyProperty.Register(nameof(IsShuffleButtonVisible), typeof(bool),
                typeof(RiseMediaTransportControls), new PropertyMetadata(false, OnShuffleButtonVisibleChanged));

        public readonly static DependencyProperty IsVideoResButtonEnabledProperty =
            DependencyProperty.Register(nameof(IsShuffleEnabled), typeof(bool),
                typeof(RiseMediaTransportControls), new PropertyMetadata(false, OnShuffleEnabledChanged));

        public readonly static DependencyProperty IsVideoResButtonVisibleProperty =
            DependencyProperty.Register(nameof(IsShuffleButtonVisible), typeof(bool),
                typeof(RiseMediaTransportControls), new PropertyMetadata(false, OnVideoResBtnVisibleChanged));
    }

    // Event handlers
    public sealed partial class RiseMediaTransportControls : MediaTransportControls
    {
        private static void OnShuffleEnabledChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            if (sender is RiseMediaTransportControls rmtc)
            {
                if (rmtc._shuffleButton != null)
                {
                    HandleShuffleEnabled(rmtc, (bool)args.NewValue);
                }
            }
        }

        private static void OnShuffleButtonVisibleChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            if (sender is RiseMediaTransportControls rmtc)
            {
                if (rmtc._shuffleButton != null)
                {
                    HandleShuffleVisibility(rmtc, (bool)args.NewValue);
                }
            }
        }

        private static void HandleShuffleEnabled(RiseMediaTransportControls rmtc, bool enabled)
        {
            rmtc._shuffleButton.IsEnabled = enabled;
        }

        private static void HandleShuffleVisibility(RiseMediaTransportControls rmtc, bool visible)
        {
            rmtc._shuffleButton.Visibility = visible ? Visibility.Visible : Visibility.Collapsed;
        }

        private static void OnVideoResBtnEnabledChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            if (sender is RiseMediaTransportControls rmtc)
            {
                if (rmtc._videoRestoreBtn != null)
                {
                    HandleVideoResBtnEnabled(rmtc, (bool)args.NewValue);
                }
            }
        }

        private static void OnVideoResBtnVisibleChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            if (sender is RiseMediaTransportControls rmtc)
            {
                if (rmtc._videoRestoreBtn != null)
                {
                    HandleVideoResBtnVisibility(rmtc, (bool)args.NewValue);
                }
            }
        }

        private static void HandleVideoResBtnEnabled(RiseMediaTransportControls rmtc, bool enabled)
        {
            rmtc._videoRestoreBtn.IsEnabled = enabled;
        }

        private static void HandleVideoResBtnVisibility(RiseMediaTransportControls rmtc, bool visible)
        {
            if (visible)
            {
                rmtc._videoRestoreBtn.Visibility = App.PViewModel.CurrentPlaybackItem.IsVideo ? Visibility.Visible : Visibility.Collapsed;
            }
            else
            {
                rmtc._videoRestoreBtn.Visibility = Visibility.Collapsed;
            }
        }

        private void VideoRestoreBtnClick(object sender, RoutedEventArgs e)
        {
            if (Window.Current.Content is Frame rootFrame)
            {
                rootFrame.Navigate(typeof(VideoPlaybackPage));
            }
        }

        private void CurrentMediaChanged(object sender, EventArgs e)
        {
            if (_videoRestoreBtn != null)
            {
                if (IsVideoRestoreButtonVisible)
                {
                    _videoRestoreBtn.Visibility = App.PViewModel.CurrentPlaybackItem.IsVideo ? Visibility.Visible : Visibility.Collapsed;
                } else
                {
                    _videoRestoreBtn.Visibility = Visibility.Collapsed;
                }
            }
        }
    }

    // Constructor, Overrides
    public sealed partial class RiseMediaTransportControls : MediaTransportControls
    {
        public RiseMediaTransportControls()
        {
            DefaultStyleKey = typeof(RiseMediaTransportControls);
        }

        protected override void OnApplyTemplate()
        {
            _shuffleButton = GetTemplateChild("ShuffleButton") as ToggleButton;

            _shuffleButton.Checked += (s, e) => ShufflingChanged?.Invoke(s, true);
            _shuffleButton.Unchecked += (s, e) => ShufflingChanged?.Invoke(s, false);

            _videoRestoreBtn = GetTemplateChild("GoToVideoPlaybackPage") as AppBarButton;

            _videoRestoreBtn.Click += VideoRestoreBtnClick;
            App.PViewModel.CurrentMediaChanged += CurrentMediaChanged;

            HandleShuffleEnabled(this, IsShuffleEnabled);
            HandleShuffleVisibility(this, IsShuffleButtonVisible);

            base.OnApplyTemplate();
        }
    }
}
