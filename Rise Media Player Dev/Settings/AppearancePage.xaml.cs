using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace RMP.App.Settings
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AppearancePage : Page
    {
        private List<string> Themes { get; set; }
        private List<string> Startup { get; set; }

        private Windows.ApplicationModel.Resources.ResourceLoader resourceLoader =
            Windows.ApplicationModel.Resources.
                ResourceLoader.GetForCurrentView("Appearance");

        public AppearancePage()
        {
            this.InitializeComponent();

            Themes = new List<string>
            {
                resourceLoader.GetString("Light"),
                resourceLoader.GetString("Dark"),
                resourceLoader.GetString("System")
            };

            Startup = new List<string>
            {
                resourceLoader.GetString("Home"),
                resourceLoader.GetString("Playlists"),
                resourceLoader.GetString("Devices"),
                resourceLoader.GetString("Songs"),
                resourceLoader.GetString("Artists"),
                resourceLoader.GetString("Albums"),
                resourceLoader.GetString("Genres"),
                resourceLoader.GetString("LocalVideos"),
                resourceLoader.GetString("Streaming"),
                resourceLoader.GetString("NowPlaying")
            };
        }

        private void SidebarCustomize_Click(object sender, RoutedEventArgs e)
        {
            Dialogs.SettingsDialog.Current.
                SettingsFrame.Navigate(typeof(NavigationPage));

            Dialogs.SettingsDialog.Current.
                Breadcrumbs.Add(resourceLoader.GetString("Sidebar"));
        }
    }
}
