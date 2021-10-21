using RMP.App.Settings.ViewModels;
using System.Collections.Generic;
using Windows.UI.Xaml.Controls;

namespace RMP.App.Settings
{
    public sealed partial class PlaybackPage : Page
    {
        private SettingsViewModel ViewModel => App.SViewModel;
        private List<string> Crossfade { get; set; }
        private List<string> VideoScale { get; set; }

        public PlaybackPage()
        {
            InitializeComponent();

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

            DataContext = ViewModel;
        }
    }
}
