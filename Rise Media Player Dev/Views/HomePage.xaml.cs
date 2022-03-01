using Rise.App.Common;
using Rise.App.Dialogs;
using Rise.App.UserControls;
using Rise.App.Web;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using static Rise.App.Common.Enums;

namespace Rise.App.Views
{
    public sealed partial class HomePage : Page
    {
        private static readonly FeatureDialog _dialog = new();

        /// <summary>
        /// Gets the <see cref="NavigationHelper"/> associated with this <see cref="Page"/>.
        /// </summary>
        private readonly NavigationHelper _navigationHelper;

        public HomePage()
        {
            InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;

            _navigationHelper = new NavigationHelper(this);
        }

        private async void ContributeButton_Click(object sender, RoutedEventArgs e)
            => _ = await URLs.GitHub.LaunchAsync();

        private async void SupportButton_Click(object sender, RoutedEventArgs e)
            => _ = await typeof(SupportProject).PlaceInWindowAsync(Windows.UI.ViewManagement.ApplicationViewMode.Default, 500, 600, true);

        private async void FoldersButton_Click(object sender, RoutedEventArgs e)
        {
            ContentDialog dialog = new ContentDialog();
            dialog.Title = "Manage local media folders";
            dialog.CloseButtonText = "Close";
            dialog.Content = new Settings.MediaSourcesPage();
            var result = await dialog.ShowAsync();

        }

        #region NavigationHelper registration
        /// <summary>
        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// Page specific logic should be placed in event handlers for the  
        /// <see cref="NavigationHelper.LoadState"/>
        /// and <see cref="NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method 
        /// in addition to page state preserved during an earlier session.
        /// </summary>
        protected override void OnNavigatedTo(NavigationEventArgs e)
            => _navigationHelper.OnNavigatedTo(e);

        protected override void OnNavigatedFrom(NavigationEventArgs e)
            => _navigationHelper.OnNavigatedFrom(e);
        #endregion

        private async void GridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as TiledImage;
            await _dialog.OpenFeatureAsync(int.Parse(item.Tag.ToString()));
        }

        //private void CommandBarButton_Click(object sender, RoutedEventArgs e)
        //{
        //    Button button = sender as Button;
        //    switch (button.Tag.ToString())
        //    {

        //        case "Version":
        //            vTip.IsOpen = true;
        //            break;
        //        case "Discy":
        //            DiscyOnHome.IsOpen = true;
        //            break;
        //        default:
        //            break;
        //    }
        //}

        private void VTip_CloseButtonClick(Microsoft.UI.Xaml.Controls.TeachingTip sender, object args)
        {

        }

        private async void VTip_ActionButtonClick(Microsoft.UI.Xaml.Controls.TeachingTip sender, object args)
            => await URLs.Releases.LaunchAsync();

        private async void GlanceManage_Click(object sender, RoutedEventArgs e)
        {
            ContentDialog dialog = new()
            {
                CloseButtonText = "Close",
                DefaultButton = ContentDialogButton.Primary,
                Content = new WidgetsDialogContent()
            };

            var result = await dialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                await "https://www.github.com/rise-software/rise-media-player".LaunchAsync();
            }
        }

        private async void WhatsNew_Click(object sender, RoutedEventArgs e)
            => _ = await typeof(WhatsNew).PlaceInWindowAsync(Windows.UI.ViewManagement.ApplicationViewMode.Default, 500, 600, true);
    }
}