using Rise.App.ViewModels;
using System;
using System.Diagnostics;
using Windows.Storage;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Rise.App.Props
{
    public sealed partial class FilePage : Page
    {
        private SongPropertiesViewModel Props { get; set; }

        public FilePage()
        {
            InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is SongPropertiesViewModel props)
            {
                Props = props;
            }

            base.OnNavigatedTo(e);
        }

        private async void OpenFileLocation_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            string folderlocation = Props.Location;
            string filename = Props.Filename;
            string result = folderlocation.Replace(filename, "");
            Debug.WriteLine(result);

            try
            {
                StorageFolder folder = await StorageFolder.GetFolderFromPathAsync(result);
                await Launcher.LaunchFolderAsync(folder);
            }
            catch
            {

            }
        }
    }
}
