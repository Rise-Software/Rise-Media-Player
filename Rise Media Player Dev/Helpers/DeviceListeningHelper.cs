namespace Rise.App.Helpers
{
    using Rise.App.ViewModels;
    using System;
    using Windows.Devices.Enumeration;
    using Windows.Devices.Portable;
    using Windows.Storage;

    public class DeviceManager
    {
        private DeviceWatcher deviceWatcher;
        public static MainViewModel MViewModel => App.MViewModel;

        public DeviceManager()
        {
            // Create a watcher for all portable storage devices
            deviceWatcher = DeviceInformation.CreateWatcher(StorageDevice.GetDeviceSelector());

            // Register event handlers for device arrival and removal
            deviceWatcher.Added += DeviceAdded;
            deviceWatcher.Removed += DeviceRemoved;

            // Start the device watcher
            UpdateDevices();
            deviceWatcher.Start();
        }

        private void DeviceAdded(DeviceWatcher sender, DeviceInformation device)
        {
            UpdateDevices();
        }

        private void DeviceRemoved(DeviceWatcher sender, DeviceInformationUpdate device)
        {
            UpdateDevices();
        }

        private async void UpdateDevices()
        {
            MViewModel.Devices.Clear();
            var drives = await KnownFolders.RemovableDevices.GetFoldersAsync();
            foreach (var item in drives)
            {
                DeviceViewModel viewModel = new()
                {
                    Name = item.DisplayName.Substring(0, item.DisplayName.Length - 5),
                    DeviceType = null,
                    FilePath = item.Path,
                    DisplayPath = "(" + item.Path + ")",
                    Icon = null
                };
                MViewModel.Devices.Add(viewModel);
            }
        }

    }
}
