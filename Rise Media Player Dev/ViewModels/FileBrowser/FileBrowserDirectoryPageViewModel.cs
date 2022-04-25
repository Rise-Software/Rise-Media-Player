using Microsoft.Toolkit.Mvvm.Messaging;
using Rise.App.Messages.FileBrowser;
using Rise.Storage;
using Rise.App.Services;
using Microsoft.Toolkit.Mvvm.DependencyInjection;

namespace Rise.App.ViewModels.FileBrowser
{
    public sealed class FileBrowserDirectoryPageViewModel : BaseFileBrowserPageViewModel, IRecipient<OpenInFileExplorerMessage>
    {
        private IFileExplorerService FileExplorerService { get; } = Ioc.Default.GetRequiredService<IFileExplorerService>();

        public IFolder? CurrentFolder { get; private set; }

        public FileBrowserDirectoryPageViewModel(IMessenger messenger)
            : base(messenger)
        {
            messenger.Register<OpenInFileExplorerMessage>(this);
        }

        public async void Receive(OpenInFileExplorerMessage message)
        {
            var folder = message.Value ?? CurrentFolder;

            await FileExplorerService.OpenPathInFileExplorerAsync(folder?.Path ?? string.Empty);
        }
    }
}
