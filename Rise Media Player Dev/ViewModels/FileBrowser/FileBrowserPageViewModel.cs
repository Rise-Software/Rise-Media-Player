using Microsoft.Toolkit.Mvvm.Messaging;
using Rise.Data.ViewModels;
using System.Threading.Tasks;

namespace Rise.App.ViewModels.FileBrowser
{
    public sealed class FileBrowserPageViewModel : ViewModel
    {
        private IMessenger Messenger { get; }

        public FileBrowserHeaderViewModel FileBrowserHeaderViewModel { get; }

        public BaseFileBrowserPageViewModel? CurrentPageViewModel { get; private set; }

        public FileBrowserPageViewModel()
        {
            Messenger = new WeakReferenceMessenger();
            FileBrowserHeaderViewModel = new(Messenger);
        }

        public async Task TryInitialize()
        {
            CurrentPageViewModel ??= new FileBrowserHomePageViewModel(Messenger);

            if (CurrentPageViewModel is FileBrowserHomePageViewModel homePageViewModel
                && homePageViewModel.Drives.Count == 0)
            {
                await homePageViewModel.EnumerateDrivesAsync();
            }
        }
    }
}
