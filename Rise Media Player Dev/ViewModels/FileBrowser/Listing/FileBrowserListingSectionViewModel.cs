using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Messaging;
using Rise.App.Models;
using Rise.Data.ViewModels;
using Rise.Storage;

namespace Rise.App.ViewModels.FileBrowser.Listing
{
    public sealed class FileBrowserListingSectionViewModel : ViewModel, IEnumerationDestination<IBaseStorage>
    {
        private IMessenger Messenger { get; }

        public ObservableCollection<FileBrowserListingItemViewModel> Items { get; }

        public string? SectionName { get; init; }

        public FileBrowserListingSectionViewModel(IMessenger messenger)
        {
            Messenger = messenger;
            Items = new();
        }

        public void AddFromEnumeration(IBaseStorage enumeration)
        {
            Items.Add(new(enumeration, Messenger));
        }
    }
}
