using Rise.App.Dialogs;
using Rise.App.ViewModels;
using Rise.Common;
using System;
using System.Collections.Generic;
using Windows.UI.Xaml.Controls;

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
        }

        private async void OnEqualizerExpanderClick(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            await new EqualizerDialog().ShowAsync();
        }
    }
}
