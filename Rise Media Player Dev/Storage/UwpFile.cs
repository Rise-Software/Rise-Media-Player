using Rise.Storage;
using System.Threading.Tasks;
using System;
using Windows.Storage;

namespace Rise.App.Storage
{
    internal sealed class UwpFile : UwpBaseStorage<StorageFile>, IFile
    {
        public string Extension { get; }

        public UwpFile(StorageFile storage)
            : base(storage)
        {
            this.Extension = System.IO.Path.GetExtension(storage.Name);
        }

        public override async Task<IFolder> GetParentAsync()
        {
            var parent = await storage.GetParentAsync();
            return new UwpFolder(parent);
        }
    }
}
