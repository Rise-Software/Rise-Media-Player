using Rise.App.ViewModels;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml.Controls;

namespace Rise.App.Views
{
    public sealed partial class LocalVideosPage : Page
    {
        /// <summary>
        /// Gets the app-wide MViewModel instance.
        /// </summary>
        private MainViewModel MViewModel => App.MViewModel;

        public LocalVideosPage()
        {
            InitializeComponent();
        }

        private async void GridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            await typeof(VideoPlaybackPage).
                OpenInWindowAsync(ApplicationViewMode.Default, 360, 500,
                e.ClickedItem as VideoViewModel);
        }
    }
}
