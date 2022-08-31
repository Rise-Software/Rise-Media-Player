using System;
using Microsoft.Toolkit.Uwp.UI;
using Rise.App.DbControllers;
using Rise.App.Dialogs;
using Rise.App.ViewModels;
using Rise.App.Web;
using Rise.Common.Constants;
using Rise.Common.Extensions;
using Rise.Common.Helpers;
using Rise.Data.ViewModels;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Rise.App.Views
{
    public sealed partial class HomePage : Page
    {
        private readonly NavigationHelper _navigationHelper;
        private readonly AdvancedCollectionView WidgetCollection;

        private MainViewModel MViewModel => App.MViewModel;
        private WidgetsBackendController WBackendController => App.WBackendController;

        public HomePage()
        {
            InitializeComponent();

            _navigationHelper = new NavigationHelper(this);
            _navigationHelper.SaveState += NavigationHelper_SaveState;

            WidgetCollection = new(MViewModel.Widgets, true);
            WidgetCollection.Filter = w => ((WidgetViewModel)w).Enabled;

            WidgetCollection.VectorChanged += WidgetCollection_VectorChanged;
            UpdateWidgetsVisibility();
        }

        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
            WidgetCollection.Filter = null;
            WidgetCollection.VectorChanged -= WidgetCollection_VectorChanged;
        }

        #region NavigationHelper registration
        protected override void OnNavigatedTo(NavigationEventArgs e)
            => _navigationHelper.OnNavigatedTo(e);

        protected override void OnNavigatedFrom(NavigationEventArgs e)
            => _navigationHelper.OnNavigatedFrom(e);
        #endregion
    }

    // Event handlers
    public sealed partial class HomePage
    {
        private void WidgetCollection_VectorChanged(IObservableVector<object> sender, IVectorChangedEventArgs args)
            => UpdateWidgetsVisibility();

        private void UpdateWidgetsVisibility()
        {
            if (WidgetCollection.Count > 0)
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

            _ = await dialog.ShowAsync();
            WidgetCollection.RefreshFilter();

            foreach (var widget in MViewModel.Widgets)
                await WBackendController.AddOrUpdateAsync(widget);
        }

        private void BrowseMedia_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(BrowsePage));
        }
    }
}