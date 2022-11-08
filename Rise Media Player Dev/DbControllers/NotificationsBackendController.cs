﻿using Newtonsoft.Json;
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
    /// A <see cref="IBackendController"/> for notifications.
    /// Database file is created lazily.
    /// </summary>
    public class NotificationsBackendController : IBackendController<NotificationViewModel>
    {
        /// <summary>
        /// The database file name
        /// </summary>
        public string DatabaseFileName => "Notifications";

        /// <summary>
        /// Gets the database file.
        /// </summary>
        /// <returns>A <see cref="Task"/> which represents the operation.</returns>
        public Task<StorageFile> GetDatabaseFileAsync()
            => ApplicationData.Current.LocalCacheFolder.CreateFileAsync($"{DatabaseFileName}.json", CreationCollisionOption.OpenIfExists).AsTask();

        /// <summary>
        /// Gets the items in the database.
        /// </summary>
        /// <returns>A <see cref="Task"/> which represents the operation.</returns>
        public async Task<IEnumerable<NotificationViewModel>> GetItemsAsync()
        {
            string text = await FileIO.ReadTextAsync(await GetDatabaseFileAsync());

            if (!string.IsNullOrWhiteSpace(text))
            {
                List<NotificationViewModel> items = JsonConvert.DeserializeObject<List<NotificationViewModel>>(text);
                return items;
            }
            else
            {
                return new List<NotificationViewModel>();
            }
        }

        /// <summary>
        /// Gets an item with the specified ID.
        /// </summary>
        /// <param name="id">The ID of the item to find.</param>
        /// <returns>A <see cref="Task"/> which represents the operation.</returns>
        public async Task<NotificationViewModel> GetItemAsync(string id)
        {
            var items = await GetItemsAsync();
            return items.FirstOrDefault(item => item.Model.Id.ToString() == id);
        }

        /// <summary>
        /// Adds an item to the database.
        /// </summary>
        /// <param name="item">The <see cref="NotificationViewModel"/> to add.</param>
        /// <returns>A <see cref="Task"/> which represents the operation.</returns>
        public async Task AddAsync(NotificationViewModel item)
        {
            var file = await GetDatabaseFileAsync();
            var text = await FileIO.ReadTextAsync(file);

            List<NotificationViewModel> items = JsonConvert.DeserializeObject<List<NotificationViewModel>>(text) ?? new List<NotificationViewModel>();
            items.Add(item);

            string json = JsonConvert.SerializeObject(items, Formatting.Indented);
            await FileIO.WriteTextAsync(file, json);
        }

        /// <summary>
        /// Updates an item in the database.
        /// </summary>
        /// <param name="item">The <see cref="NotificationViewModel"/> to update.</param>
        /// <returns>A <see cref="Task"/> which represents the operation.</returns>
        public async Task UpdateAsync(NotificationViewModel item)
        {
            var file = await GetDatabaseFileAsync();

            List<NotificationViewModel> items = JsonConvert.DeserializeObject<List<NotificationViewModel>>(await FileIO.ReadTextAsync(file)) ?? new List<NotificationViewModel>();

            NotificationViewModel item1 = items.FirstOrDefault(i => i.Model.Id == item.Model.Id);
            var index = items.IndexOf(item1);

            if (index < 0)
                return;

            items[index] = item;

            string json = JsonConvert.SerializeObject(items, Formatting.Indented);
            await FileIO.WriteTextAsync(file, json);
        }

        /// <summary>
        /// Adds an item to the database, or updates it if it already exists.
        /// </summary>
        /// <param name="item">The <see cref="NotificationViewModel"/> to add/update.</param>
        /// <returns>A <see cref="Task"/> which represents the operation.</returns>
        public async Task AddOrUpdateAsync(NotificationViewModel item)
        {
            var file = await GetDatabaseFileAsync();

            List<NotificationViewModel> items = JsonConvert.DeserializeObject<List<NotificationViewModel>>(await FileIO.ReadTextAsync(file)) ?? new List<NotificationViewModel>();

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
        /// <param name="item">The <see cref="NotificationViewModel"/> to remove.</param>
        /// <returns>A <see cref="Task"/> which represents the operation.</returns>
        public async Task DeleteAsync(NotificationViewModel item)
        {
            var file = await GetDatabaseFileAsync();

            // It's already deleted in the MViewModel notifications list, so why bother deleting again?
            string json = JsonConvert.SerializeObject(App.MViewModel.Notifications, Formatting.Indented);
            await FileIO.WriteTextAsync(file, json);
        }
    }
}