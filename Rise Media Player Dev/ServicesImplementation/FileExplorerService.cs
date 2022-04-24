using System;
using System.Threading.Tasks;
using Rise.App.Services;
using Rise.Storage;
using Windows.System;

namespace Rise.App.ServicesImplementation
{
    internal sealed class FileExplorerService : IFileExplorerService
    {
        public async Task<bool> OpenPathInFileExplorerAsync(IPath path)
        {
            return await Launcher.LaunchFolderPathAsync(path.Path);
        }
    }
}
