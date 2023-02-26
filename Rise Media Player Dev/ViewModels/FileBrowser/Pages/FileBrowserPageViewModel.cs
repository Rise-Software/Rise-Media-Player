using CommunityToolkit.Mvvm.Messaging;
using System.Linq;
using Rise.Data.ViewModels;
using Rise.App.Messages.FileBrowser;

namespace Rise.App.ViewModels.FileBrowser.Pages
{
    public sealed class FileBrowserPageViewModel : ViewModel, IRecipient<FileBrowserNavigationRequestedMessage>
    {
        public IMessenger Messenger { get; }

        public FileBrowserHeaderViewModel FileBrowserHeaderViewModel { get; }

        public BaseFileBrowserPageViewModel? CurrentPageViewModel { get; private set; }

        public FileBrowserPageViewModel()
        {
            Messenger = new WeakReferenceMessenger();
            FileBrowserHeaderViewModel = new(Messenger);

            Messenger.Register(this);
        }

        public void Receive(FileBrowserNavigationRequestedMessage message)
        {
            CurrentPageViewModel = message.Value;
        }

        public void EnsureInitialized()
        {
            if (CurrentPageViewModel == null)
                Messenger.Send(new FileBrowserNavigationRequestedMessage(FileBrowserHomePageViewModel.GetOrCreate(Messenger)));

            if (CurrentPageViewModel is FileBrowserHomePageViewModel homePageViewModel && !homePageViewModel.Drives.Any())
                homePageViewModel.EnumerateDrives();
        }
    }
}
