using System.Threading.Tasks;

namespace Rise.Storage
{
    public interface IBaseStorage
    {
        string Path { get; }

        string Name { get; }

        Task<IFolder?> GetParentAsync();

        Task DeleteAsync(bool permanently);
    }
}
