using Rise.Common.Helpers;
using Rise.Data.ViewModels;
using System;
using System.Globalization;
using System.Net;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using YoutubeExplode;
using YoutubeExplode.Common;
using YoutubeExplode.Videos.Streams;

namespace Rise.App.Dialogs
{
    public sealed partial class MusicStreamingDialog : ContentDialog
    {
        private MediaPlaybackViewModel ViewModel => App.MPViewModel;

        public MusicStreamingDialog()
        {
            InitializeComponent();
        }

        private async void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            var deferral = args.GetDeferral();

            var url = StreamingTextBox.Text;

            string title = null, subtitle = null, thumbnailUrl = null;

            if (!Uri.TryCreate(StreamingTextBox.Text, UriKind.RelativeOrAbsolute, out Uri uri))
            {
                args.Cancel = true;
                InvalidUrlText.Visibility = Visibility.Visible;
                return;
            }

            bool isYoutubeLink = url.Contains("youtube.com/watch");

            if (isYoutubeLink)
            {
                var youtubeClient = new YoutubeClient();
                var youtubeSong = await youtubeClient.Videos.GetAsync(url.Replace("music.youtube.com", "www.youtube.com"));

                title = youtubeSong.Title;
                subtitle = youtubeSong.Author.ChannelTitle;
                thumbnailUrl = youtubeSong.Thumbnails.GetWithHighestResolution().Url;

                var streams = await youtubeClient.Videos.Streams.GetManifestAsync(url);

                uri = new(streams.GetAudioStreams().GetWithHighestBitrate().Url);
            }

            Hide();

            await ViewModel.ResetPlaybackAsync();

            var song = await WebHelpers.GetSongFromUriAsync(uri, title, subtitle, thumbnailUrl, !isYoutubeLink);
            ViewModel.AddSingleItemToQueue(song);
            ViewModel.Player.Play();

            deferral?.Complete();
        }
    }
}
