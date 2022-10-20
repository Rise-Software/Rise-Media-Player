using Rise.Common.Helpers;
using Rise.Data.ViewModels;
using System;
using System.Globalization;
using System.Net;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

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
            string url = StreamingTextBox.Text;

            bool isValidSong = false;
            try
            {
                var req = WebRequest.Create(url);
                req.Method = "HEAD";

                using var resp = req.GetResponse();
                isValidSong = resp.ContentType.ToLower(CultureInfo.InvariantCulture)
                    .StartsWith("audio/", StringComparison.OrdinalIgnoreCase);
            }
            catch { }

            if (!isValidSong || !Uri.IsWellFormedUriString(url, UriKind.Absolute))
            {
                args.Cancel = true;
                InvalidUrlText.Visibility = Visibility.Visible;
                return;
            }

            // Well formed URL (if it isn't we've already returned anyways)
            Hide();

            await ViewModel.ResetPlaybackAsync();

            var song = WebHelpers.GetSongFromUri(new(url));
            ViewModel.AddSingleItemToQueue(song);
            ViewModel.Player.Play();
        }
    }
}
