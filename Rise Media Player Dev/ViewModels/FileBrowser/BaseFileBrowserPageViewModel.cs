using Microsoft.Toolkit.Mvvm.Messaging;
using Rise.Data.ViewModels;

namespace Rise.App.ViewModels.FileBrowser
{
    public abstract class BaseFileBrowserPageViewModel : ViewModel
    {
        protected IMessenger Messenger { get; }

        public BaseFileBrowserPageViewModel(IMessenger messenger)
        {
            this.Messenger = messenger;
        }
    }
}
