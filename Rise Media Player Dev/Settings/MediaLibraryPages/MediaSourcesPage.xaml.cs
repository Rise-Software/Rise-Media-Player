using Rise.Common.Helpers;
using Windows.UI.Xaml.Controls;

namespace Rise.App.Settings
{
    public sealed partial class MediaSourcesPage : Page
    {
        private readonly NavigationHelper _navigationHelper;

        public MediaSourcesPage()
        {
            this.InitializeComponent();
            this._navigationHelper = new NavigationHelper(this);

            this.ContentFrame.Navigate(typeof(MediaSourcesListsPage), "AllMedia");
        }

        private void NavigationView_ItemInvoked(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewItemInvokedEventArgs args)
        {
            this.ContentFrame.Navigate(typeof(MediaSourcesListsPage),
                args.InvokedItem.ToString(),
                args.RecommendedNavigationTransitionInfo);
        }
    }
}
