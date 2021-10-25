using RMP.App.UserControls;
using RMP.App.Views;
using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.WindowManagement;
using Windows.UI.WindowManagement.Preview;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Media;

namespace RMP.App.Windows
{
    public class GeneralControl
    {
        /// <summary>
        /// Creates a new window with ApplicationView.
        /// </summary>
        /// <param name="page">The window frame's initial page.</param>
        /// <param name="viewMode">Default window view mode.</param>
        /// <param name="minWidth">Minimum window width.</param>
        /// <param name="minHeight">Minimum window height.</param>
        /// <param name="parameter">Parameters for the frame.</param>
        /// <returns>Whether or not the window opened successfully.</returns>
        public static async Task<bool> CreateWindow(Type page, ApplicationViewMode viewMode,
            int minWidth, int minHeight, object parameter = null)
        {
            CoreApplicationView window = CoreApplication.CreateNewView();
            ApplicationView newView = null;
            Size minSize = new Size(minWidth, minHeight);

            await window.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                Frame frame = new Frame();
                _ = frame.Navigate(page, parameter);

                Window.Current.Content = frame;
                Window.Current.Activate();

                newView = ApplicationView.GetForCurrentView();
                _ = await newView.TryEnterViewModeAsync(viewMode);
                newView.SetPreferredMinSize(minSize);
                _ = new WindowTitleBar(window, newView);
                _ = newView.TryResizeView(minSize);
            });

