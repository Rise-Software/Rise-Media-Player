using Rise.App.UserControls;
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

namespace Rise.App.Views
{
    public static class WindowExtensions
    {
        /// <summary>
        /// Opens a page inside a new window with <see cref="ApplicationView"/>.
        /// </summary>
        /// <param name="page">The window frame's initial page.</param>
        /// <param name="viewMode">Default window view mode.</param>
        /// <param name="minWidth">Minimum window width.</param>
        /// <param name="minHeight">Minimum window height.</param>
        /// <param name="openOnCreate">Whether or not to open the window after
        /// creating it.</param>
        /// <param name="parameter">Parameters for the frame.</param>
        /// <returns>The <see cref="ApplicationView.Id"/>.</returns>
        public static async Task<int> PlaceInWindowAsync(this Type page, ApplicationViewMode viewMode,
            int minWidth, int minHeight, bool openOnCreate = true, object parameter = null)
        {
            CoreApplicationView window = CoreApplication.CreateNewView();
            ApplicationView newView = null;
            Size minSize = new(minWidth, minHeight);

            await window.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                Frame frame = new();
                _ = frame.Navigate(page, parameter);

                Window.Current.Content = frame;
                Window.Current.Activate();

                newView = ApplicationView.GetForCurrentView();
                _ = await newView.TryEnterViewModeAsync(viewMode);
                newView.SetPreferredMinSize(minSize);
                _ = new WindowTitleBar(window, newView);
                _ = newView.TryResizeView(minSize);
            });

            if (openOnCreate)
            {
                _ = await ApplicationViewSwitcher.
                    TryShowAsStandaloneAsync(newView.Id);
            }

            return newView.Id;
        }

        /// <summary>
        /// Places a page inside a new window with <see cref="AppWindow"/>.
        /// </summary>
        /// <param name="page">The window frame's initial page.</param>
        /// <param name="viewMode">Default window view mode.</param>
        /// <param name="minWidth">Minimum window width.</param>
        /// <param name="minHeight">Minimum window height.</param>
        /// <param name="openOnCreate">Whether or not to open the window after
        /// creating it.</param>
        /// <param name="parameter">Parameters for the frame.</param>
        /// <returns>The generated <see cref="AppWindow"/>.</returns>
        public static async Task<AppWindow> PlaceInWindowAsync(this Type page, AppWindowPresentationKind viewMode,
            int minWidth, int minHeight, bool openOnCreate = true, object parameter = null)
        {
            AppWindow window = await AppWindow.TryCreateAsync();
            Size minSize = new(minWidth, minHeight);

            Frame frame = new();
            _ = frame.Navigate(page, parameter);

            ElementCompositionPreview.SetAppWindowContent(window, frame);
            _ = new WindowTitleBar(window.TitleBar);
            _ = window.Presenter.RequestPresentation(viewMode);
            WindowManagementPreview.SetPreferredMinSize(window, minSize);
            window.RequestSize(minSize);

            if (openOnCreate)
            {
                await window.TryShowAsync();
            }

            return window;
        }
    }

    /// <summary>
    /// A simple titlebar handler that can be used on an
    /// <see cref="ApplicationView"/>. For use with the
    /// <see cref="MicaTitleBar"/> control.
    /// </summary>
    public class ApplicationTitleBar
    {
        private readonly MicaTitleBar _titleBar;

        public ApplicationTitleBar(MicaTitleBar titleBar,
            TypedEventHandler<CoreApplicationViewTitleBar, object> metricsChangedHandler = null)
        {
            _titleBar = titleBar;
            ApplicationViewTitleBar bar = ApplicationView.GetForCurrentView().TitleBar;

            bar.ButtonBackgroundColor = Colors.Transparent;
            bar.ButtonInactiveBackgroundColor = Colors.Transparent;

            // Hide default title bar.
            CoreApplicationViewTitleBar coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
            coreTitleBar.ExtendViewIntoTitleBar = true;

            // Set XAML element as a draggable region.
            Window.Current.SetTitleBar(_titleBar);

            // Register a handler for when the size of the overlaid caption control changes.
            // For example, when the app moves to a screen with a different DPI.
            if (metricsChangedHandler != null)
            {
                coreTitleBar.LayoutMetricsChanged += metricsChangedHandler;
            }
            else
            {
                coreTitleBar.LayoutMetricsChanged += (s, e) => HandleResize(s);
            }

            // Register a handler for when the title bar visibility changes.
            // For example, when the title bar is invoked in full screen mode.
            coreTitleBar.IsVisibleChanged += CoreTitleBar_IsVisibleChanged;

            //Register a handler for when the window changes focus
            Window.Current.Activated += Current_Activated;
        }

        private void HandleResize(CoreApplicationViewTitleBar coreTitleBar)
        {
            // Update title bar control size as needed to account for system size changes.
            _titleBar.Height = coreTitleBar.Height;

            // Ensure the custom title bar does not overlap window caption controls
            Thickness currMargin = _titleBar.Margin;
            _titleBar.Margin = new Thickness(currMargin.Left, currMargin.Top, coreTitleBar.SystemOverlayRightInset, currMargin.Bottom);
        }

        private void CoreTitleBar_IsVisibleChanged(CoreApplicationViewTitleBar sender, object args)
        {
            _titleBar.Visibility = sender.IsVisible ?
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

            _titleBar.Foreground =
                e.WindowActivationState == CoreWindowActivationState.Deactivated ?
                inactiveForegroundBrush :
                defaultForegroundBrush;
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
