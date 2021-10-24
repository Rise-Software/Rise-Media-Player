using RMP.App.ViewModels;
using Windows.UI.Xaml.Controls;

namespace RMP.App.Views
{
    public sealed partial class CurrentlyPlayingPage : Page
    {
        /// <summary>
        /// Gets the app-wide NPViewModel instance.
        /// </summary>
        private PlaybackViewModel ViewModel => App.PViewModel;

        public CurrentlyPlayingPage()
        {
            InitializeComponent();
            DataContext = ViewModel;
        }
    }
}
