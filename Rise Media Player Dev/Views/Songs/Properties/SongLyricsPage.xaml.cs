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
        private SongPropertiesViewModel Props { get; set; }

        public SongLyricsPage()
        {
            InitializeComponent();

            Loaded += OnLoaded;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is SongPropertiesViewModel props)
                Props = props;

            base.OnNavigatedTo(e);
        }

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            Lyrics.Text = await Props.Model.GetLyricsAsync() ?? "No lyrics found.";

            LoadingRing.IsActive = false;
            LoadingRing.Visibility = Visibility.Collapsed;
        }
    }
}
