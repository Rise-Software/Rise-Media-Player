using CommunityToolkit.Mvvm.Messaging.Messages;
using Rise.Storage;

namespace Rise.App.Messages.FileBrowser
{
    public sealed class OpenInFileExplorerMessage : ValueChangedMessage<IFolder?>
    {
        public OpenInFileExplorerMessage(IFolder? value)
            : base(value)
        {
        }
    }
}
