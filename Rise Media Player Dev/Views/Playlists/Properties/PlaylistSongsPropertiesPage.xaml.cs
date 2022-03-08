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
        private PlaylistViewModel _plViewModel;

        public PlaylistSongsPropertiesPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            _plViewModel = e.Parameter as PlaylistViewModel;
        }

        private async void RemoveSong_Click(object sender, RoutedEventArgs e)
        {
            SongViewModel song = (sender as Button).Tag as SongViewModel;

            _plViewModel.Songs.Remove(song);
        }

        private void MoveBottom_Click(object sender, RoutedEventArgs e)
        {
            SongViewModel song = (sender as Button).Tag as SongViewModel;

            if ((_plViewModel.Songs.IndexOf(song) + 1) < _plViewModel.Songs.Count)
            {
                _plViewModel.Songs.Move(_plViewModel.Songs.IndexOf(song), _plViewModel.Songs.IndexOf(song) + 1);
            }
        }

        private void MoveUp_Click(object sender, RoutedEventArgs e)
        {
            SongViewModel song = (sender as Button).Tag as SongViewModel;

            if ((_plViewModel.Songs.IndexOf(song) - 1) > 0)
            {
                int index1 = _plViewModel.Songs.IndexOf(song);
                int index2 = _plViewModel.Songs.IndexOf(song) - 1;
                System.Diagnostics.Debug.WriteLine(index1);
                System.Diagnostics.Debug.WriteLine(index2);
                System.Diagnostics.Debug.WriteLine(_plViewModel.Songs);
                //_plViewModel.Songs.Move(_plViewModel.Songs.IndexOf(song), _plViewModel.Songs.IndexOf(song) -1);
            }
        }
    }
}
