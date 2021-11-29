using Rise.App.Common;
using Rise.App.ViewModels;
using System;
using Windows.UI.Xaml;
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
        /// Gets the app-wide PViewModel instance.
        /// </summary>
        private PlaybackViewModel PViewModel => App.PViewModel;

        /// <summary>
        /// Gets the <see cref="NavigationHelper"/> associated with this <see cref="Page"/>.
        /// </summary>
        private readonly NavigationHelper _navigationHelper;

        public LocalVideosPage()
        {
            InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;

            _navigationHelper = new NavigationHelper(this);
        }

        private async void GridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            await PViewModel.PlayVideoAsync(e.ClickedItem as VideoViewModel);
            if (Window.Current.Content is Frame rootFrame)
            {
                rootFrame.Navigate(typeof(VideoPlaybackPage));
            }
        }
    }
}
