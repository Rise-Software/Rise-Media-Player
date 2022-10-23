using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Messaging;
using Rise.App.Messages.FileBrowser;
using Rise.Storage;
using Rise.App.Services;
using CommunityToolkit.Mvvm.DependencyInjection;
using Rise.App.ViewModels.FileBrowser.Listing;

namespace Rise.App.ViewModels.FileBrowser.Pages
{
    public sealed class FileBrowserDirectoryPageViewModel : BaseFileBrowserPageViewModel, IRecipient<OpenInFileExplorerMessage>, IRecipient<FileBrowserDirectoryNavigationRequestedMessage>
    {
        private IFileExplorerService FileExplorerService { get; } = Ioc.Default.GetRequiredService<IFileExplorerService>();

        public FileBrowserListingViewModel ListingViewModel { get; }

        public IFolder CurrentFolder { get; private set; }

        public FileBrowserDirectoryPageViewModel(IMessenger messenger, IFolder startingFolder)
            : base(messenger)
        {
            ChangeFolder(startingFolder);
            this.ListingViewModel = new(messenger);

            messenger.Register<OpenInFileExplorerMessage>(this);
            messenger.Register<FileBrowserDirectoryNavigationRequestedMessage>(this);
        }

        public async void Receive(OpenInFileExplorerMessage message)
        {
            var folder = message.Value ?? CurrentFolder;
            await FileExplorerService.OpenPathInFileExplorerAsync(folder?.Path ?? string.Empty);
        }

        public void Receive(FileBrowserDirectoryNavigationRequestedMessage message)
        {
            ChangeFolder(message.Value);
        }

        public Task EnumerateDirectoryAsync(CancellationToken cancellationToken)
            => ListingViewModel.StartEnumerationAsync(CurrentFolder, cancellationToken);

        private void ChangeFolder(IFolder folder)
        {
            CurrentFolder = folder;
            Messenger.Send(new FileBrowserDirectoryEnumerationRequestedMessage(CancellationToken.None));
        }
    }
}
