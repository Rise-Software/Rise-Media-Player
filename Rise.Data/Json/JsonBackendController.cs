using Newtonsoft.Json;
using Rise.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using TagLib.Ape;
using Windows.Foundation;
using Windows.Storage;

namespace Rise.Data.Json
{
    /// <summary>
    /// Represents a JSON based data store.
    /// </summary>
    /// <typeparam name="T">Type of items to store.</typeparam>
    public sealed partial class JsonBackendController<T>
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

    // JSON handling
    public sealed partial class JsonBackendController<T>
    {
        /// <summary>
        /// The collection of items in the controller. To load items
        /// from the data store, call <see cref="LoadStoredItems"/> or
        /// <see cref="LoadStoredItemsAsync"/>.
        /// </summary>
        public ObservableCollection<T> Items { get; } = new();

        /// <summary>
        /// Gets the items currently saved in the JSON file.
        /// </summary>
        public IEnumerable<T> GetStoredItems()
        {
            var text = FileIO.ReadTextAsync(BackingFile).Get();
            if (!string.IsNullOrWhiteSpace(text))
                return JsonConvert.DeserializeObject<IEnumerable<T>>(text);

            return Enumerable.Empty<T>();
        }

        /// <summary>
        /// Adds the items currently saved in the JSON file to the
        /// <see cref="Items"/> collection.
        /// </summary>
        public bool LoadStoredItems()
        {
            var items = GetStoredItems();
            if (items.Count() == 0)
            {
                foreach (var itm in items)
                    Items.Add(itm);

                return true;
            }
            return false;
        }

        /// <summary>
        /// Gets the items currently saved in the JSON file.
        /// </summary>
        public async Task<IEnumerable<T>> GetStoredItemsAsync()
        {
            var text = await FileIO.ReadTextAsync(BackingFile);
            if (!string.IsNullOrWhiteSpace(text))
                return JsonConvert.DeserializeObject<IEnumerable<T>>(text);

            return Enumerable.Empty<T>();
        }

        /// <summary>
        /// Adds the items currently saved in the JSON file to the
        /// <see cref="Items"/> collection.
        /// </summary>
        public async Task<bool> LoadStoredItemsAsync()
        {
            var items = await GetStoredItemsAsync();
            if (items.Count() == 0)
            {
                foreach (var itm in items)
                    Items.Add(itm);

                return true;
            }
            return false;
        }

        /// <summary>
        /// Saves the item list to the JSON file.
        /// </summary>
        /// <returns>An action representing the write operation.</returns>
        public IAsyncAction SaveAsync()
        {
            string json = JsonConvert.SerializeObject(Items);
            return FileIO.WriteTextAsync(BackingFile, json);
        }
    }
}
