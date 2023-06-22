using Rise.Data.ViewModels;
using System;

namespace Rise.App.ViewModels
{
    /// <summary>
    /// Represents a device with path, items and location.
    /// </summary>
    public sealed partial class DeviceViewModel : ViewModel
    {

        private string _name;
        /// <summary>
        /// Gets or sets the name of the device.
        /// </summary>
        public string Name
        {
            get => _name;
            set => Set(ref _name, value);
        }

        private string _deviceType;
        /// <summary>
        /// Gets or sets the device type.
        /// </summary>
        public string DeviceType
        {
            get => _deviceType;
            set => Set(ref _deviceType, value);
        }

        private string _icon;
        /// <summary>
        /// Gets or sets an icon for the device.
        /// </summary>
        public string Icon
        {
            get => _icon;
            set => Set(ref _icon, value);
        }

        private string _filePath;
        /// <summary>
        /// Gets or sets the device path.
        /// </summary>
        public string FilePath
        {
            get => _filePath;
            set => Set(ref _filePath, value);
        }

        private string _displayPath;
        /// <summary>
        /// Gets or sets the device path for app display and UI use.
        /// </summary>
        public string DisplayPath
        {
            get => _displayPath;
            set => Set(ref _displayPath, value);
        }
    }
}
