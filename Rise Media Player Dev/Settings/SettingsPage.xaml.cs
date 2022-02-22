using Microsoft.UI.Xaml.Controls;
using Rise.App.Common;
using Rise.App.Dialogs;
using Rise.App.ViewModels;
using Rise.App.Views;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Navigation;

namespace Rise.App.Settings
{
    public sealed partial class SettingsPage : Page
    {
        #region Variables
        private SettingsViewModel ViewModel => App.SViewModel;

        private List<SettingsItemViewModel> SettingsItems = new();
        private List<SettingsItemViewModel> SettingsFooterItems = new();
        #endregion

        public SettingsPage()
        {
            InitializeComponent();
            
            NavigationCacheMode = NavigationCacheMode.Enabled;

            SettingsItems.Add(new SettingsItemViewModel("Sample sample", "\uF8AE", "STag1"));
            SettingsItems.Add(new SettingsItemViewModel("Sample sample", "\uF8AE", "STag2"));
            SettingsItems.Add(new SettingsItemViewModel("Sample sample", "\uF8AE", "STag3"));

            SettingsFooterItems.Add(new SettingsItemViewModel("Sample sample11", "\uF8AE", "STag1"));
            SettingsFooterItems.Add(new SettingsItemViewModel("Sample sample11", "\uF8AE", "STag2"));
            SettingsFooterItems.Add(new SettingsItemViewModel("Sample sampl11e", "\uF8AE", "STag3"));

            _ = new ApplicationTitleBar(TitleBar);
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // for footer
            if (MainPagesList.SelectedItem != null)
            {
                MainPagesList.SelectionChanged -= MainPagesList_SelectionChanged;
                MainPagesList.SelectedItem = null;
                MainPagesList.SelectionChanged += MainPagesList_SelectionChanged;
            }
        }

        private void MainPagesList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FooterPagesList.SelectedItem != null)
            {
                FooterPagesList.SelectionChanged -= ListView_SelectionChanged;
                FooterPagesList.SelectedItem = null;
                FooterPagesList.SelectionChanged += ListView_SelectionChanged;
            }
        }

        /*private void ChangeIcons(bool coloredIconsVisible)
        {
            Visibility coloredIconVisibility = Visibility.Collapsed;
            Visibility monoIconVisibility = Visibility.Visible;

            if (coloredIconsVisible)
            {
                coloredIconVisibility = Visibility.Visible;
                monoIconVisibility = Visibility.Collapsed;
            }

            foreach (FontIcon icon in FontIcons)
            {
                icon.Visibility = monoIconVisibility;
            }

            foreach (ImageIcon icon in ImageIcons)
            {
                icon.Visibility = coloredIconVisibility;
            }
        }*/
    }
}
