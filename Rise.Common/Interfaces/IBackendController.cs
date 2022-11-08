using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;

namespace Rise.Common.Interfaces
{
    public interface IBackendController<T>
    {
        string DatabaseFileName { get; }

        Task<StorageFile> GetDatabaseFileAsync();

        Task<IEnumerable<T>> GetItemsAsync();

        Task<T> GetItemAsync(string id);

        Task AddAsync(T item);

        Task UpdateAsync(T item);

        Task AddOrUpdateAsync(T item);

        Task DeleteAsync(T item);
    }

    public interface IBackendController : IBackendController<object>
    {
        
    }
}
