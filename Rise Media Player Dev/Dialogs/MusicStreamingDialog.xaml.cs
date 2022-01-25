using Rise.App.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Rise.App.Dialogs
{
    public sealed partial class MusicStreamingDialog : ContentDialog
    {
        public MusicStreamingDialog()
        {
            InitializeComponent();
        }

        private async void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            if (!Uri.IsWellFormedUriString(StreamingTextBox.Text, UriKind.Absolute))
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

            SongViewModel song = new()
            {
                Title = "title",
                Track = 0,
                Disc = 0,
                Album = "UnknownAlbumResource",
                Artist = "UnknownArtistResource",
                AlbumArtist = "UnknownArtistResource",
                Location = StreamingTextBox.Text
            };

            await App.PViewModel.PlaySongFromUrlAsync(song);
        }
    }
}
