using Microsoft.Toolkit.Mvvm.Messaging;
using Rise.Storage;

namespace Rise.App.ViewModels.FileBrowser
{
    public sealed class FileBrowserDirectoryPageViewModel : BaseFileBrowserPageViewModel
    {
        public IFolder CurrentFolder { get; private set; }

        public FileBrowserDirectoryPageViewModel(IMessenger messenger)
            : base(messenger)
        {
        }
    }
}
