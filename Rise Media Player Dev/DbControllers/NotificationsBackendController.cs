using Newtonsoft.Json;
using Rise.App.ViewModels;
using Rise.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;

namespace Rise.App.DbControllers
{
    public class NotificationsBackendController : BaseBackendController
    {
        public NotificationsBackendController() : base("Notifications") { }

        public Task AddItemAsync(string title, string description, string icon)
        {
            return InsertAsync(new()
            {
                Title = title,
                Description = description,
                Icon = icon
            });
        }

        public async Task<IEnumerable<Notification>> GetAsync()
        {
            var text = await FileIO.ReadTextAsync(DbFile);

            if (!string.IsNullOrWhiteSpace(text))
                return JsonConvert.DeserializeObject<IEnumerable<Notification>>(text);

            return Enumerable.Empty<Notification>();
        }

        public async Task InsertAsync(Notification item)
        {
            var text = await FileIO.ReadTextAsync(DbFile);
            var list = JsonConvert.DeserializeObject<List<Notification>>(text) ?? new List<Notification>();

            list.Add(item);

            string json = JsonConvert.SerializeObject(list, Formatting.Indented);
            await FileIO.WriteTextAsync(DbFile, json);
        }

        public async Task UpdateAsync(Notification item, int index)
        {
            var text = await FileIO.ReadTextAsync(DbFile);
            var list = JsonConvert.DeserializeObject<List<Notification>>(text) ?? new List<Notification>();

            list[index] = item;

            string json = JsonConvert.SerializeObject(list, Formatting.Indented);
            await FileIO.WriteTextAsync(DbFile, json);
        }

        public async Task DeleteAsync(Notification item)
        {
            var text = await FileIO.ReadTextAsync(DbFile);
            var items = JsonConvert.DeserializeObject<List<Notification>>(text) ?? new List<Notification>();

            _ = items.Remove(item);

            string json = JsonConvert.SerializeObject(items, Formatting.Indented);
            await FileIO.WriteTextAsync(DbFile, json);
        }
    }
}