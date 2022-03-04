using Rise.App.Common;
using Rise.App.ViewModels;
using System;
using System.Diagnostics;
using System.IO;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Rise.App.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PlaylistDetailsPage : Page
    {
        private PlaylistViewModel plViewModel;
        private SongViewModel _song;

        private static readonly DependencyProperty SelectedSongProperty =
            DependencyProperty.Register("SelectedSong", typeof(SongViewModel), typeof(PlaylistDetailsPage), null);

        private SongViewModel SelectedSong
        {
            get => (SongViewModel)GetValue(SelectedSongProperty);
            set => SetValue(SelectedSongProperty, value);
        }

        public PlaylistDetailsPage()
        {
            InitializeComponent();
            Loaded += PlaylistDetailsPage_Loaded;
        }

        private async void PlaylistDetailsPage_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                foreach (SongViewModel song in plViewModel.Songs)
                {
                    if (!File.Exists(song.Location))
                    {
                        plViewModel.Songs.Remove(song);
                        await plViewModel.SaveAsync();
                    }
                }
            }
            catch (Exception)
            {

            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            plViewModel = e.Parameter as PlaylistViewModel;
        }

        private void Grid_PointerEntered(object sender, PointerRoutedEventArgs e)
            => EventsLogic.FocusSong(ref _song, e);

        private void Grid_PointerExited(object sender, PointerRoutedEventArgs e)
            => EventsLogic.UnfocusSong(ref _song, e);

        private void MainList_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            if ((e.OriginalSource as FrameworkElement).DataContext is SongViewModel song)
            {
                SelectedSong = song;
                SongFlyout.ShowAt(MainList, e.GetPosition(MainList));
            }
        }

        private async void MainList_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            if ((e.OriginalSource as FrameworkElement).DataContext is SongViewModel song)
            {
                int index = MainList.Items.IndexOf(song);
                await App.PViewModel.StartMusicPlaybackAsync(plViewModel.Songs.GetEnumerator(), index, plViewModel.Songs.Count, false);
            }
        }
        private async void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            if ((e.OriginalSource as FrameworkElement).DataContext is SongViewModel song)
            {
                int index = MainList.Items.IndexOf(song);
                await App.PViewModel.StartMusicPlaybackAsync(plViewModel.Songs.GetEnumerator(), index, plViewModel.Songs.Count, false);
            }
            else
            {
                await App.PViewModel.StartMusicPlaybackAsync(plViewModel.Songs.GetEnumerator(), 0, plViewModel.Songs.Count, false);
            }
        }

        private void Album_Click(Hyperlink sender, HyperlinkClickEventArgs args)
            => EventsLogic.GoToAlbum(sender);

        private void Artist_Click(Hyperlink sender, HyperlinkClickEventArgs args)
            => EventsLogic.GoToArtist(sender);

        private async void Remove_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine(plViewModel.Songs.Remove(SelectedSong));
        }

        private async void PlaylistProperties_Click(object sender, RoutedEventArgs e)
        {
            await plViewModel.StartEditAsync();
        }

        private async void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            await plViewModel.DeleteAsync();
            this.Frame.GoBack();
        }
    }
}
