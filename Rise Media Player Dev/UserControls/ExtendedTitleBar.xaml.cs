using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Media;

namespace Rise.App.UserControls
{
    /// <summary>
    /// This control makes it easy to extend your app's titlebar.
    /// Just drop it into your view and you'll be good to go.
    /// </summary>
    public sealed partial class ExtendedTitleBar : UserControl
    {
        #region Private fields
        private readonly static DependencyProperty IconProperty =
            DependencyProperty.Register(nameof(Icon), typeof(string), typeof(ExtendedTitleBar), new PropertyMetadata("ms-appx:///Assets/App/Titlebar.png"));

        private readonly static DependencyProperty TitleProperty =
            DependencyProperty.Register(nameof(Title), typeof(string), typeof(ExtendedTitleBar), new PropertyMetadata(null));

        private readonly static DependencyProperty ShowIconProperty =
            DependencyProperty.Register(nameof(ShowIcon), typeof(bool), typeof(ExtendedTitleBar), new PropertyMetadata(true));
        #endregion

        #region Public properties/fields
        /// <summary>
        /// The icon displayed on the titlebar.
        /// </summary>
        public string Icon
        {
            get => (string)GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }

        /// <summary>
        /// The text displayed on the titlebar.
        /// </summary>
        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set
            {
                SetValue(TitleProperty, value);
                HandleSizeChanges();

                ApplicationView.GetForCurrentView().Title = value;
            }
        }

        /// <summary>
        /// At this width, the TitleBar will begin displaying the title.
        /// </summary>
        public int MinTitleWidth { get; set; } = 0;

        /// <summary>
        /// Whether or not the icon should be visible.
        /// </summary>
        public bool ShowIcon
        {
            get => (bool)GetValue(ShowIconProperty);
            set => SetValue(ShowIconProperty, value);
        }

        /// <summary>
        /// This handles caption control size changes. Some default handling will still be applied.
        /// </summary>
        public event TypedEventHandler<CoreApplicationViewTitleBar, object> LayoutMetricsChanged;

        /// <summary>
        /// The width taken up by the icon and title.
        /// </summary>
        public double LabelWidth => AppData.DesiredSize.Width;
        #endregion

        #region Constructor
        public ExtendedTitleBar()
        {
            InitializeComponent();

#if DEBUG
            DefaultTitleParagraph.Inlines.Add(new Run()
            {
                FontWeight = Windows.UI.Text.FontWeights.SemiBold,
                Text = " [DEBUG]"
            });
#endif

            Loaded += (s, e) =>
            {
                SetupTitleBar();

                HandleSizeChanges();

                SizeChanged += (d, r) => HandleSizeChanges();
            };
        }
        #endregion

        #region TitleBar setup
        public void SetupTitleBar()
        {
            ApplicationViewTitleBar bar = ApplicationView.GetForCurrentView().TitleBar;

            bar.ButtonBackgroundColor = Colors.Transparent;
            bar.ButtonInactiveBackgroundColor = Colors.Transparent;

            // Hide default titlebar
            CoreApplicationViewTitleBar coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
            coreTitleBar.ExtendViewIntoTitleBar = true;

            // Set XAML element as a draggable region
            Window.Current.SetTitleBar(this);

            // Register a handler for when the size of the overlaid caption control changes.
            // For example, when the app moves to a screen with a different DPI
            coreTitleBar.LayoutMetricsChanged += CoreTitleBar_LayoutMetricsChanged;

            // Register a handler for when the title bar visibility changes.
            // For example, when the title bar is invoked in full screen mode
            coreTitleBar.IsVisibleChanged += CoreTitleBar_IsVisibleChanged;

            // Register a handler for when the window changes focus
            Window.Current.Activated += Current_Activated;
        }

        private void CoreTitleBar_LayoutMetricsChanged(CoreApplicationViewTitleBar sender, object args)
        {
            // Update title bar control size as needed to account for system size changes
            Height = sender.Height;

            // Ensure the custom title bar does not overlap window caption controls
            Thickness currMargin = Margin;
            Margin = new Thickness(currMargin.Left, currMargin.Top, sender.SystemOverlayRightInset, currMargin.Bottom);

            // If necessary, run this as well
            LayoutMetricsChanged?.Invoke(sender, args);
        }

        private void CoreTitleBar_IsVisibleChanged(CoreApplicationViewTitleBar sender, object args)
        {
            Visibility = sender.IsVisible ?
                Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// Update the TitleBar based on the inactive/active state of the app.
        /// </summary>
        /// <param name="e">Window activation event args.</param>
        private void Current_Activated(object sender, WindowActivatedEventArgs e)
        {
            SolidColorBrush defaultForegroundBrush =
                (SolidColorBrush)Application.Current.Resources["TextFillColorPrimaryBrush"];

            SolidColorBrush inactiveForegroundBrush =
                (SolidColorBrush)Application.Current.Resources["TextFillColorDisabledBrush"];

            Foreground = e.WindowActivationState == CoreWindowActivationState.Deactivated ?
                inactiveForegroundBrush :
                defaultForegroundBrush;
        }

        private void HandleSizeChanges()
        {
            double width = Window.Current.Bounds.Width;

            Visibility vis = Visibility.Collapsed;
            if (width >= MinTitleWidth)
            {
                vis = Visibility.Visible;
            }

            if (Title != null)
            {
                AppTitle.Visibility = vis;
                DefaultTitle.Visibility = Visibility.Collapsed;
            }
            else
            {
                DefaultTitle.Visibility = vis;
                AppTitle.Visibility = Visibility.Collapsed;
            }
        }
        #endregion
    }
}
