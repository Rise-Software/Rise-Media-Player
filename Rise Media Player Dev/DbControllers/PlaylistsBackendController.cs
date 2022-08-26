using Newtonsoft.Json;
using Rise.App.ViewModels;
using Rise.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;

namespace Rise.App.DbControllers
{
    /// <summary>
    /// A <see cref="IBackendController"/> for playlists.
    /// Database file is created lazily.
    /// </summary>
    public class PlaylistsBackendController : IBackendController<PlaylistViewModel>
    {
        /// <summary>
        /// The database file name
        /// </summary>
        public string DatabaseFileName => "Playlists";

        /// <summary>
        /// Gets the database file.
        /// </summary>
        /// <returns>A <see cref="Task"/> which represents the operation.</returns>
        public Task<StorageFile> GetDatabaseFileAsync()
            => ApplicationData.Current.LocalCacheFolder.CreateFileAsync($"{DatabaseFileName}.json", CreationCollisionOption.OpenIfExists).AsTask();

        /// <summary>
        /// Adds an item to the database.
        /// </summary>
        /// <param name="item">The <see cref="PlaylistViewModel"/> to add.</param>
        /// <returns>A <see cref="Task"/> which represents the operation.</returns>
        public async Task AddAsync(PlaylistViewModel item)
        {
            var file = await GetDatabaseFileAsync();
            var text = await FileIO.ReadTextAsync(file);

            List<PlaylistViewModel> items = JsonConvert.DeserializeObject<List<PlaylistViewModel>>(text) ?? new List<PlaylistViewModel>();
            items.Add(item);

            string json = JsonConvert.SerializeObject(items, Formatting.Indented);
            await FileIO.WriteTextAsync(file, json);
        }

        /// <summary>
        /// Adds an item to the database, or updates it if it already exists.
        /// </summary>
        /// <param name="item">The <see cref="PlaylistViewModel"/> to add/update.</param>
        /// <returns>A <see cref="Task"/> which represents the operation.</returns>
        public async Task AddOrUpdateAsync(PlaylistViewModel item)
        {
            var file = await GetDatabaseFileAsync();

            List<PlaylistViewModel> items = JsonConvert.DeserializeObject<List<PlaylistViewModel>>(await FileIO.ReadTextAsync(file)) ?? new List<PlaylistViewModel>();

            bool exists = items.Any(p =>
            {
                if (p != null)
                    return p.Model.Id == item.Model.Id;
                else
                    return false;
            });

            if (exists)
                await UpdateAsync(item);
            else
                await AddAsync(item);
        }

        /// <summary>
        /// Deletes an item from the database.
        /// </summary>
        /// <param name="item">The <see cref="PlaylistViewModel"/> to remove.</param>
        /// <returns>A <see cref="Task"/> which represents the operation.</returns>
        public async Task DeleteAsync(PlaylistViewModel item)
        {
            var file = await GetDatabaseFileAsync();

            // It's already deleted in the MViewModel notifications list, so why bother deleting again?
            string json = JsonConvert.SerializeObject(App.MViewModel.Playlists, Formatting.Indented);
            await FileIO.WriteTextAsync(file, json);
        }

        /// <summary>
        /// Updates an item in the database.
        /// </summary>
        /// <param name="item">The <see cref="NotificationViewModel"/> to update.</param>
        /// <returns>A <see cref="Task"/> which represents the operation.</returns>
        public async Task UpdateAsync(PlaylistViewModel item)
        {
            var file = await GetDatabaseFileAsync();

            List<PlaylistViewModel> items = JsonConvert.DeserializeObject<List<PlaylistViewModel>>(await FileIO.ReadTextAsync(file)) ?? new List<PlaylistViewModel>();

            PlaylistViewModel item1 = items.FirstOrDefault(i => i.Model.Id == item.Model.Id);
            var index = items.IndexOf(item1);

            if (index < 0)
                return;

            items[index] = item;

            string json = JsonConvert.SerializeObject(items, Formatting.Indented);
            await FileIO.WriteTextAsync(file, json);
        }

        /// <summary>
        /// Gets an item with the specified ID.
        /// </summary>
        /// <param name="id">The ID of the item to find.</param>
        /// <returns>A <see cref="Task"/> which represents the operation.</returns>
        public async Task<PlaylistViewModel> GetItemAsync(string id)
        {
            var items = await GetItemsAsync();
            return items.FirstOrDefault(item => item.Model.Id.ToString() == id);
        }

        /// <summary>
        /// Gets the items in the database.
        /// </summary>
        /// <returns>A <see cref="Task"/> which represents the operation.</returns>
        public async Task<IEnumerable<PlaylistViewModel>> GetItemsAsync()
        {
            string text = await FileIO.ReadTextAsync(await GetDatabaseFileAsync());

            if (!string.IsNullOrWhiteSpace(text))
            {
                List<PlaylistViewModel> items = JsonConvert.DeserializeObject<List<PlaylistViewModel>>(text);
                return items;
            }
            else
            {
                return new List<PlaylistViewModel>();
            }
        }
    }
}