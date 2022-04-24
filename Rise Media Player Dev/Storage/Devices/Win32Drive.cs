using Rise.Storage;
using Rise.Storage.Devices;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Rise.App.Storage.Devices
{
    internal sealed class Win32Drive : IDrive
    {
        private readonly DriveInfo _internalDrive;

        public string Name => _internalDrive.Name;

        public string VolumeLabel => _internalDrive.VolumeLabel;

        public long AvailableFreeSpace => _internalDrive.AvailableFreeSpace;

        public long TotalFreeSpace => _internalDrive.TotalFreeSpace;

        public long TotalSize => _internalDrive.TotalSize;

        public Win32Drive(DriveInfo internalDrive)
        {
            this._internalDrive = internalDrive;
        }

        public Task<IFolder> GetRootFolderAsync()
        {
            throw new NotImplementedException();
        }
    }
}
