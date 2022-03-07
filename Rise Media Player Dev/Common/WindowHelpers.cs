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
