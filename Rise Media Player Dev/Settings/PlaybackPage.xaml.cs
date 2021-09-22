using System.Collections.Generic;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace RMP.App.Settings
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PlaybackPage : Page
    {
        private List<string> Crossfade { get; set; }
        private List<string> VideoScale { get; set; }

        public PlaybackPage()
        {
            this.InitializeComponent();

            Crossfade = new List<string>
            {
                ResourceLoaders.PlaybackLoader.GetString("Duration0"),
                ResourceLoaders.PlaybackLoader.GetString("Duration3s"),
                ResourceLoaders.PlaybackLoader.GetString("Duration5s"),
                ResourceLoaders.PlaybackLoader.GetString("Duration10s")
            };

            VideoScale = new List<string>
            {
                ResourceLoaders.PlaybackLoader.GetString("WindowSize"),
                ResourceLoaders.PlaybackLoader.GetString("MatchRes")
            };
        }
    }
}
