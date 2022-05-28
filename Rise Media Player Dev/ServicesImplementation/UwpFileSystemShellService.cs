using System;
using System.Threading.Tasks;
using Windows.System;
using Rise.App.Services;
using Rise.App.Storage;
using Rise.Storage;

namespace Rise.App.ServicesImplementation
{
    internal sealed class UwpFileSystemShellService : IFileSystemShellService
    {
        public async Task OpenFileAsync(IFile file)
        {
            await Launcher.LaunchFileAsync((file as UwpFile)!.GetInternalImpl());
        }
    }
}
