using Rise.App.ViewModels;
using System;
using Windows.Media.Playback;
using Windows.Storage;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Rise.App.Views
{
    public sealed partial class VideoPlaybackPage : Page
    {
        private MediaPlayer Player { get; set; }

        public VideoPlaybackPage()
        {
            InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is VideoViewModel video)
            {
                Player = new MediaPlayer();
                PlayerElement.SetMediaPlayer(Player);

                StorageFile media = await StorageFile.GetFileFromPathAsync(video.Location);
                Player.SetFileSource(media);

                Player.Play();
            }

            base.OnNavigatedTo(e);
        }
    }
}
