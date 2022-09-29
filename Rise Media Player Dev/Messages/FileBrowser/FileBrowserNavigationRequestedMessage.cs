using CommunityToolkit.Mvvm.Messaging.Messages;
using Rise.App.ViewModels.FileBrowser.Pages;

namespace Rise.App.Messages.FileBrowser
{
    public sealed class FileBrowserNavigationRequestedMessage : ValueChangedMessage<BaseFileBrowserPageViewModel>
    {
        public FileBrowserNavigationRequestedMessage(BaseFileBrowserPageViewModel value)
            : base(value)
        {
        }
    }
}
