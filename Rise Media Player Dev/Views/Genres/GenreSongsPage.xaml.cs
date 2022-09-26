﻿using Rise.App.Helpers;
using Rise.App.UserControls;
using Rise.App.ViewModels;
using Rise.Common.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace Rise.App.Views
{
    public sealed partial class GenreSongsPage : MediaPageBase
    {
        private MainViewModel MViewModel => App.MViewModel;
        private readonly AddToPlaylistHelper PlaylistHelper;

        private GenreViewModel SelectedGenre;
        public SongViewModel SelectedItem
        {
            get => (SongViewModel)GetValue(SelectedItemProperty);
            set => SetValue(SelectedItemProperty, value);
        }

        public GenreSongsPage()
            : base("Title", App.MViewModel.Songs)
        {
            InitializeComponent();

            NavigationHelper.LoadState += NavigationHelper_LoadState;

            PlaylistHelper = new(MViewModel.Playlists, AddToPlaylistAsync);
            PlaylistHelper.AddPlaylistsToSubItem(AddTo);
            PlaylistHelper.AddPlaylistsToFlyout(AddToBar);
        }

        private void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            if (e.NavigationParameter is Guid id)
            {
                SelectedGenre = MViewModel.Genres.
                    FirstOrDefault(g => g.Model.Id == id);

                MediaViewModel.Items.Filter = s => ((SongViewModel)s).Genres.Contains(SelectedGenre.Name);
            }
            else if (e.NavigationParameter is string str)
            {
                SelectedGenre = MViewModel.Genres.
                    FirstOrDefault(g => g.Name == str);

                MediaViewModel.Items.Filter = s => ((SongViewModel)s).Genres.Contains(str);
            }
        }
    }

    // Playlists
    public sealed partial class GenreSongsPage
    {
        private Task AddToPlaylistAsync(PlaylistViewModel playlist)
        {
            var items = new List<SongViewModel>();

            foreach (var itm in MediaViewModel.Items)
                items.Add((SongViewModel)itm);

            if (playlist == null)
                return PlaylistHelper.CreateNewPlaylistAsync(items);
            else
                return playlist.AddSongsAsync(items);
        }
    }

    // Event handlers
    public sealed partial class GenreSongsPage
    {
        private void MainList_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            if ((e.OriginalSource as FrameworkElement).DataContext is SongViewModel song)
                MediaViewModel.PlayFromItemCommand.Execute(song);
        }

        private void MenuFlyout_Opening(object sender, object e)
        {
            var fl = sender as MenuFlyout;
            var cont = MainList.ItemFromContainer(fl.Target);

            if (cont == null)
                fl.Hide();
            else
                SelectedItem = (SongViewModel)cont;
        }
    }
}
