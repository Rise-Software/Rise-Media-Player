using Microsoft.Toolkit.Uwp.UI;
using Microsoft.Toolkit.Uwp.UI.Animations;
using Rise.App.Helpers;
using Rise.App.UserControls;
using Rise.App.ViewModels;
using Rise.Common.Enums;
using Rise.Common.Extensions;
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
    /// <summary>
    /// Contains the songs inside an album.
    /// </summary>
    public sealed partial class AlbumSongsPage : MediaPageBase
    {
        public MainViewModel MViewModel => App.MViewModel;
        private readonly AddToPlaylistHelper PlaylistHelper;

        private AlbumViewModel SelectedAlbum;
        public SongViewModel SelectedItem
        {
            get => (SongViewModel)GetValue(SelectedItemProperty);
            set => SetValue(SelectedItemProperty, value);
        }

        private readonly AdvancedCollectionView AlbumsByArtist = new();
        private double? _offset = null;

        public AlbumSongsPage()
            : base(MediaItemType.Song, App.MViewModel.Songs)
        {
            InitializeComponent();

            NavigationHelper.LoadState += NavigationHelper_LoadState;
            NavigationHelper.SaveState += NavigationHelper_SaveState;

            PlaylistHelper = new(MViewModel.Playlists, AddToPlaylistAsync);
            PlaylistHelper.AddPlaylistsToSubItem(AddTo);
            PlaylistHelper.WatchFlyout(AddToBar);
        }

        private void OnPageLoaded(object sender, RoutedEventArgs e)
        {
            if (_offset != null)
                MainList.FindVisualChild<ScrollViewer>().ChangeView(null, _offset, null);

            TrackCountName.Text = SelectedAlbum.TrackCount + " songs";

            // Load more albums by artist only when necessary
            if (AlbumsByArtist.Count > 0)
                _ = FindName("MoreAlbumsByArtist");
        }

        private void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            if (e.NavigationParameter is Guid id)
            {
                SelectedAlbum = App.MViewModel.Albums.
                    FirstOrDefault(a => a.Model.Id == id);

                MediaViewModel.Items.Filter = s => ((SongViewModel)s).Album == SelectedAlbum.Title;
            }
            else if (e.NavigationParameter is string str)
            {
                SelectedAlbum = App.MViewModel.Albums.FirstOrDefault(a => a.Title == str);
                MediaViewModel.Items.Filter = s => ((SongViewModel)s).Album == str;
            }

            MediaViewModel.Items.SortDescriptions.Clear();
            MediaViewModel.Items.SortDescriptions.Add(new SortDescription("Disc", SortDirection.Ascending));
            MediaViewModel.Items.SortDescriptions.Add(new SortDescription("Track", SortDirection.Ascending));

            AlbumsByArtist.Source = MViewModel.Albums;
            AlbumsByArtist.Filter = a => ((AlbumViewModel)a).Title != SelectedAlbum.Title && ((AlbumViewModel)a).Artist == SelectedAlbum.Artist;
            AlbumsByArtist.SortDescriptions.Add(new SortDescription("Year", SortDirection.Descending));

            if (e.PageState != null)
            {
                bool result = e.PageState.TryGetValue("Offset", out var offset);
                if (result)
                    _offset = (double)offset;
            }
        }

        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
            var scr = MainList.FindVisualChild<ScrollViewer>();
            if (scr != null)
                e.PageState["Offset"] = scr.VerticalOffset;

            AlbumsByArtist.Filter = null;
            Frame.SetListDataItemForNextConnectedAnimation(SelectedAlbum);
        }
    }

    // Playlists
    public sealed partial class AlbumSongsPage
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

        private async void LikeAlbum_Checked(object sender, RoutedEventArgs e)
        {
            var songs = new List<SongViewModel>();

            var playlist = App.MViewModel.Playlists.
                FirstOrDefault(p => p.Title == "Liked");
            var create = playlist == null;

            if (create)
            {
                playlist = new()
                {
                    Title = $"Liked",
                    Description = "Your liked songs, albums and artists' songs go here.",
                    Icon = "ms-appx:///Assets/NavigationView/PlaylistsPage/blankplaylist.png",
                    Duration = "0"
                };
            }

            foreach (var song in MediaViewModel.Items)
                songs.Add((SongViewModel)song);

            await playlist.AddSongsAsync(songs, create);
        }

        private async void LikeAlbum_Unchecked(object sender, RoutedEventArgs e)
        {
            var songs = new List<SongViewModel>();
            var playlist = App.MViewModel.Playlists.
                FirstOrDefault(p => p.Title == "Liked");

            if (playlist == null)
                return;

            foreach (var song in MediaViewModel.Items)
                songs.Add((SongViewModel)song);

            await playlist.RemoveSongsAsync(songs);
        }
    }

    // Event handlers
    public sealed partial class AlbumSongsPage
    {
        private void MainList_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            if ((e.OriginalSource as FrameworkElement).DataContext is SongViewModel song)
                MediaViewModel.PlayFromItemCommand.Execute(song);
        }

        private void GridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem is AlbumViewModel album)
                _ = Frame.Navigate(typeof(AlbumSongsPage), album.Model.Id);
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

        private void AskDiscy_Click(object sender, RoutedEventArgs e)
        {
            DiscyOnSong.IsOpen = true;
        }
    }
}
