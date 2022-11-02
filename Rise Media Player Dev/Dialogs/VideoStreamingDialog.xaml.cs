using Rise.Common.Helpers;
using Rise.Data.ViewModels;
using System;
using System.Globalization;
using System.Net;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

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
            string url = StreamingTextBox.Text;

            bool isValidVideo = false;
            try
            {
                var req = WebRequest.Create(url);
                req.Method = "HEAD";

                using var resp = req.GetResponse();
                isValidVideo = resp.ContentType.ToLower(CultureInfo.InvariantCulture)
                    .StartsWith("video/", StringComparison.OrdinalIgnoreCase);
            }
            catch { }

            if (!isValidVideo || !Uri.IsWellFormedUriString(url, UriKind.Absolute))
            {
                args.Cancel = true;
                InvalidUrlText.Visibility = Visibility.Visible;
                return;
            }

            // Well formed URL (if it isn't we've already returned anyways)
            Hide();

            await ViewModel.ResetPlaybackAsync();

            var video = WebHelpers.GetVideoFromUri(new(url));
            ViewModel.AddSingleItemToQueue(video);
            ViewModel.Player.Play();
        }
    }
}
