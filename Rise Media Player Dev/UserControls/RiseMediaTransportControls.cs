using Rise.App.Dialogs;
using Rise.App.ViewModels;
using Rise.App.Views;
using Rise.Common.Enums;
using Rise.Common.Extensions;
using System;
using System.Windows.Input;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
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
        /// <summary>
        /// Gets or sets a value that indicates the horizontal
        /// alignment for the main playback controls.
        /// </summary>
        public HorizontalAlignment HorizontalControlsAlignment
        {
            get => (HorizontalAlignment)GetValue(HorizontalControlsAlignmentProperty);
            set => SetValue(HorizontalControlsAlignmentProperty, value);
        }

        /// <summary>
        /// Gets or sets a value that indicates the way timeline
        /// elements are displayed.
        /// </summary>
        public SliderDisplayModes TimelineDisplayMode
        {
            get => (SliderDisplayModes)GetValue(TimelineDisplayModeProperty);
            set => SetValue(TimelineDisplayModeProperty, value);
        }

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
        /// Gets or sets a value that indicates whether the shuffle
        /// button is checked.
        /// </summary>
        public bool IsShuffleButtonChecked
        {
            get => (bool)GetValue(IsShuffleButtonCheckedProperty);
            set => SetValue(IsShuffleButtonCheckedProperty, value);
        }

        /// <summary>
        /// Gets or sets a command that runs whenever one of the
        /// overlay buttons is clicked, with the desired view mode
        /// as a parameter.
        /// </summary>
        public ICommand OverlayCommand
        {
            get => (ICommand)GetValue(OverlayCommandProperty);
            set => SetValue(OverlayCommandProperty, value);
        }

        /// <summary>
        /// Gets or sets a value that indicates whether a user
        /// can open the now playing page.
        /// </summary>
        public bool IsOverlayEnabled
        {
            get => (bool)GetValue(IsOverlayEnabledProperty);
            set => SetValue(IsOverlayEnabledProperty, value);
        }

        /// <summary>
        /// Gets or sets a value that indicates whether the overlay
        /// button is shown.
        /// </summary>
        public bool IsOverlayButtonVisible
        {
            get => (bool)GetValue(IsOverlayButtonVisibleProperty);
            set => SetValue(IsOverlayButtonVisibleProperty, value);
        }

        /// <summary>
        /// Gets or sets a value that indicates whether the properties
        /// button is enabled.
        /// </summary>
        public bool IsPropertiesEnabled
        {
            get => (bool)GetValue(IsPropertiesEnabledProperty);
            set => SetValue(IsPropertiesEnabledProperty, value);
        }

        /// <summary>
        /// Gets or sets a value that indicates whether the properties
        /// button is shown.
        /// </summary>
        public bool IsPropertiesButtonVisible
        {
            get => (bool)GetValue(IsPropertiesButtonVisibleProperty);
            set => SetValue(IsPropertiesButtonVisibleProperty, value);
        }

        /// <summary>
        /// Gets or sets a value that indicates whether the equalizer
        /// button is enabled.
        /// </summary>
        public bool IsEqualizerButtonEnabled
        {
            get => (bool)GetValue(IsEqualizerButtonEnabledProperty);
            set => SetValue(IsEqualizerButtonEnabledProperty, value);
        }

        /// <summary>
        /// Gets or sets a value that indicates whether the equalizer
        /// button is shown.
        /// </summary>
        public bool IsEqualizerButtonVisible
        {
            get => (bool)GetValue(IsEqualizerButtonVisibleProperty);
            set => SetValue(IsEqualizerButtonVisibleProperty, value);
        }

        /// <summary>
        /// Gets or sets a value that indicates whether the queue
        /// button is enabled.
        /// </summary>
        public bool IsQueueButtonEnabled
        {
            get => (bool)GetValue(IsQueueButtonEnabledProperty);
            set => SetValue(IsQueueButtonEnabledProperty, value);
        }

        /// <summary>
        /// Gets or sets a value that indicates whether the queue
        /// button is shown.
        /// </summary>
        public bool IsQueueButtonVisible
        {
            get => (bool)GetValue(IsQueueButtonVisibleProperty);
            set => SetValue(IsQueueButtonVisibleProperty, value);
        }

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

        /// <summary>
        /// The flyout to display when clicking the queue button.
        /// </summary>
        public Flyout QueueFlyout
        {
            get => (Flyout)GetValue(QueueFlyoutProperty);
            set => SetValue(QueueFlyoutProperty, value);
        }
    }

    // Dependency Properties
    public sealed partial class RiseMediaTransportControls : MediaTransportControls
    {
        public readonly static DependencyProperty HorizontalControlsAlignmentProperty =
            DependencyProperty.Register(nameof(HorizontalControlsAlignment), typeof(HorizontalAlignment),
                typeof(RiseMediaTransportControls), new PropertyMetadata(HorizontalAlignment.Center));

        public readonly static DependencyProperty TimelineDisplayModeProperty =
            DependencyProperty.Register(nameof(TimelineDisplayMode), typeof(SliderDisplayModes),
                typeof(RiseMediaTransportControls), new PropertyMetadata(SliderDisplayModes.Full, TimelineDisplayModeChanged));

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
                typeof(RiseMediaTransportControls), new PropertyMetadata(false));

        public readonly static DependencyProperty IsShuffleButtonVisibleProperty =
            DependencyProperty.Register(nameof(IsShuffleButtonVisible), typeof(bool),
                typeof(RiseMediaTransportControls), new PropertyMetadata(false));

        public readonly static DependencyProperty IsShuffleButtonCheckedProperty =
            DependencyProperty.Register(nameof(IsShuffleButtonChecked), typeof(bool),
                typeof(RiseMediaTransportControls), new PropertyMetadata(false));

        public readonly static DependencyProperty OverlayCommandProperty =
            DependencyProperty.Register(nameof(OverlayCommand), typeof(ICommand),
                typeof(RiseMediaTransportControls), new PropertyMetadata(null));

        public readonly static DependencyProperty IsOverlayEnabledProperty =
            DependencyProperty.Register(nameof(IsOverlayEnabled), typeof(bool),
                typeof(RiseMediaTransportControls), new PropertyMetadata(false));

        public readonly static DependencyProperty IsOverlayButtonVisibleProperty =
            DependencyProperty.Register(nameof(IsOverlayButtonVisible), typeof(bool),
                typeof(RiseMediaTransportControls), new PropertyMetadata(false));

        public readonly static DependencyProperty IsPropertiesEnabledProperty =
            DependencyProperty.Register(nameof(IsPropertiesEnabled), typeof(bool),
                typeof(RiseMediaTransportControls), new PropertyMetadata(false));

        public readonly static DependencyProperty IsPropertiesButtonVisibleProperty =
            DependencyProperty.Register(nameof(IsPropertiesButtonVisible), typeof(bool),
                typeof(RiseMediaTransportControls), new PropertyMetadata(false));

        public readonly static DependencyProperty IsQueueButtonEnabledProperty =
            DependencyProperty.Register(nameof(IsQueueButtonEnabled), typeof(bool),
                typeof(RiseMediaTransportControls), new PropertyMetadata(false));

        public readonly static DependencyProperty IsQueueButtonVisibleProperty =
            DependencyProperty.Register(nameof(IsQueueButtonVisible), typeof(bool),
                typeof(RiseMediaTransportControls), new PropertyMetadata(false));

        public readonly static DependencyProperty IsEqualizerButtonEnabledProperty =
            DependencyProperty.Register(nameof(IsEqualizerButtonEnabled), typeof(bool),
                typeof(RiseMediaTransportControls), new PropertyMetadata(false));

        public readonly static DependencyProperty IsEqualizerButtonVisibleProperty =
            DependencyProperty.Register(nameof(IsEqualizerButtonVisible), typeof(bool),
                typeof(RiseMediaTransportControls), new PropertyMetadata(false));

        public readonly static DependencyProperty QueueFlyoutProperty =
            DependencyProperty.Register(nameof(QueueFlyout), typeof(Flyout),
                typeof(RiseMediaTransportControls), new PropertyMetadata(null));
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
            if (GetTemplateChild("OverlayButton") is ButtonBase overlayButton)
                overlayButton.CommandParameter = ApplicationViewMode.Default;

            if (GetTemplateChild("MiniViewButton") is ButtonBase miniButton)
                miniButton.CommandParameter = ApplicationViewMode.CompactOverlay;

            if (GetTemplateChild("InfoPropertiesButton") is ButtonBase propertiesButton)
                propertiesButton.Click += PropertiesButtonClick;

            if (GetTemplateChild("EqualizerButton") is ButtonBase equalizerButton)
                equalizerButton.Click += EqualizerButtonClick;

            base.OnApplyTemplate();
        }
    }

    // Event handlers
    public sealed partial class RiseMediaTransportControls : MediaTransportControls
    {
        private static async void TimelineDisplayModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is RiseMediaTransportControls rmtc)
                await rmtc.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    switch ((SliderDisplayModes)e.NewValue)
                    {
                        case SliderDisplayModes.Hidden:
                            VisualStateManager.GoToState(rmtc, "HiddenTimelineState", true);
                            break;
                        case SliderDisplayModes.Minimal:
                            VisualStateManager.GoToState(rmtc, "MinimalTimelineState", true);
                            break;
                        case SliderDisplayModes.SliderOnly:
                            VisualStateManager.GoToState(rmtc, "SliderOnlyTimelineState", true);
                            break;
                        case SliderDisplayModes.Full:
                            VisualStateManager.GoToState(rmtc, "FullTimelineState", true);
                            break;
                    }
                });
        }

        private async void PropertiesButtonClick(object sender, RoutedEventArgs e)
        {
            if (App.MPViewModel.PlayingItem is SongViewModel song && !App.MPViewModel.PlayingItem.IsOnline)
            {
                try
                {
                    StorageFile file = await StorageFile.GetFileFromPathAsync(song.Location);
                    if (file != null)
                    {
                        SongPropertiesViewModel props = new(song, file.DateCreated)
                        {
                            FileProps = await file.GetBasicPropertiesAsync()
                        };

                        _ = await typeof(SongPropertiesPage).
                            PlaceInApplicationViewAsync(props, 380, 550, true);
                    }
                }
                catch
                {

                }
            }
        }

        private void EqualizerButtonClick(object sender, RoutedEventArgs e)
            => _ = new EqualizerDialog().ShowAsync();
    }
}