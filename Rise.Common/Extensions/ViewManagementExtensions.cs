using Rise.Common.Constants;
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

namespace Rise.Common.Extensions
{
    public static class ViewManagementExtensions
    {
        /// <summary>
        /// Opens a type inside a new window with <see cref="ApplicationView"/>.
        /// </summary>
        /// <param name="type">The window frame's initial ype.</param>
        /// <param name="parameter">Parameters for the frame.</param>
        /// <param name="minWidth">Minimum window width.</param>
        /// <param name="minHeight">Minimum window height.</param>
        /// <param name="resizeOnCreate">Whether or not to resize the window
        /// to its minimum dimensions after creating it.</param>
        /// <param name="viewMode">Default window view mode.</param>
        /// <returns>Whether or not the window was successfully shown.</returns>
        public static async Task<bool> ShowInApplicationViewAsync(this Type type,
            object parameter = null,
            int minWidth = ViewManagement.DefaultMinWindowWidth,
            int minHeight = ViewManagement.DefaultMinWindowHeight,
            bool resizeOnCreate = true,
            ApplicationViewMode viewMode = ApplicationViewMode.Default)
        {
            int id = await type.PlaceInApplicationViewAsync(parameter,
                minWidth,
                minHeight,
                resizeOnCreate,
                viewMode);

            return await ApplicationViewSwitcher.
                TryShowAsStandaloneAsync(id);
        }

        /// <summary>
        /// Opens a type inside a new window with <see cref="AppWindow"/>.
        /// </summary>
        /// <param name="type">The window frame's initial type.</param>
        /// <param name="parameter">Parameters for the frame.</param>
        /// <param name="minWidth">Minimum window width.</param>
        /// <param name="minHeight">Minimum window height.</param>
        /// <param name="resizeOnCreate">Whether or not to resize the window
        /// to its minimum dimensions after creating it.</param>
        /// <param name="viewMode">Default window view mode.</param>
        /// <returns>Whether or not the window was successfully shown.</returns>
        public static async Task<bool> ShowInAppWindowAsync(this Type type,
            object parameter = null,
            int minWidth = ViewManagement.DefaultMinWindowWidth,
            int minHeight = ViewManagement.DefaultMinWindowHeight,
            bool resizeOnCreate = true,
            AppWindowPresentationKind viewMode = AppWindowPresentationKind.Default)
        {
            var window = await type.PlaceInAppWindowAsync(parameter,
                minWidth,
                minHeight,
                resizeOnCreate,
                viewMode);

            return await window.TryShowAsync();
        }

        /// <summary>
        /// Places a type inside a new <see cref="ApplicationView"/>.
        /// </summary>
        /// <param name="type">The window frame's initial type.</param>
        /// <param name="parameter">Parameters for the frame.</param>
        /// <param name="minWidth">Minimum window width.</param>
        /// <param name="minHeight">Minimum window height.</param>
        /// <param name="resizeOnCreate">Whether or not to resize the window
        /// to its minimum dimensions after creating it.</param>
        /// <param name="viewMode">Default window view mode.</param>
        /// <returns>The generated <see cref="ApplicationView"/>.</returns>
        public static async Task<int> PlaceInApplicationViewAsync(this Type type,
            object parameter = null,
            int minWidth = ViewManagement.DefaultMinWindowWidth,
            int minHeight = ViewManagement.DefaultMinWindowHeight,
            bool resizeOnCreate = false,
            ApplicationViewMode viewMode = ApplicationViewMode.Default)
        {
            var window = CoreApplication.CreateNewView();
            ApplicationView newView = null;

            Size minSize = new(minWidth, minHeight);

            await window.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                Frame frame = new();
                _ = frame.Navigate(type, parameter);

                Window.Current.Content = frame;
                Window.Current.Activate();

                newView = ApplicationView.GetForCurrentView();
                _ = await newView.TryEnterViewModeAsync(viewMode);
                newView.SetPreferredMinSize(minSize);

                if (resizeOnCreate)
                {
                    _ = newView.TryResizeView(minSize);
                }
            });

            return newView.Id;
        }

        /// <summary>
        /// Places a type inside a new <see cref="AppWindow"/>.
        /// </summary>
        /// <param name="type">The window frame's initial type.</param>
        /// <param name="parameter">Parameters for the frame.</param>
        /// <param name="minWidth">Minimum window width.</param>
        /// <param name="minHeight">Minimum window height.</param>
        /// <param name="resizeOnCreate">Whether or not to resize the window
        /// to its minimum dimensions after creating it.</param>
        /// <param name="viewMode">Default window view mode.</param>
        /// <returns>The generated <see cref="AppWindow"/>.</returns>
        public static async Task<AppWindow> PlaceInAppWindowAsync(this Type type,
            object parameter = null,
            int minWidth = ViewManagement.DefaultMinWindowWidth,
            int minHeight = ViewManagement.DefaultMinWindowHeight,
            bool resizeOnCreate = false,
            AppWindowPresentationKind viewMode = AppWindowPresentationKind.Default)
        {
            var window = await AppWindow.TryCreateAsync();
            Size minSize = new(minWidth, minHeight);

            Frame frame = new();
            _ = frame.Navigate(type, parameter);

            ElementCompositionPreview.SetAppWindowContent(window, frame);

            var titleBar = window.TitleBar;
            titleBar.ButtonBackgroundColor = Colors.Transparent;
            titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
            titleBar.ExtendsContentIntoTitleBar = true;

            _ = window.Presenter.RequestPresentation(viewMode);
            WindowManagementPreview.SetPreferredMinSize(window, minSize);

            if (resizeOnCreate)
            {
                window.RequestSize(minSize);
            }

            return window;
        }
    }
}
