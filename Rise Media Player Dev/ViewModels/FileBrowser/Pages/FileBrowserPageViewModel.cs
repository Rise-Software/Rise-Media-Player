using Microsoft.Toolkit.Mvvm.Messaging;
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
            this.Messenger = new WeakReferenceMessenger();
            this.FileBrowserHeaderViewModel = new(Messenger);

            this.Messenger.Register<FileBrowserNavigationRequestedMessage>(this);
        }

        public void Receive(FileBrowserNavigationRequestedMessage message)
        {
            CurrentPageViewModel = message.Value;
        }

        public void EnsureInitialized()
        {
            if (CurrentPageViewModel is null)
            {
                Messenger.Send(new FileBrowserNavigationRequestedMessage(new FileBrowserHomePageViewModel(Messenger)));
            }

            if (CurrentPageViewModel is FileBrowserHomePageViewModel homePageViewModel
                && homePageViewModel.Drives.Count == 0)
            {
                homePageViewModel.EnumerateDrives();
            }
        }
    }
}
