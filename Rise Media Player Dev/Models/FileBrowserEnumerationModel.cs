using Rise.Storage;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Rise.App.Models
{
    public sealed class FileBrowserEnumerationModel
    {
        private readonly IReadOnlyCollection<EnumerationSource<IBaseStorage>> _enumerationSources;

        public FileBrowserEnumerationModel(IReadOnlyCollection<EnumerationSource<IBaseStorage>> enumerationSources)
        {
            _enumerationSources = enumerationSources;
        }

        public void ResetSources()
        {
            foreach (var item in _enumerationSources)
            {
                item.ResetData();
            }
        }

        public async Task EnumerateFolderAsync(IFolder folder, CancellationToken cancellationToken)
        {
            foreach (var storage in await folder.GetStorageAsync())
            {
                foreach (var item in _enumerationSources)
                {
                    if (cancellationToken.IsCancellationRequested)
                        return;

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
