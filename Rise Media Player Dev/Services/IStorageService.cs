using System.Collections.Generic;
using System.Threading.Tasks;
using Rise.Storage.Devices;
using Rise.Storage;

namespace Rise.App.Services
{
    public interface IStorageService
    {
        Task<bool> EnsureFileSystemIsAccessible();

        IEnumerable<IDrive> EnumerateDrives();

        IEnumerable<IDevice> EnumerateDevices();

        Task<IFile> GetFileAsync(string path);

        Task<IFolder> GetFolderAsync(string path);

        Task<TStorage> GetStorageAsync<TStorage>(string path) where TStorage : IBaseStorage;
    }
}
