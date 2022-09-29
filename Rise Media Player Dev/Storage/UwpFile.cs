using Rise.Storage;
using System.Threading.Tasks;
using System;
using Windows.Storage;

namespace Rise.App.Storage
{
    internal sealed class UwpFile : UwpBaseStorage<StorageFile>, IFile
    {
        private string? _Extension;

        public string Extension
            => _Extension ??= System.IO.Path.GetExtension(storage.Name);

        public UwpFile(StorageFile storage)
            : base(storage)
        {
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
