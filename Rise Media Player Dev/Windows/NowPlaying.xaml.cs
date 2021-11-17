using Rise.App.ViewModels;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace Rise.App.Views
{
    public sealed partial class NowPlaying : Page
    {
        /// <summary>
        /// Gets the app-wide NPViewModel instance.
        /// </summary>
        private MusicPlaybackViewModel ViewModel => App.PViewModel;

        public NowPlaying()
        {
            InitializeComponent();
            Player.SetMediaPlayer(ViewModel.Player);

            ApplicationView.GetForCurrentView().TitleBar.ButtonBackgroundColor = Colors.Transparent;
            ApplicationView.GetForCurrentView().TitleBar.ButtonInactiveBackgroundColor = Colors.Transparent;

            DataContext = ViewModel;
            _ = PlayFrame.Navigate(typeof(CurrentlyPlayingPage));
        }

        private void QueueButton_Click(object sender, RoutedEventArgs e)
        {
            _ = PlayFrame.Navigate(typeof(QueuePage));
            BackButton.Visibility = Visibility.Visible;

            Queue.Visibility = Visibility.Visible;
            AlbumQueue.Visibility = Visibility.Visible;
            QueueButton.Visibility = Visibility.Collapsed;
            Queue.IsChecked = true;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            _ = PlayFrame.Navigate(typeof(CurrentlyPlayingPage));
            BackButton.Visibility = Visibility.Collapsed;

            Queue.Visibility = Visibility.Collapsed;
            AlbumQueue.Visibility = Visibility.Collapsed;
            QueueButton.Visibility = Visibility.Visible;
        }

        private void ToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            Queue.IsChecked = false;
            AlbumQueue.IsChecked = false;

            ToggleButton clicked = (ToggleButton)sender;
            clicked.Checked -= ToggleButton_Checked;
            clicked.IsChecked = true;

            switch (clicked.Tag.ToString())
            {
                case "QueueItem":
                    _ = PlayFrame.Navigate(typeof(QueuePage));
                    break;

                default:
                    break;
            }

            clicked.Checked += ToggleButton_Checked;
        }
    }
}
