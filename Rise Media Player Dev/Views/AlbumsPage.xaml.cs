using RMP.App.Settings.ViewModels;
using RMP.App.ViewModels;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace RMP.App.Views
{
    public sealed partial class AlbumsPage : Page
    {
        public static AlbumsPage Current;

        /// <summary>
        /// Gets the app-wide MViewModel instance.
        /// </summary>
        private MainViewModel MViewModel => App.MViewModel;

        /// <summary>
        /// Gets the app-wide SViewModel instance.
        /// </summary>
        private SettingsViewModel SViewModel => App.SViewModel;

        public AlbumsPage()
        {
            InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;

            Current = this;
        }
    }
}
