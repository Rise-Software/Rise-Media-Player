using System.Threading;
using System.Threading.Tasks;
using Rise.Storage;

namespace Rise.App.Services
{
    public interface IFileSystemShellService
    {
        Task OpenFileAsync(IFile file, CancellationToken cancellationToken = default);
    }
}
