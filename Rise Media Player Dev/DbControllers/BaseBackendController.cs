using Rise.Common.Extensions;
using Windows.Storage;

namespace Rise.App.DbControllers
{
    public class BaseBackendController
    {
        public readonly StorageFile DbFile;
        public readonly string DbName;

        public BaseBackendController(string dbName)
        {
            DbFile = ApplicationData.Current.LocalCacheFolder.CreateFileAsync($"{dbName}.json", CreationCollisionOption.OpenIfExists).Get();
            DbName = dbName;
        }
    }
}
