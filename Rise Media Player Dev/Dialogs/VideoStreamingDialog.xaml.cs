using Rise.Common.Helpers;
using Rise.Data.ViewModels;
using System;
using System.Globalization;
using System.Linq;
using System.Net;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using YoutubeExplode;
using YoutubeExplode.Common;
using YoutubeExplode.Videos.Streams;

namespace Rise.App.Dialogs
{
    public sealed partial class VideoStreamingDialog : ContentDialog
    {
        private MediaPlaybackViewModel ViewModel => App.MPViewModel;

        public VideoStreamingDialog()
        {
            InitializeComponent();
        }

        private async void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            var url = StreamingTextBox.Text;

            string title = null, subtitle = null, thumbnailUrl = null;

            if (!Uri.TryCreate(url, UriKind.RelativeOrAbsolute, out Uri uri))
            {
                args.Cancel = true;
                InvalidUrlText.Visibility = Visibility.Visible;
                return;
            }

            if (url.Contains("youtube.com/watch"))
            {
                var youtubeClient = new YoutubeClient();
                var youtubeVideo = await youtubeClient.Videos.GetAsync(url.Replace("music.youtube.com", "www.youtube.com"));

                title = youtubeVideo.Title;
                subtitle = youtubeVideo.Author.ChannelTitle;
                thumbnailUrl = youtubeVideo.Thumbnails.GetWithHighestResolution().Url;

                var streams = await youtubeClient.Videos.Streams.GetManifestAsync(url);

                uri = new(streams.GetMuxedStreams().GetWithHighestVideoQuality().Url);
            }

            Hide();

            await ViewModel.ResetPlaybackAsync();

            var video = WebHelpers.GetVideoFromUri(uri, title, subtitle, thumbnailUrl);
            ViewModel.AddSingleItemToQueue(video);
            ViewModel.Player.Play();
        }
    }
}
