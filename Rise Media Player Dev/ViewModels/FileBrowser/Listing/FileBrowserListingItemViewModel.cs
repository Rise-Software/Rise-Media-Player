using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rise.Data.ViewModels;
using Rise.Storage;

namespace Rise.App.ViewModels.FileBrowser.Listing
{
    public sealed class FileBrowserListingItemViewModel : ViewModel
    {
        private IBaseStorage _storage;

        public string Name { get; }

        public FileBrowserListingItemViewModel(IBaseStorage storage)
        {
            this._storage = storage;
            Name = storage.Name;
        }
    }
}
