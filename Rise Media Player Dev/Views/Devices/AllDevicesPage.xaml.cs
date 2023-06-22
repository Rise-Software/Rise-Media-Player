using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Rise.Common.Helpers;
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
using System.Diagnostics;
using Windows.Devices.Enumeration;
using Windows.Devices.Portable;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.System;
using NAudio.CoreAudioApi;
using System.Xml.Linq;
using Windows.UI.Xaml.Shapes;
using Rise.App.ViewModels;
using Windows.UI.Text;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Rise.App.Views.Devices
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AllDevicesPage : Page
    {
        public static MainViewModel MViewModel => App.MViewModel;

        public AllDevicesPage()
        {
            this.InitializeComponent();
            DeviceList.ItemsSource = MViewModel.Devices;
        }

        private async void DeviceList_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem is DeviceViewModel device)
            {
                var folder = await StorageFolder.GetFolderFromPathAsync(device.FilePath);
                var options = new FolderLauncherOptions();
                _ = await Launcher.LaunchFolderAsync(folder, options);
            }
        }
    }
}
