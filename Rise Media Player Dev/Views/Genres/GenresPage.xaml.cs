using Microsoft.Toolkit.Uwp.UI.Animations;
using Rise.App.UserControls;
using Rise.App.ViewModels;
using Rise.Common.Enums;
using Rise.Common.Extensions;
using Rise.Common.Helpers;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace Rise.App.Views
{
    public sealed partial class GenresPage : MediaPageBase
    {
        public GenreViewModel SelectedItem
        {
            get => (GenreViewModel)GetValue(SelectedItemProperty);
            set => SetValue(SelectedItemProperty, value);
        }

        private readonly string Label = "Genres";
        private double? _offset = null;

        public GenresPage()
            : base(MediaItemType.Genre, App.MViewModel.Genres)
        {
            InitializeComponent();

            NavigationHelper.LoadState += NavigationHelper_LoadState;
            NavigationHelper.SaveState += NavigationHelper_SaveState;
        }

        private void OnPageLoaded(object sender, RoutedEventArgs e)
        {
            if (_offset != null)
                MainGrid.FindVisualChild<ScrollViewer>().ChangeView(null, _offset, null);
        }

        private void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            if (e.PageState != null)
            {
                bool result = e.PageState.TryGetValue("Offset", out var offset);
                if (result)
                    _offset = (double)offset;
            }
        }

        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
            var scr = MainGrid.FindVisualChild<ScrollViewer>();
            if (scr != null)
                e.PageState["Offset"] = scr.VerticalOffset;
        }
    }

    // Event handlers
    public sealed partial class GenresPage
    {
        private void GridView_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if ((e.OriginalSource as FrameworkElement).DataContext is GenreViewModel genre)
            {
                if (!KeyboardHelpers.IsCtrlPressed())
                {
                    Frame.SetListDataItemForNextConnectedAnimation(genre);
                    _ = Frame.Navigate(typeof(GenreSongsPage), genre.Model.Id);
                }
                else
                {
                    SelectedItem = genre;
                }
            }
        }

        private async void AddFolders_Click(object sender, RoutedEventArgs e)
        {
            ContentDialog dialog = new ContentDialog();
            dialog.Title = "Manage local media folders";
            dialog.CloseButtonText = "Close";
            dialog.Content = new Settings.MediaSourcesPage();
            var result = await dialog.ShowAsync();
        }
    }
}
