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
        private AppBarButton _restoreButton;

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
        /// Gets or sets a value that indicates whether a user
        /// can restore a view of the media.
        /// </summary>
        public bool IsRestoreEnabled
        {
            get => (bool)GetValue(IsRestoreEnabledProperty);
            set => SetValue(IsRestoreEnabledProperty, value);
        }

        /// <summary>
        /// Gets or sets a value that indicates whether the restore
        /// button is shown.
        /// </summary>
        public bool IsRestoreButtonVisible
        {
            get => (bool)GetValue(IsRestoreVisibleProperty);
            set => SetValue(IsRestoreVisibleProperty, value);
        }

        /// <summary>
        /// Invoked when the shuffle button is clicked. Event arg
        /// corresponds to the IsChecked value of the ToggleButton.
        /// </summary>
        public event EventHandler<bool> ShufflingChanged;

        /// <summary>
        /// Invoked when the restore button is clicked.
        /// </summary>
        public event RoutedEventHandler RestoreButtonClick;

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

        public readonly static DependencyProperty IsShuffleButtonCheckedProperty =
            DependencyProperty.Register(nameof(IsShuffleButtonChecked), typeof(bool),
                typeof(RiseMediaTransportControls), new PropertyMetadata(false, OnShuffleButtonCheckedChanged));

        public readonly static DependencyProperty IsRestoreEnabledProperty =
            DependencyProperty.Register(nameof(IsRestoreEnabled), typeof(bool),
                typeof(RiseMediaTransportControls), new PropertyMetadata(false, OnRestoreEnabledChanged));

        public readonly static DependencyProperty IsRestoreVisibleProperty =
            DependencyProperty.Register(nameof(IsRestoreButtonVisible), typeof(bool),
                typeof(RiseMediaTransportControls), new PropertyMetadata(false, OnRestoreButtonVisibleChanged));
    }

    // Event handlers
    public sealed partial class RiseMediaTransportControls : MediaTransportControls
    {
        private static void HandleControlEnabled(Control control, bool enabled)
        {
            if (control != null)
            {
                control.IsEnabled = enabled;
            }
        }

        private static void HandleElementVisibility(FrameworkElement element, bool visible)
        {
            if (element != null)
            {
                element.Visibility = visible ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private static void HandleToggleChecked(ToggleButton toggle, bool? isChecked)
        {
            if (toggle != null)
            {
                toggle.IsChecked = isChecked;
            }
        }

        private static void OnShuffleEnabledChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            if (sender is RiseMediaTransportControls rmtc)
            {
                HandleControlEnabled(rmtc._shuffleButton, (bool)args.NewValue);
            }
        }

        private static void OnShuffleButtonVisibleChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            if (sender is RiseMediaTransportControls rmtc)
            {
                HandleElementVisibility(rmtc._shuffleButton, (bool)args.NewValue);
            }
        }

        private static void OnShuffleButtonCheckedChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            if (sender is RiseMediaTransportControls rmtc)
            {
                HandleToggleChecked(rmtc._shuffleButton, (bool)args.NewValue);
            }
        }

        private static void OnRestoreEnabledChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            if (sender is RiseMediaTransportControls rmtc)
            {
                HandleControlEnabled(rmtc._restoreButton, (bool)args.NewValue);
            }
        }

        private static void OnRestoreButtonVisibleChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            if (sender is RiseMediaTransportControls rmtc)
            {
                HandleElementVisibility(rmtc._restoreButton, (bool)args.NewValue);
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

            _restoreButton = GetTemplateChild("RestoreButton") as AppBarButton;
            _restoreButton.Click += (s, e) => RestoreButtonClick?.Invoke(s, e);

            HandleControlEnabled(_shuffleButton, IsShuffleEnabled);
            HandleElementVisibility(_shuffleButton, IsShuffleButtonVisible);
            HandleToggleChecked(_shuffleButton, IsShuffleButtonChecked);

            HandleControlEnabled(_restoreButton, IsRestoreEnabled);
            HandleElementVisibility(_restoreButton, IsRestoreButtonVisible);

            base.OnApplyTemplate();
        }
    }
}
