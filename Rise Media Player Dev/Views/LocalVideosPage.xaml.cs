using Rise.App.ViewModels;
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
    }
}
