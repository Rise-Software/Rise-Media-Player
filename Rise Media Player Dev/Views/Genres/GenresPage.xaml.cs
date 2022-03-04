using Microsoft.Toolkit.Uwp.UI;
using Rise.App.Common;
using Rise.App.Helpers;
using Rise.App.ViewModels;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

namespace Rise.App.Views
{
    public sealed partial class GenresPage : Page
    {
        /// <summary>
        /// Gets the app-wide MViewModel instance.
        /// </summary>
        private MainViewModel ViewModel => App.MViewModel;

        private AdvancedCollectionView Genres => ViewModel.FilteredGenres;

        private static readonly DependencyProperty SelectedGenreProperty =
            DependencyProperty.Register("SelectedGenre", typeof(GenreViewModel), typeof(GenresPage), null);

        private GenreViewModel SelectedGenre
        {
            get => (GenreViewModel)GetValue(SelectedGenreProperty);
            set => SetValue(SelectedGenreProperty, value);
        }

        private bool IsCtrlPressed;

        /// <summary>
        /// Gets the <see cref="NavigationHelper"/> associated with this <see cref="Page"/>.
        /// </summary>
        private readonly NavigationHelper _navigationHelper;

        public GenresPage()
        {
            InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;

            _navigationHelper = new NavigationHelper(this);
        }

        #region Event handlers
        private void GridView_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (!KeyboardHelpers.IsCtrlPressed())
            {
                if ((e.OriginalSource as FrameworkElement).DataContext is GenreViewModel genre)
                {
                    _ = Frame.Navigate(typeof(GenreSongsPage), genre);
                    SelectedGenre = null;
                }
            }
            else
            {
                if ((e.OriginalSource as FrameworkElement).DataContext is GenreViewModel genre)
                {
                    SelectedGenre = genre;
                }
            }
        }
        #endregion

        #region NavigationHelper registration
        /// <summary>
        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// Page specific logic should be placed in event handlers for the  
        /// <see cref="NavigationHelper.LoadState"/>
        /// and <see cref="NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method 
        /// in addition to page state preserved during an earlier session.
        /// </summary>
        protected override void OnNavigatedTo(NavigationEventArgs e)
            => _navigationHelper.OnNavigatedTo(e);

        protected override void OnNavigatedFrom(NavigationEventArgs e)
            => _navigationHelper.OnNavigatedFrom(e);
        #endregion

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
