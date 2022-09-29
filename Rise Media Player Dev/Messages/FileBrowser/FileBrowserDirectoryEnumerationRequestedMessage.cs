using System.Threading;
using CommunityToolkit.Mvvm.Messaging.Messages;

namespace Rise.App.Messages.FileBrowser
{
    public sealed class FileBrowserDirectoryEnumerationRequestedMessage : ValueChangedMessage<CancellationToken>
    {
        public FileBrowserDirectoryEnumerationRequestedMessage(CancellationToken value)
            : base(value)
        {
        }
    }
}
