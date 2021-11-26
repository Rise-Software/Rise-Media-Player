using Rise.App.ViewModels;
using System;
using System.Numerics;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media.Animation;

namespace Rise.App.Views
{
    public sealed partial class NowPlaying : Page
    {
        /// <summary>
        /// Gets the app-wide NPViewModel instance.
        /// </summary>
        private PlaybackViewModel ViewModel => App.PViewModel;

        private bool IsInCurrentlyPlayingPage = true;

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
            IsInCurrentlyPlayingPage = false;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            _ = PlayFrame.Navigate(typeof(CurrentlyPlayingPage));
            BackButton.Visibility = Visibility.Collapsed;

            Queue.Visibility = Visibility.Collapsed;
            AlbumQueue.Visibility = Visibility.Collapsed;
            QueueButton.Visibility = Visibility.Visible;
            IsInCurrentlyPlayingPage = true;
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

        private void Page_PointerEntered(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            if (IsInCurrentlyPlayingPage)
            {
                PlayFrameHoverAnimationIn.Begin();
                BlurBrushBorderAnimationIn.Begin();
                PlayerElementHoverAnimationIn.Begin();
                QueueButtonHoverAnimationIn.Begin();
                PlayFrame.Visibility = Visibility.Visible;
                Player.Visibility = Visibility.Visible;
                ImageBrushAlbumCover.Opacity = 0.25;
                QueueButton.Visibility = Visibility.Visible;
                BlurBrush.Amount = 10;
            }
        }

        private void Page_PointerExited(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            if (IsInCurrentlyPlayingPage)
            {
                PlayFrameHoverAnimationOut.Begin();
                BlurBrushBorderAnimationOut.Begin();
                PlayerElementHoverAnimationOut.Begin();
                QueueButtonHoverAnimationOut.Begin();
                PlayFrame.Visibility = Visibility.Collapsed;
                Player.Visibility = Visibility.Collapsed;
                ImageBrushAlbumCover.Opacity = 1;
                QueueButton.Visibility = Visibility.Collapsed;
                BlurBrush.Amount = 0;
            }
        }
    }
}
