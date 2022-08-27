using System;
using System.Collections.Specialized;
using System.Linq;
using Rise.App.Dialogs;
using Rise.App.ViewModels;
using Rise.App.Web;
using Rise.Common.Constants;
using Rise.Common.Extensions;
using Rise.Common.Helpers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Rise.App.Views
{
    public sealed partial class HomePage : Page
    {
        /// <summary>
        /// Gets the <see cref="NavigationHelper"/> associated with this <see cref="Page"/>.
        /// </summary>
        private readonly NavigationHelper _navigationHelper;

        private MainViewModel MViewModel => App.MViewModel;

        public HomePage()
        {
            InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;

            _navigationHelper = new NavigationHelper(this);

            UpdateWidgetsVisibility();
            MViewModel.Widgets.CollectionChanged += Widgets_CollectionChanged;
        }

        private void Widgets_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
            => UpdateWidgetsVisibility();

        private void UpdateWidgetsVisibility()
        {
            if (MViewModel.Widgets.Any())
                VisualStateManager.GoToState(this, "WidgetsAddedState", false);
            else
                VisualStateManager.GoToState(this, "NoWidgetsState", false);
        }

        private async void SupportButton_Click(object sender, RoutedEventArgs e)
            => await URLs.Support.LaunchAsync();

        private async void WhatsNew_Click(object sender, RoutedEventArgs e)
        {
            _ = await typeof(WhatsNew).
                ShowInApplicationViewAsync(null, 500, 600, true);
        }

        private async void FoldersButton_Click(object sender, RoutedEventArgs e)
        {
            ContentDialog dialog = new()
            {
                Title = "Manage local media folders",
                CloseButtonText = "Close",
                Content = new Settings.MediaSourcesPage()
            };

            var result = await dialog.ShowAsync();
        }

        private async void GlanceManage_Click(object sender, RoutedEventArgs e)
        {
            ContentDialog dialog = new()
            {
                CloseButtonText = "Close",
                DefaultButton = ContentDialogButton.Primary,
                Content = new WidgetsDialogContent()
            };

            var result = await dialog.ShowAsync();
        }

        private void BrowseMedia_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(BrowsePage));
        }

        #region NavigationHelper registration
        protected override void OnNavigatedTo(NavigationEventArgs e)
            => _navigationHelper.OnNavigatedTo(e);

        protected override void OnNavigatedFrom(NavigationEventArgs e)
            => _navigationHelper.OnNavigatedFrom(e);
        #endregion
    }
}