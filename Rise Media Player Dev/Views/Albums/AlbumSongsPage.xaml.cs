using Microsoft.Toolkit.Uwp.UI;
using Microsoft.Toolkit.Uwp.UI.Animations.Expressions;
using Rise.App.UserControls;
using Rise.App.ViewModels;
using Rise.Common.Extensions;
using Rise.Common.Helpers;
using Rise.Data.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

namespace Rise.App.Views
{
    /// <summary>
    /// Contains the songs inside an album.
    /// </summary>
    public sealed partial class AlbumSongsPage : MediaPageBase
    {
        public MainViewModel MViewModel => App.MViewModel;
        private JsonBackendController<PlaylistViewModel> PBackend
            => App.MViewModel.PBackend;

        private AlbumViewModel SelectedAlbum;
        public SongViewModel SelectedItem
        {
            get => (SongViewModel)GetValue(SelectedItemProperty);
            set => SetValue(SelectedItemProperty, value);
        }

        private readonly AdvancedCollectionView AlbumsByArtist = new();

        private bool MoreAlbumsExpanded;

        private Compositor _compositor;
        private SpriteVisual _backgroundVisual;

        public AlbumSongsPage()
            : base("Disc", App.MViewModel.Songs, App.MViewModel.Playlists)
        {
            InitializeComponent();

            NavigationHelper.LoadState += NavigationHelper_LoadState;
            NavigationHelper.SaveState += NavigationHelper_SaveState;

            PlaylistHelper.AddPlaylistsToSubItem(AddTo, AddSelectedItemToPlaylistCommand);
            PlaylistHelper.AddPlaylistsToFlyout(AddToBar, AddMediaItemsToPlaylistCommand);
        }

        private void OnPageLoaded(object sender, RoutedEventArgs e)
        {
            // Load more albums by artist only when necessary
            if (AlbumsByArtist.Count > 0)
                _ = FindName("MoreAlbumsByArtist");

            var scrollViewer = MainList.FindDescendant<ScrollViewer>();

            var propSet = ElementCompositionPreview.GetScrollViewerManipulationPropertySet(scrollViewer);
            _compositor = propSet.Compositor;
            CreateImageBackgroundGradientVisual(propSet.GetSpecializedReference<ManipulationPropertySetReferenceNode>().Translation.Y);
        }

        private void BackgroundHost_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (_backgroundVisual == null) return;
            _backgroundVisual.Size = new Vector2((float)e.NewSize.Width, (float)BackgroundHost.Height);
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

            MediaViewModel.Items.SortDescriptions.Add(new SortDescription("Track", SortDirection.Ascending));

            AlbumsByArtist.Source = MViewModel.Albums;
            AlbumsByArtist.Filter = a => ((AlbumViewModel)a).Title != SelectedAlbum.Title && ((AlbumViewModel)a).Artist == SelectedAlbum.Artist;
            AlbumsByArtist.SortDescriptions.Add(new SortDescription("Year", SortDirection.Descending));
        }

        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
            AlbumsByArtist.ClearFilter();
        }
    }

    // Playlists
    public sealed partial class AlbumSongsPage
    {
        private async void LikeAlbum_Checked(object sender, RoutedEventArgs e)
        {
            var playlist = PBackend.Items.FirstOrDefault(p => p.Title == "Liked");
            if (playlist == null)
            {
                playlist = new()
                {
                    Title = $"Liked",
                    Description = "Your liked songs, albums and artists' songs go here.",
                    Icon = "ms-appx:///Assets/NavigationView/PlaylistsPage/LikedPlaylist.png"
                };
                PBackend.Items.Add(playlist);
            }

            foreach (var song in MediaViewModel.Items)
                playlist.Songs.Add((SongViewModel)song);

            await PBackend.SaveAsync();
        }

        private async void LikeAlbum_Unchecked(object sender, RoutedEventArgs e)
        {
            var playlist = PBackend.Items.FirstOrDefault(p => p.Title == "Liked");
            if (playlist == null)
                return;

            foreach (var song in MediaViewModel.Items)
                _ = playlist.Songs.Remove((SongViewModel)song);

            await PBackend.SaveAsync();
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

        private void UpDown_Click(object sender, RoutedEventArgs e)
        {
            if (MoreAlbumsExpanded)
                VisualStateManager.GoToState(this, "Collapsed", true);
            else
                VisualStateManager.GoToState(this, "Expanded", true);

            MoreAlbumsExpanded = !MoreAlbumsExpanded;
        }

        private void CreateImageBackgroundGradientVisual(ScalarNode scrollVerticalOffset)
        {
            if (_compositor == null) return;

            var imageSurface = LoadedImageSurface.StartLoadFromUri(new(SelectedAlbum.Thumbnail));
            var imageBrush = _compositor.CreateSurfaceBrush(imageSurface);
            imageBrush.HorizontalAlignmentRatio = 0.5f;
            imageBrush.VerticalAlignmentRatio = 0.5f;
            imageBrush.Stretch = CompositionStretch.UniformToFill;

            var gradientBrush = _compositor.CreateLinearGradientBrush();
            gradientBrush.EndPoint = new Vector2(0, 1);
            gradientBrush.MappingMode = CompositionMappingMode.Relative;
            gradientBrush.ColorStops.Add(_compositor.CreateColorGradientStop(0.6f, Colors.White));
            gradientBrush.ColorStops.Add(_compositor.CreateColorGradientStop(1, Colors.Transparent));

            var maskBrush = _compositor.CreateMaskBrush();
            maskBrush.Source = imageBrush;
            maskBrush.Mask = gradientBrush;

            SpriteVisual visual = _backgroundVisual = _compositor.CreateSpriteVisual();
            visual.Size = new Vector2((float)BackgroundHost.ActualWidth, (float)BackgroundHost.Height);
            visual.Opacity = 0.15f;
            visual.Brush = maskBrush;

            visual.StartAnimation("Offset.Y", scrollVerticalOffset);
            imageBrush.StartAnimation("Offset.Y", -scrollVerticalOffset * 0.8f);

            ElementCompositionPreview.SetElementChildVisual(BackgroundHost, visual);
        }
    }
}
