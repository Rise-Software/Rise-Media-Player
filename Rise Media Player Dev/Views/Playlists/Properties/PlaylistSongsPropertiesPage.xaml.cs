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

namespace Rise.App.Views.Playlists.Properties
{
    public sealed partial class PlaylistSongsPropertiesPage : Page
    {
        private PlaylistViewModel Playlist;

        public PlaylistSongsPropertiesPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            Playlist = e.Parameter as PlaylistViewModel;
        }

        private void RemoveSong_Click(object sender, RoutedEventArgs e)
        {
            SongViewModel song = (sender as Button).Tag as SongViewModel;
            Playlist.Songs.Remove(song);
        }

        private void MoveBottom_Click(object sender, RoutedEventArgs e)
        {
            SongViewModel song = (sender as Button).Tag as SongViewModel;

            if ((Playlist.Songs.IndexOf(song) + 1) < Playlist.Songs.Count)
            {
                Playlist.Songs.Move(Playlist.Songs.IndexOf(song), Playlist.Songs.IndexOf(song) + 1);
            }
        }

        private void MoveUp_Click(object sender, RoutedEventArgs e)
        {
            SongViewModel song = (sender as Button).Tag as SongViewModel;

            if ((Playlist.Songs.IndexOf(song) - 1) > 0)
            {
                Playlist.Songs.Move(Playlist.Songs.IndexOf(song), Playlist.Songs.IndexOf(song) - 1);
            }
        }
    }
}
