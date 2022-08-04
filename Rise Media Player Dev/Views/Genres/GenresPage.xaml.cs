using Microsoft.Toolkit.Uwp.UI.Animations;
using Rise.App.UserControls;
using Rise.App.ViewModels;
using Rise.Common.Extensions;
using Rise.Common.Helpers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

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
            : base("Name", App.MViewModel.Genres)
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
        private void MainGrid_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem is GenreViewModel genre && !KeyboardHelpers.IsCtrlPressed())
            {
                Frame.SetListDataItemForNextConnectedAnimation(genre);
                _ = Frame.Navigate(typeof(GenreSongsPage), genre.Model.Id);
            }
        }
    }
}
