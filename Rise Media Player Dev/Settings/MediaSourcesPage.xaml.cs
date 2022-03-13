using Rise.Common;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Rise.App.Settings
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MediaSourcesPage : Page
    {
        private readonly NavigationHelper _navigationHelper;

        public MediaSourcesPage()
        {
            this.InitializeComponent();

            _navigationHelper = new NavigationHelper(this);
            //var titleBar = ApplicationView.GetForCurrentView().TitleBar;

            //titleBar.ButtonBackgroundColor = Colors.Transparent;
            //titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;

            //// Hide default title bar.
            //var coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
            //coreTitleBar.ExtendViewIntoTitleBar = true;
            //UpdateTitleBarLayout(coreTitleBar);

            //// Set XAML element as a draggable region.
            //Window.Current.SetTitleBar(AppTitleBar);

            //// Register a handler for when the size of the overlaid caption control changes.
            //// For example, when the app moves to a screen with a different DPI.
            //coreTitleBar.LayoutMetricsChanged += CoreTitleBar_LayoutMetricsChanged;

            //// Register a handler for when the title bar visibility changes.
            //// For example, when the title bar is invoked in full screen mode.
            //coreTitleBar.IsVisibleChanged += CoreTitleBar_IsVisibleChanged;

            ////Register a handler for when the window changes focus
            //Window.Current.Activated += Current_Activated;
        }

        //private void CoreTitleBar_LayoutMetricsChanged(CoreApplicationViewTitleBar sender, object args)
        //{
        //    UpdateTitleBarLayout(sender);
        //}

        //private void UpdateTitleBarLayout(CoreApplicationViewTitleBar coreTitleBar)
        //{
        //    // Update title bar control size as needed to account for system size changes.
        //    AppTitleBar.Height = coreTitleBar.Height;

        //    // Ensure the custom title bar does not overlap window caption controls
        //    Thickness currMargin = AppTitleBar.Margin;
        //    AppTitleBar.Margin = new Thickness(currMargin.Left, currMargin.Top, coreTitleBar.SystemOverlayRightInset, currMargin.Bottom);
        //}

        //private void CoreTitleBar_IsVisibleChanged(CoreApplicationViewTitleBar sender, object args)
        //{
        //    if (sender.IsVisible)
        //    {
        //        AppTitleBar.Visibility = Visibility.Visible;
        //    }
        //    else
        //    {
        //        AppTitleBar.Visibility = Visibility.Collapsed;
        //    }
        //}

        //// Update the TitleBar based on the inactive/active state of the app
        //private void Current_Activated(object sender, Windows.UI.Core.WindowActivatedEventArgs e)
        //{
        //    SolidColorBrush defaultForegroundBrush = (SolidColorBrush)Application.Current.Resources["TextFillColorPrimaryBrush"];
        //    SolidColorBrush inactiveForegroundBrush = (SolidColorBrush)Application.Current.Resources["TextFillColorDisabledBrush"];

        //    if (e.WindowActivationState == Windows.UI.Core.CoreWindowActivationState.Deactivated)
        //    {
        //        AppTitle.Foreground = inactiveForegroundBrush;
        //    }
        //    else
        //    {
        //        AppTitle.Foreground = defaultForegroundBrush;
        //    }
        //}

        private void Selection_SelectionChanged(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewSelectionChangedEventArgs args)
        {
            var selectedItem = (Microsoft.UI.Xaml.Controls.NavigationViewItem)args.SelectedItem;
            string selectedItemTag = ((string)selectedItem.Tag);

            if (selectedItemTag == "AllMedia")
            {
                SelectVidorMusic.Navigate(typeof(Dialogs.VFoldersDialog));
                Dialogs.VFoldersDialog.Current.MusicList.Visibility = Visibility.Visible;
                Dialogs.VFoldersDialog.Current.VideosList.Visibility = Visibility.Visible;
                Dialogs.VFoldersDialog.Current.VideoButtons.Visibility = Visibility.Collapsed;
                Dialogs.VFoldersDialog.Current.MusicButtons.Visibility = Visibility.Collapsed;
                Dialogs.VFoldersDialog.Current.AllButtons.Visibility = Visibility.Visible;
            }
            else if (selectedItemTag == "Music")
            {
                SelectVidorMusic.Navigate(typeof(Dialogs.VFoldersDialog));
                Dialogs.VFoldersDialog.Current.MusicList.Visibility = Visibility.Visible;
                Dialogs.VFoldersDialog.Current.VideosList.Visibility = Visibility.Collapsed;
                Dialogs.VFoldersDialog.Current.VideoButtons.Visibility = Visibility.Collapsed;
                Dialogs.VFoldersDialog.Current.AllButtons.Visibility = Visibility.Collapsed;
                Dialogs.VFoldersDialog.Current.MusicButtons.Visibility = Visibility.Visible;
            }
            else if (selectedItemTag == "Videos")
            {
                SelectVidorMusic.Navigate(typeof(Dialogs.VFoldersDialog));
                Dialogs.VFoldersDialog.Current.MusicList.Visibility = Visibility.Collapsed;
                Dialogs.VFoldersDialog.Current.VideosList.Visibility = Visibility.Visible;
                Dialogs.VFoldersDialog.Current.MusicButtons.Visibility = Visibility.Collapsed;
                Dialogs.VFoldersDialog.Current.AllButtons.Visibility = Visibility.Collapsed;
                Dialogs.VFoldersDialog.Current.VideoButtons.Visibility = Visibility.Visible;
            }
            else
            {

            }
        }
    }
}
