using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace Rise.App.DbControllers
{
    public class BaseBackendController
    {
        public static StorageFile DbFile;
        public static string DbName;

        public BaseBackendController(string dbName)
        {
            _ = setupBackend(dbName);
        }

        private async Task setupBackend(string dbName)
        {
            DbFile = await ApplicationData.Current.LocalCacheFolder.CreateFileAsync($"{dbName}.json", CreationCollisionOption.OpenIfExists);
            DbName = dbName;
        }
    }
}
