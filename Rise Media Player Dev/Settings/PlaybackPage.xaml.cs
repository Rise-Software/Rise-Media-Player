using Rise.App.Common;
using Rise.App.ViewModels;
using System.Collections.Generic;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Rise.App.Settings
{
    public sealed partial class PlaybackPage : Page
    {
        private SettingsViewModel ViewModel => App.SViewModel;
        private readonly List<string> Crossfade = new()
        {
            ResourceLoaders.PlaybackLoader.GetString("Duration0"),
            ResourceLoaders.PlaybackLoader.GetString("Duration3s"),
            ResourceLoaders.PlaybackLoader.GetString("Duration5s"),
            ResourceLoaders.PlaybackLoader.GetString("Duration10s")
        };

        private readonly List<string> VideoScale = new()
        {
            ResourceLoaders.PlaybackLoader.GetString("WindowSize"),
            ResourceLoaders.PlaybackLoader.GetString("MatchRes")
        };

        public PlaybackPage()
        {
            InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;
        }
    }
}
