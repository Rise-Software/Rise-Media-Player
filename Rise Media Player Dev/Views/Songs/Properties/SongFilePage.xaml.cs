using Rise.App.ViewModels;
using System;
using System.Diagnostics;
using Windows.Storage;
using Windows.System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Rise.App.Views
{
    public sealed partial class SongFilePage : Page
    {
        private SongPropertiesViewModel Props { get; set; }

        public SongFilePage()
        {
            InitializeComponent();
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
