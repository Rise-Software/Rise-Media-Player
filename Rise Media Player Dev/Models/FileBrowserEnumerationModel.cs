using System.Collections.Generic;
using System.Threading.Tasks;
using Rise.Storage;

namespace Rise.App.Models
{
    public sealed class FileBrowserEnumerationModel
    {
        private readonly IReadOnlyCollection<EnumerationSource<IBaseStorage>> _enumerationSources;

        public FileBrowserEnumerationModel(IReadOnlyCollection<EnumerationSource<IBaseStorage>> enumerationSources)
        {
            this._enumerationSources = enumerationSources;
        }

        public async Task EnumerateFolderAsync(IFolder folder)
        {
            foreach (var storage in await folder.GetStorageAsync())
            {
                foreach (var item in _enumerationSources)
                {
                    if (item.Predicate(storage))
                    {
                        var enumerationDestination = item.GetOrCreateEnumerationDestination();
                        enumerationDestination.AddFromEnumeration(storage);
                    }
                }
            }
        }
    }
}
