using Rise.Models;
using System;
using System.Collections.ObjectModel;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace RMP.App.Settings
{
    public sealed partial class InsiderWallpapers : Page
    {
        private readonly ObservableCollection<Wallpaper> Walls =
            new ObservableCollection<Wallpaper>();

        public InsiderWallpapers()
        {
            InitializeComponent();

            Walls.Add(new Wallpaper
            {
                Name = "Abstract Blue",
                ShortDescription = "Light Mode Version",
                Description = "Show off the Rise Media Player logo whilst " +
                "enjoying abstract shapes and a calm blue colour. Available in light " +
                "and dark variants.",
                Source = "ms-appx:///Assets/Wallpapers/LightBlue.png"
            });

            Walls.Add(new Wallpaper
            {
                Name = "Abstract Blue",
                ShortDescription = "Dark Mode Version",
                Description = "Show off the Rise Media Player logo whilst " +
                "enjoying abstract shapes and a calm blue colour. Available in light " +
                "and dark variants.",
                Source = "ms-appx:///Assets/Wallpapers/DarkBlue.png"
            });

            Walls.Add(new Wallpaper
            {
                Name = "Abstract Pink",
                ShortDescription = "Light Mode Version",
                Description = "Show off the Rise Media Player logo whilst " +
                "enjoying abstract shapes and a calm pink colour. Available in light " +
                "and dark variants.",
                Source = "ms-appx:///Assets/Wallpapers/LightPink.png"
            });

            Walls.Add(new Wallpaper
            {
                Name = "Abstract Pink",
                ShortDescription = "Dark Mode Version",
                Description = "Show off the Rise Media Player logo whilst " +
                "enjoying abstract shapes and a calm pink colour. Available in light " +
                "and dark variants.",
                Source = "ms-appx:///Assets/Wallpapers/DarkPink.png"
            });
        }

        // Binding to the selected item property doesn't work for whatever reason.
        private void WallsView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (WallsView.SelectedIndex > -1)
            {
                WallName.Text = Walls[WallsView.SelectedIndex].Name;
                WallShortDesc.Text = Walls[WallsView.SelectedIndex].ShortDescription;
                WallDesc.Text = Walls[WallsView.SelectedIndex].Description;
            }
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            StorageFile picFile =
                await StorageFile.GetFileFromApplicationUriAsync
                (new Uri(Walls[WallsView.SelectedIndex].Source));

            FolderPicker folderPicker = new FolderPicker
            {
                SuggestedStartLocation = PickerLocationId.PicturesLibrary
            };
            folderPicker.FileTypeFilter.Add("*");

            StorageFolder folder = await folderPicker.PickSingleFolderAsync();
            if (folder != null)
            {
                await picFile.CopyAsync(folder);
            }
        }
    }
}
