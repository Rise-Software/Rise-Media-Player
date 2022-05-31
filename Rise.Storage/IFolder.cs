using System.Collections.Generic;
using System.Threading.Tasks;
using Rise.Storage.Enums;

namespace Rise.Storage
{
    public interface IFolder : IBaseStorage
    {
        Task<IFile> CreateFileAsync(string desiredName);

        Task<IFile> CreateFileAsync(string desiredName, CreationCollisionOption options);

        Task<IFolder> CreateFolderAsync(string desiredName);

        Task<IFolder> CreateFolderAsync(string desiredName, CreationCollisionOption options);

        Task<IFile?> GetFileAsync(string fileName);

        Task<IFolder?> GetFolderAsync(string folderName);

        Task<IEnumerable<IFile>> GetFilesAsync();

        Task<IEnumerable<IFolder>> GetFoldersAsync();

        Task<IEnumerable<IBaseStorage>> GetStorageAsync();
    }
}
