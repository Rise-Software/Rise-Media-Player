using System;
using System.Threading.Tasks;
using Windows.System;
using Rise.App.Services;
using Rise.App.Storage;
using Rise.Storage;
using System.Threading;

namespace Rise.App.ServicesImplementation
{
    internal sealed class UwpFileSystemShellService : IFileSystemShellService
    {
        public Task OpenFileAsync(IFile file, CancellationToken cancellationToken = default)
        {
            if (file is not UwpFile uwpFile)
                return Task.CompletedTask;

            return Launcher.LaunchFileAsync(uwpFile.GetInternalImpl()).AsTask(cancellationToken);
        }
    }
}
