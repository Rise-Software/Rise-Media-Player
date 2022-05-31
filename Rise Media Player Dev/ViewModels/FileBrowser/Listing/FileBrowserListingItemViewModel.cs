using System.Threading.Tasks;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Microsoft.Toolkit.Mvvm.Messaging;
using Rise.App.Messages.FileBrowser;
using Rise.App.Services;
using Rise.Data.ViewModels;
using Rise.Storage;

namespace Rise.App.ViewModels.FileBrowser.Listing
{
    public sealed class FileBrowserListingItemViewModel : ViewModel
    {
        private IBaseStorage _storage;

        private IFileSystemShellService FileSystemShellService { get; } = Ioc.Default.GetRequiredService<IFileSystemShellService>();

        private IMessenger Messenger { get; }

        public string Name { get; }

        public FileBrowserListingItemViewModel(IBaseStorage storage, IMessenger messenger)
        {
            this._storage = storage;
            this.Messenger = messenger;
            this.Name = storage.Name;
        }

        public async Task OpenAsync()
        {
            if (_storage is IFile file)
            {
                await FileSystemShellService.OpenFileAsync(file);
            }
            else if (_storage is IFolder folder)
            {
                Messenger.Send(new FileBrowserDirectoryNavigationRequestedMessage(folder));
            }
        }
    }
}
