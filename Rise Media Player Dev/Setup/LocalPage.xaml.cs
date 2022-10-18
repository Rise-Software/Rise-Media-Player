﻿using Microsoft.UI.Xaml.Controls;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Rise.App.Setup
{
    public sealed partial class LocalPage : Page
    {
        public LocalPage()
        {
            InitializeComponent();
        }

        private async void OnIconLoaded(object sender, RoutedEventArgs e)
        {
            var player = sender as AnimatedVisualPlayer;
            await player.PlayAsync(0, 0.5, false);
        }
    }
}
