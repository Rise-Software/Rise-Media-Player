using System.Collections.Generic;
using System.Threading;
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

        public void ResetSources()
        {
            foreach (var item in _enumerationSources)
            {
                item.ResetData();
            }
        }

        public async Task EnumerateFolderAsync(IFolder folder, CancellationToken token)
        {
            foreach (var storage in await folder.GetStorageAsync())
            {
                foreach (var item in _enumerationSources)
                {
                    if (token.IsCancellationRequested)
                    {
                        return;
                    }

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
