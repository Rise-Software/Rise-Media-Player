using System.Threading.Tasks;

namespace Rise.App.Services
{
    public interface IFileExplorerService
    {
        Task<bool> OpenPathInFileExplorerAsync(string path);
    }
}
