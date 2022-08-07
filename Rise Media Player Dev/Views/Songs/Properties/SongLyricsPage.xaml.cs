using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Controls;
using System.Collections.Generic;
using System.Linq;
using Rise.Models;
using Rise.App.Helpers;
using Rise.App.ViewModels;

namespace Rise.App.Views
{
    public sealed partial class SongLyricsPage : Page
    {
        private MusixmatchLyrics lyrics;

        private SongPropertiesViewModel Props { get; set; }

        private List<string> lyricsText;

        public SongLyricsPage()
        {
            InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;

            Loaded += OnLoaded;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is SongPropertiesViewModel props)
            {
                Props = props;
            }

            base.OnNavigatedTo(e);
        }

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            try
            {
                lyrics = await MusixmatchHelper.GetLyricsAsync(Props.Title, Props.Artist);

                if (string.IsNullOrEmpty(lyrics.Message.Body.Lyrics.LyricsBody))
                    return;

                System.Diagnostics.Debug.WriteLine("\nCannot find lyrics\n");

                lyricsText = lyrics.Message.Body.Lyrics.LyricsBody.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();

                LyricsListView.ItemsSource = lyricsText;
            } catch
            {

            }
        }
    }
}
