using CommunityToolkit.Mvvm.Messaging.Messages;
using Rise.Storage;

namespace Rise.App.Messages.FileBrowser
{
    public sealed class FileBrowserDirectoryNavigationRequestedMessage : ValueChangedMessage<IFolder>
    {
        public FileBrowserDirectoryNavigationRequestedMessage(IFolder value)
            : base(value)
        {
        }
    }
}