            return await ApplicationViewSwitcher.TryShowAsStandaloneAsync(newView.Id);
        }

        /// <summary>
        /// Creates a new window with AppWindow.
        /// </summary>
        /// <param name="page">The window frame's initial page.</param>
        /// <param name="viewMode">Default window view mode.</param>
        /// <param name="parameter">Parameters for the frame.</param>
        /// <returns>Whether or not the window opened successfully.</returns>
        public static async Task<bool> CreateWindow(Type page, AppWindowPresentationKind viewMode,
            int minWidth, int minHeight, object parameter = null)
        {
            AppWindow window = await AppWindow.TryCreateAsync();
            Size minSize = new Size(minWidth, minHeight);

            Frame frame = new Frame();
            _ = frame.Navigate(page, parameter);

            ElementCompositionPreview.SetAppWindowContent(window, frame);
            _ = new WindowTitleBar(window.TitleBar);
            _ = window.Presenter.RequestPresentation(viewMode);
            WindowManagementPreview.SetPreferredMinSize(window, minSize);
            window.RequestSize(minSize);

            return await window.TryShowAsync();
        }
    }

    public class MainTitleBar
    {
        private readonly MicaTitleBar AppTitleBar = MainPage.Current.AppTitleBar;
        private readonly Grid ControlsPanel = MainPage.Current.ControlsPanel;
        private readonly Microsoft.UI.Xaml.Controls.NavigationView NavView =
            MainPage.Current.NavView;

        public MainTitleBar()
        {
            ApplicationViewTitleBar titleBar = ApplicationView.GetForCurrentView().TitleBar;

            titleBar.ButtonBackgroundColor = Colors.Transparent;
            titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;

            // Hide default title bar.
            CoreApplicationViewTitleBar coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
            coreTitleBar.ExtendViewIntoTitleBar = true;

            // Set XAML element as a draggable region.
            Window.Current.SetTitleBar(AppTitleBar);

            // Register a handler for when the size of the overlaid caption control changes.
            // For example, when the app moves to a screen with a different DPI.
            coreTitleBar.LayoutMetricsChanged += CoreTitleBar_LayoutMetricsChanged;

            // Register a handler for when the title bar visibility changes.
            // For example, when the title bar is invoked in full screen mode.
            coreTitleBar.IsVisibleChanged += CoreTitleBar_IsVisibleChanged;

            //Register a handler for when the window changes focus
            Window.Current.Activated += Current_Activated;

            UpdateTitleBarLayout(coreTitleBar);
        }

        /// <summary>
        /// Handle TitleBar layout metrics changes.
        /// </summary>
        private void CoreTitleBar_LayoutMetricsChanged(CoreApplicationViewTitleBar sender, object args)
        {
            UpdateTitleBarLayout(sender);
        }

        /// <summary>
        /// Update the TitleBar layout.
        /// </summary>
        private void UpdateTitleBarLayout(CoreApplicationViewTitleBar coreTitleBar)
        {
            // Update title bar control size as needed to account for system size changes.
            AppTitleBar.Height = coreTitleBar.Height;

            // Ensure the custom title bar does not overlap window caption controls
            Thickness currMargin = AppTitleBar.Margin;
            AppTitleBar.Margin = new Thickness(currMargin.Left, currMargin.Top, coreTitleBar.SystemOverlayRightInset, currMargin.Bottom);
            ControlsPanel.Margin = new Thickness(48 + AppTitleBar.LabelWidth + 132, currMargin.Top, 48 + AppTitleBar.LabelWidth + 132, currMargin.Bottom);

            UpdateTitleBarItems(NavView);
        }

        private void CoreTitleBar_IsVisibleChanged(CoreApplicationViewTitleBar sender, object args)
        {
            AppTitleBar.Visibility = sender.IsVisible ?
                Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// Update the TitleBar based on the inactive/active state of the app.
        /// </summary>
        private void Current_Activated(object sender, WindowActivatedEventArgs e)
        {
            SolidColorBrush defaultForegroundBrush = (SolidColorBrush)Application.Current.Resources["TextFillColorPrimaryBrush"];
            SolidColorBrush inactiveForegroundBrush = (SolidColorBrush)Application.Current.Resources["TextFillColorDisabledBrush"];

            AppTitleBar.Foreground = e.WindowActivationState == CoreWindowActivationState.Deactivated
                ? inactiveForegroundBrush
                : defaultForegroundBrush;
        }

        /// <summary>
        /// Update the TitleBar content layout depending on NavigationView DisplayMode.
        /// </summary>
        public void UpdateTitleBarItems(Microsoft.UI.Xaml.Controls.NavigationView NavView)
        {
            const int topIndent = 16;
            const int expandedIndent = 48;
            int minimalIndent = 104;

            // If the back button is not visible, reduce the TitleBar content indent.
            if (NavView.IsBackButtonVisible.Equals(Microsoft.UI.Xaml.Controls.NavigationViewBackButtonVisible.Collapsed))
            {
                minimalIndent = 48;
            }

            Thickness currMargin = AppTitleBar.Margin;

            // Set the TitleBar margin dependent on NavigationView display mode
            if (NavView.PaneDisplayMode == Microsoft.UI.Xaml.Controls.NavigationViewPaneDisplayMode.Top)
            {
                AppTitleBar.Margin = new Thickness(topIndent, currMargin.Top, currMargin.Right, currMargin.Bottom);
                ControlsPanel.Margin = new Thickness(topIndent + AppTitleBar.LabelWidth + 48, currMargin.Top, currMargin.Right, currMargin.Bottom);
            }
            else if (NavView.DisplayMode == Microsoft.UI.Xaml.Controls.NavigationViewDisplayMode.Minimal)
            {
                AppTitleBar.Margin = new Thickness(minimalIndent, currMargin.Top, currMargin.Right, currMargin.Bottom);
                ControlsPanel.Margin = new Thickness(minimalIndent + 36, currMargin.Top, currMargin.Right, currMargin.Bottom);
            }
            else
            {
                AppTitleBar.Margin = new Thickness(expandedIndent, currMargin.Top, currMargin.Right, currMargin.Bottom);
                ControlsPanel.Margin = new Thickness(expandedIndent + AppTitleBar.LabelWidth + 132, currMargin.Top, expandedIndent + AppTitleBar.LabelWidth + 132, currMargin.Bottom);
            }
        }
    }

    public class SetupTitleBar
    {
        private readonly MicaTitleBar AppTitleBar = SetupPage.Current.AppTitleBar;
        public SetupTitleBar()
        {
            ApplicationViewTitleBar titleBar = ApplicationView.GetForCurrentView().TitleBar;

            titleBar.ButtonBackgroundColor = Colors.Transparent;
            titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;

            // Hide default title bar.
            CoreApplicationViewTitleBar coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
            coreTitleBar.ExtendViewIntoTitleBar = true;

            // Set XAML element as a draggable region.
            Window.Current.SetTitleBar(AppTitleBar);

            // Register a handler for when the size of the overlaid caption control changes.
            // For example, when the app moves to a screen with a different DPI.
            coreTitleBar.LayoutMetricsChanged += CoreTitleBar_LayoutMetricsChanged;

            // Register a handler for when the title bar visibility changes.
            // For example, when the title bar is invoked in full screen mode.
            coreTitleBar.IsVisibleChanged += CoreTitleBar_IsVisibleChanged;

            //Register a handler for when the window changes focus
            Window.Current.Activated += Current_Activated;
            UpdateTitleBarLayout(coreTitleBar);
        }

        /// <summary>
        /// Handle TitleBar layout metrics changes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void CoreTitleBar_LayoutMetricsChanged(CoreApplicationViewTitleBar sender, object args)
        {
            UpdateTitleBarLayout(sender);
        }

        /// <summary>
        /// Update the TitleBar layout.
        /// </summary>
        /// <param name="coreTitleBar">Core App TitleBar.</param>
        private void UpdateTitleBarLayout(CoreApplicationViewTitleBar coreTitleBar)
        {
            // Update title bar control size as needed to account for system size changes.
            AppTitleBar.Height = coreTitleBar.Height;

            // Ensure the custom title bar does not overlap window caption controls
            Thickness currMargin = AppTitleBar.Margin;
            AppTitleBar.Margin = new Thickness(currMargin.Left, currMargin.Top, coreTitleBar.SystemOverlayRightInset, currMargin.Bottom);
        }

        private void CoreTitleBar_IsVisibleChanged(CoreApplicationViewTitleBar sender, object args)
        {
            AppTitleBar.Visibility = sender.IsVisible ?
                Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// Update the TitleBar based on the inactive/active state of the app.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Current_Activated(object sender, WindowActivatedEventArgs e)
        {
            SolidColorBrush defaultForegroundBrush = (SolidColorBrush)Application.Current.Resources["TextFillColorPrimaryBrush"];
            SolidColorBrush inactiveForegroundBrush = (SolidColorBrush)Application.Current.Resources["TextFillColorDisabledBrush"];

            AppTitleBar.Foreground = e.WindowActivationState == CoreWindowActivationState.Deactivated
                ? inactiveForegroundBrush
                : defaultForegroundBrush;
        }
    }

    public class WindowTitleBar
    {
        public WindowTitleBar(CoreApplicationView coreView, ApplicationView view)
        {
            CoreApplicationViewTitleBar coreTitleBar = coreView.TitleBar;
            ApplicationViewTitleBar titleBar = view.TitleBar;

            titleBar.ButtonBackgroundColor = Colors.Transparent;
            titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;

            coreTitleBar.ExtendViewIntoTitleBar = true;
        }

        public WindowTitleBar(AppWindowTitleBar titleBar)
        {
            titleBar.ButtonBackgroundColor = Colors.Transparent;
            titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;

            titleBar.ExtendsContentIntoTitleBar = true;
        }
    }
}
