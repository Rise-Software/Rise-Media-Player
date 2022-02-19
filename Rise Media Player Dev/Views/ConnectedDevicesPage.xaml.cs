using Rise.App.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace Rise.App.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ConnectedDevicesPage : Page
    {
        private Collection<DeviceViewModel> _devices = new();

        public ConnectedDevicesPage()
        {
            InitializeComponent();

            for (int i = 0; i < 10; i++)
            {
                _devices.Add(new DeviceViewModel()
                {
                    Title = $"Device {i + 1}",
                    Description = "A cool device :)",
                    Online = true
                });
            }
        }
    }
}
