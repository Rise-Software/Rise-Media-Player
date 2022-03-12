using Rise.App.ViewModels;
using Rise.Common.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Rise.App.Views.Albums.Properties
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AlbumPropertiesPage : Page
    {
        private AlbumViewModel Album;
        private bool _saveChanges = false;
        private IEnumerable<ToggleButton> Toggles { get; set; }
        public AlbumPropertiesPage()
        {
            this.InitializeComponent();

            NavigationCacheMode = NavigationCacheMode.Enabled;

            Toggles = ItemGrid.GetChildren<ToggleButton>();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Album = e.Parameter as AlbumViewModel;

            Details.IsChecked = true;
            base.OnNavigatedTo(e);
        }

        private void ToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            Details.IsChecked = false;
            Songs.IsChecked = false;
            More.IsChecked = false;

            ToggleButton clicked = (ToggleButton)sender;
            clicked.Checked -= ToggleButton_Checked;
            clicked.IsChecked = true;

            switch (clicked.Tag.ToString())
            {
                case "DetailsItem":
                    _ = PropsFrame.Navigate(typeof(AlbumPropsDetailsPagexaml), Album);
                    break;

                case "FileItem":
                    //_ = PropsFrame.Navigate(typeof(FilePage), Album);
                    break;

                default:
                    break;
            }

            clicked.Checked += ToggleButton_Checked;
        }

        private async void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            _saveChanges = false;
            _ = await ApplicationView.GetForCurrentView().TryConsolidateAsync();
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            _saveChanges = true;
            _ = await ApplicationView.GetForCurrentView().TryConsolidateAsync();
        }

        private async void PlaylistPropertiesPage_Consolidated(ApplicationView sender, ApplicationViewConsolidatedEventArgs args)
        {
            _ = await ApplicationView.GetForCurrentView().TryConsolidateAsync();
        }
    }
}
