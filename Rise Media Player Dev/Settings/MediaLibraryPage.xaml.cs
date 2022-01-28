using Rise.App.Common;
using Rise.App.Dialogs;
using Rise.App.Helpers;
using Rise.App.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using static Rise.App.Common.Enums;

namespace Rise.App.Settings
{
    public sealed partial class MediaLibraryPage : Page
    {
        #region Variables
        private SettingsViewModel ViewModel => App.SViewModel;

        internal static MediaLibraryPage Current;

        private readonly FoldersDialog Dialog = new();
        private readonly VFoldersDialog VDialog = new();

        public ContentDialog FolderDialog = new()
        {
            Title = ResourceLoaders.MediaLibraryLoader.GetString("Folders"),
        };

        public ContentDialog VFolderDialog = new()
        {
            Title = ResourceLoaders.MediaLibraryLoader.GetString("Folders"),
        };

        private readonly List<string> Deletion = new()
        {
            ResourceLoaders.MediaLibraryLoader.GetString("OnlyApp"),
            ResourceLoaders.MediaLibraryLoader.GetString("Device")
        };
        #endregion

        public MediaLibraryPage()
        {
            InitializeComponent();
            Current = this;

            FolderDialog.Content = Dialog;
            VFolderDialog.Content = VDialog;

            NavigationCacheMode = NavigationCacheMode.Enabled;
        }

        internal bool AccountMenuText
        {
            get => LastFMStatus.IsEnabled;
            set => LastFMStatus.IsEnabled = value;
        }
        private async void ChooseFolders_Click(object sender, RoutedEventArgs e)
            => _ = await FolderDialog.ShowAsync(ExistingDialogOptions.CloseExisting);

        private async void VChooseFolders_Click(object sender, RoutedEventArgs e)
            => _ = await VFolderDialog.ShowAsync(ExistingDialogOptions.CloseExisting);

        private void CommandBarButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            switch (button.Tag.ToString())
            {
                case "Insider":
                    _ = Frame.Navigate(typeof(InsiderPage));
                    SettingsDialogContainer.Breadcrumbs.Add
                        (ResourceLoaders.SidebarLoader.GetString("Ins"));
                    break;

                case "OnlineServices":
                    break;
                default:
                    break;
            }
        }

        private async void LastFmFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await LastFMHelper.LogIn();
            }
            catch (Exception e1)
            {
                Debug.WriteLine(e1.Message);
            }
        }
    }
}
