using System.Collections.ObjectModel;
using Rise.App.Models;
using Rise.Data.ViewModels;
using Rise.Storage;

namespace Rise.App.ViewModels.FileBrowser.Listing
{
    public sealed class FileBrowserListingSectionViewModel : ViewModel, IEnumerationDestination<IBaseStorage>
    {
        public ObservableCollection<FileBrowserListingItemViewModel> Items { get; }

        public FileBrowserListingSectionViewModel()
        {
            Items = new();
        }

        public void AddFromEnumeration(IBaseStorage enumeration)
        {
            Items.Add(new(enumeration));
        }
    }
}
