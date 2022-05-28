using Rise.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;

using CreationCollisionOption = Rise.Storage.Enums.CreationCollisionOption;

namespace Rise.App.Storage
{
    internal sealed class UwpFolder : UwpBaseStorage<StorageFolder>, IFolder
    {
        public UwpFolder(StorageFolder storage)
            : base(storage)
        {
        }

        public async Task<IFile> CreateFileAsync(string desiredName)
        {
            var file = await storage.CreateFileAsync(desiredName);
            return new UwpFile(file);
        }

        public async Task<IFile> CreateFileAsync(string desiredName, CreationCollisionOption options)
        {
            var file = await storage.CreateFileAsync(desiredName, (Windows.Storage.CreationCollisionOption)options);
            return new UwpFile(file);
        }

        public async Task<IFolder> CreateFolderAsync(string desiredName)
        {
            var folder = await storage.CreateFolderAsync(desiredName);
            return new UwpFolder(folder);
        }

        public async Task<IFolder> CreateFolderAsync(string desiredName, CreationCollisionOption options)
        {
            var folder = await storage.CreateFolderAsync(desiredName, (Windows.Storage.CreationCollisionOption)options);
            return new UwpFolder(folder);
        }

        public async Task<IFile> GetFileAsync(string fileName)
        {
            var file = await storage.GetFileAsync(fileName);
            return new UwpFile(file);
        }

        public async Task<IFolder> GetFolderAsync(string folderName)
        {
            var folder = await storage.GetFolderAsync(folderName);
            return new UwpFolder(folder);
        }

        public async Task<IEnumerable<IFile>> GetFilesAsync()
        {
            var files = await storage.GetFilesAsync();
            return files.Select(x => new UwpFile(x));
        }

        public async Task<IEnumerable<IFolder>> GetFoldersAsync()
        {
            var folders = await storage.GetFoldersAsync();
            return folders.Select(x => new UwpFolder(x));
        }

        public async Task<IEnumerable<IBaseStorage>> GetStorageAsync()
        {
            var filesAndFolders = await storage.GetItemsAsync();
            return filesAndFolders.Select<IStorageItem, IBaseStorage>(x =>
                    x is StorageFolder folder ? new UwpFolder(folder)
                    : (x is StorageFile file ? new UwpFile(file) : null))
                .Where(x => x is not null);
        }

        public override async Task<IFolder?> GetParentAsync()
        {
            try
            {
                var parent = await storage.GetParentAsync();
                return new UwpFolder(parent);
            }
            catch
            {
                return null;
            }
        }
    }
}
