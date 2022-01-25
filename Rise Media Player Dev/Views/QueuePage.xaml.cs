using Microsoft.Toolkit.Uwp.UI;
using Rise.App.ViewModels;
using System.Diagnostics;
using Windows.Media;
using Windows.Media.Playback;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

namespace Rise.App.Views
{
    public sealed partial class QueuePage : Page
    {
        /// <summary>
        /// Gets the app-wide NPViewModel instance.
        /// </summary>
        private static PlaybackViewModel ViewModel => App.PViewModel;
        private readonly AdvancedCollectionView Songs = new(ViewModel.PlayingSongs);

        public QueuePage()
        {
            InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;

            Loaded += (s, e) =>
            {
                Queue.Checked += ToggleButton_Checked;
                AlbumQueue.Checked += ToggleButton_Checked;
            };
        }

        private void PlaybackList_CurrentItemChanged(MediaPlaybackList sender, CurrentMediaPlaybackItemChangedEventArgs args)
        {
            if (args.Reason == MediaPlaybackItemChangedReason.Error)
            {
                Debug.WriteLine("Error when going to next item.");
                return;
            }

            if (sender.CurrentItem != null)
            {
                if (sender.CurrentItem.GetDisplayProperties().Type == MediaPlaybackType.Music)
                {
                    using (Songs.DeferRefresh())
                    {
                        var props = sender.CurrentItem.GetDisplayProperties();
                        Songs.Filter = s =>
                            ((SongViewModel)s).Album == props.MusicProperties.AlbumTitle;
                    }

                    Songs.Refresh();
                }
            }

        }

        private void MainList_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            int itemIndex = ViewModel.PlayingSongs.
                IndexOf(MainList.SelectedItem as SongViewModel);

            if (itemIndex < 0)
            {
                return;
            }

            ViewModel.PlaybackList.MoveTo((uint)itemIndex);
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
            => Frame.GoBack();

        private void ToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            var btn = sender as ToggleButton;
            switch (btn.Tag.ToString())
            {
                case "QueueItem":
                    AlbumQueue.IsChecked = false;

                    using (Songs.DeferRefresh())
                    {
                        Songs.Filter = null;
                    }

                    Songs.Refresh();
                    ViewModel.PlaybackList.CurrentItemChanged -= PlaybackList_CurrentItemChanged;
                    break;

                default:
                    Queue.IsChecked = false;
                    if (ViewModel.PlaybackList.CurrentItem != null)
                    {
                        if (ViewModel.PlaybackList.CurrentItem.GetDisplayProperties().Type == MediaPlaybackType.Music)
                        {
                            var props = ViewModel.PlaybackList.CurrentItem.GetDisplayProperties();
                            Songs.Filter = s =>
                                ((SongViewModel)s).Album == props.MusicProperties.AlbumTitle;
                        }
                    }

                    ViewModel.PlaybackList.CurrentItemChanged += PlaybackList_CurrentItemChanged;
                    break;
            }
        }
    }
}
