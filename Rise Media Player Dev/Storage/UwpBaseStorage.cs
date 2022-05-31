using System;
using System.Threading.Tasks;
using Rise.Storage;
using Windows.Storage;

namespace Rise.App.Storage
{
    internal abstract class UwpBaseStorage<TStorage> : IBaseStorage
        where TStorage : class, IStorageItem
    {
        protected readonly TStorage storage;

        public string Path { get; }

        public string Name { get; }

        public UwpBaseStorage(TStorage storage)
        {
            this.storage = storage;
            this.Path = storage.Path;
            this.Name = storage.Name;
        }

        public async Task DeleteAsync(bool permanently)
        {
            await storage.DeleteAsync(permanently ? StorageDeleteOption.PermanentDelete : StorageDeleteOption.Default);
        }

        internal TStorage GetInternalImpl()
        {
            return storage;
        }

        public abstract Task<IFolder?> GetParentAsync();
    }
}
