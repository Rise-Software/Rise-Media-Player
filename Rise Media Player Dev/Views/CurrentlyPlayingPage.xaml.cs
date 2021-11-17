using Rise.App.ViewModels;
using Windows.UI.Xaml.Controls;

namespace Rise.App.Views
{
    public sealed partial class CurrentlyPlayingPage : Page
    {
        /// <summary>
        /// Gets the app-wide NPViewModel instance.
        /// </summary>
        private MusicPlaybackViewModel ViewModel => App.PViewModel;

        public CurrentlyPlayingPage()
        {
            InitializeComponent();
            DataContext = ViewModel;
        }
    }
}
