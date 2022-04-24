using Microsoft.Toolkit.Mvvm.Messaging.Messages;
using Rise.Storage;

namespace Rise.App.Messages.FileBrowser
{
    public sealed class CurrentFolderChangedMessage : ValueChangedMessage<IFolder?>
    {
        public CurrentFolderChangedMessage(IFolder? value)
            : base(value)
        {
        }
    }
}
