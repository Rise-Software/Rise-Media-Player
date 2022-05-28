using System.Threading.Tasks;
using Microsoft.Toolkit.Mvvm.Messaging;
using Rise.App.Messages.FileBrowser;
using Rise.Storage;
using Rise.App.Services;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Rise.App.ViewModels.FileBrowser.Listing;

namespace Rise.App.ViewModels.FileBrowser.Pages
{
    public sealed class FileBrowserDirectoryPageViewModel : BaseFileBrowserPageViewModel,
        IRecipient<OpenInFileExplorerMessage>,
        IRecipient<FileBrowserDirectoryNavigationRequestedMessage>
    {
        private IFileExplorerService FileExplorerService { get; } = Ioc.Default.GetRequiredService<IFileExplorerService>();

        public FileBrowserListingViewModel ListingViewModel { get; }

        private IFolder? CurrentFolder { get; set; }

        public FileBrowserDirectoryPageViewModel(IMessenger messenger, IFolder? startingFolder = null)
            : base(messenger)
        {
            this.CurrentFolder = startingFolder;
            this.ListingViewModel = new();

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
            CurrentFolder = message.Value;
        }

        public Task EnumerateDirectoryAsync()
        {
            if (CurrentFolder is null)
            {
                return Task.CompletedTask;
            }

            return ListingViewModel.StartEnumerationAsync(CurrentFolder);
        }
    }
}
