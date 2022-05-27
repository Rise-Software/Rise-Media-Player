using Rise.Storage;
using Rise.Storage.Devices;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Rise.App.Services;

namespace Rise.App.Storage.Devices
{
    internal sealed class Win32Drive : IDrive
    {
        private IStorageService StorageService { get; } = Ioc.Default.GetRequiredService<IStorageService>();

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

        public Task<IFolder?> GetRootFolderAsync()
        {
            return StorageService.GetFolderAsync(Name)!;
        }
    }
}
