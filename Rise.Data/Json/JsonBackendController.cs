using Rise.Common.Extensions;
using System;
using System.Threading.Tasks;
using Windows.Storage;

namespace Rise.Data.Json
{
    /// <summary>
    /// Represents a JSON based data store.
    /// </summary>
    /// <typeparam name="T">Type of items to store.</typeparam>
    public sealed class JsonBackendController<T>
    {
        private static StorageFolder _dataFolder;
        private static readonly StorageFolder dataFolder
            = _dataFolder ??= ApplicationData.Current.LocalCacheFolder;
        private readonly StorageFile BackingFile;

        private JsonBackendController(StorageFile backingFile)
        {
            BackingFile = backingFile;
        }

        /// <summary>
        /// Creates a new instance of the backend controller and
        /// returns it.
        /// </summary>
        /// <param name="filename">Filename to use for the backing
        /// JSON file.</param>
        /// <returns>An instance of the backend controller.</returns>
        public static JsonBackendController<T> Create(string filename)
        {
            var file = dataFolder.CreateFileAsync($"{filename}.json", CreationCollisionOption.OpenIfExists).Get();
            return new JsonBackendController<T>(file);
        }

        /// <summary>
        /// Asynchronously creates a new instance of the backend controller
        /// and returns it.
        /// </summary>
        /// <param name="filename">Filename to use for the backing
        /// JSON file.</param>
        /// <returns>An instance of the backend controller.</returns>
        public static async Task<JsonBackendController<T>> CreateAsync(string filename)
        {
            var file = await dataFolder.CreateFileAsync($"{filename}.json", CreationCollisionOption.OpenIfExists);
            return new JsonBackendController<T>(file);
        }
    }
}
