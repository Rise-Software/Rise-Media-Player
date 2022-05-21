using System;
using System.Globalization;
using System.Net;
using Rise.App.ViewModels;
using Rise.Common.Constants;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Rise.App.Dialogs
{
    public sealed partial class VideoStreamingDialog : ContentDialog
    {
        public VideoStreamingDialog()
        {
            InitializeComponent();
        }

        private async void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            bool isValidVideo;

            try
            {
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(StreamingTextBox.Text);
                req.Method = "HEAD";
                using var resp = req.GetResponse();
                isValidVideo = resp.ContentType.ToLower(CultureInfo.InvariantCulture)
                           .StartsWith("video/", StringComparison.OrdinalIgnoreCase);
            }
            catch (Exception)
            {
                isValidVideo = false;
            }

            if (!(Uri.IsWellFormedUriString(StreamingTextBox.Text, UriKind.Absolute) && isValidVideo))
            {
                // Not a well formed URL, show error and don't continue.
                if (InvalidUrlText.Visibility == Visibility.Collapsed)
                {
                    InvalidUrlText.Visibility = Visibility.Visible;
                }
                return;
            }

            // Well formed URL (if it isn't then we already stopped calling this function at this point)
            // TODO: create a song view model based on the information found in the file and play it
            if (InvalidUrlText.Visibility == Visibility.Visible)
            {
                InvalidUrlText.Visibility = Visibility.Collapsed;
            }

            Hide();

            VideoViewModel video = new()
            {
                Title = "Online Video",
                Location = StreamingTextBox.Text,
                Thumbnail = URIs.AlbumThumb,
                Directors = "None"
            };

            await App.MPViewModel.PlaySingleItemAsync(video);
        }
    }
}
