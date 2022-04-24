using System.Threading.Tasks;

namespace Rise.Storage
{
    public interface IBaseStorage
    {
        IPath Path { get; }

        string Name { get; }

        Task<IFolder?> GetParentAsync();

        Task Delete(bool permanently);
    }
}
