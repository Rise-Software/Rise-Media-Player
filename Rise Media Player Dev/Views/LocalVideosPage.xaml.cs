using Rise.App.ViewModels;
using System;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Rise.App.Views
{
    public sealed partial class LocalVideosPage : Page
    {
        /// <summary>
        /// Gets the app-wide MViewModel instance.
        /// </summary>
        private MainViewModel MViewModel => App.MViewModel;

        /// <summary>
        /// Gets the app-wide VPViewModel instance.
        /// </summary>
        private VideoPlaybackViewModel VPViewModel => App.VPViewModel;
        private int viewId = -1;

        public LocalVideosPage()
        {
            InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;
        }

        private async void GridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (viewId == -1)
            {
                viewId = await typeof(VideoPlaybackPage).
                    OpenInWindowAsync(ApplicationViewMode.Default, 360, 500);
            }
            else
            {
                await ApplicationViewSwitcher.TryShowAsStandaloneAsync(viewId);
            }

            await VPViewModel.PlayVideoAsync(e.ClickedItem as VideoViewModel);
        }
    }
}
