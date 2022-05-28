using Rise.App.Services;
using Rise.App.Storage;
using Rise.App.Storage.Devices;
using Rise.Storage;
using Rise.Storage.Devices;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;

namespace Rise.App.ServicesImplementation
{
    internal sealed class UwpStorageService : IStorageService
    {
        public Task<bool> EnsureFileSystemIsAccessible()
        {
            // TODO: Detect broadFileSystem and show dialog if unavailable
            return Task.FromResult(true);
        }

        public IEnumerable<IDrive> EnumerateDrives()
        {
            foreach (var item in DriveInfo.GetDrives())
            {
                yield return new Win32Drive(item);
            }
        }

        public IEnumerable<IDevice> EnumerateDevices()
        {
            throw new InvalidOperationException();
        }

        public async Task<IFile?> GetFileAsync(string path)
        {
            try
            {
                var file = await StorageFile.GetFileFromPathAsync(path);
                return new UwpFile(file);
            }
            catch
            {
                return null;
            }
        }

        public async Task<IFolder?> GetFolderAsync(string path)
        {
            try
            {
                var folder = await StorageFolder.GetFolderFromPathAsync(path);
                return new UwpFolder(folder);
            }
            catch
            {
                return null;
            }
        }

        public async Task<TStorage?> GetStorageAsync<TStorage>(string path)
            where TStorage : class, IBaseStorage
        {
            var storageType = typeof(TStorage);

            if (typeof(IFile).IsAssignableFrom(storageType))
            {
                return (TStorage?)(IBaseStorage?)await GetFileAsync(path);
            }
            else if (typeof(IFolder).IsAssignableFrom(storageType))
            {
                return (TStorage?)(IBaseStorage?)await GetFolderAsync(path);
            }
            else
            {
                throw new ArgumentException("Invalid storage type.", nameof(TStorage));
            }
        }
    }
}
