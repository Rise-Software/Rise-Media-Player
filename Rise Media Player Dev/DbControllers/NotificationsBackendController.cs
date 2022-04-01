using Newtonsoft.Json;
using Rise.App.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;

namespace Rise.App.DbControllers
{
    public class NotificationsBackendController : BaseBackendController
    {
        public NotificationsBackendController(string dbName) : base(dbName) { }

        public NotificationsBackendController() : base("Notifications") { }

        public async Task AddNotificationAsync(string title, string description, string icon)
        {
            NotificationViewModel notification = new(new Models.Notification
            {
                Title = title,
                Description = description,
                Icon = icon
            });

            notification.IsNew = true;
            await notification.SaveAsync();
        }

        public async Task<List<NotificationViewModel>> GetAsync()
        {
            string text = await FileIO.ReadTextAsync(await ApplicationData.Current.LocalCacheFolder.CreateFileAsync($"Playlists.json", CreationCollisionOption.OpenIfExists));
            if (!string.IsNullOrWhiteSpace(text))
            {
                List<NotificationViewModel> notifications = JsonConvert.DeserializeObject<List<NotificationViewModel>>(await FileIO.ReadTextAsync(DbFile));
                return notifications;
            }
            else
            {
                return new List<NotificationViewModel>();
            }
        }

        public async Task InsertAsync(NotificationViewModel notification)
        {
            List<NotificationViewModel> notifications = JsonConvert.DeserializeObject<List<NotificationViewModel>>(await FileIO.ReadTextAsync(DbFile)) ?? new List<NotificationViewModel>();
            notifications.Add(notification);

            string json = JsonConvert.SerializeObject(notifications, Formatting.Indented);
            await FileIO.WriteTextAsync(DbFile, json);
        }

        public async Task UpdateAsync(NotificationViewModel notification, int index)
        {
            List<NotificationViewModel> notifications = JsonConvert.DeserializeObject<List<NotificationViewModel>>(await FileIO.ReadTextAsync(DbFile)) ?? new List<NotificationViewModel>();
            notifications[index] = notification;

            string json = JsonConvert.SerializeObject(notifications, Formatting.Indented);
            await FileIO.WriteTextAsync(DbFile, json);
        }

        public async Task DeleteAsync(NotificationViewModel notification)
        {
            // It's already deleted in the MViewModel notifications list, so why bother deleting again?
            string json = JsonConvert.SerializeObject(App.MViewModel.Notifications, Formatting.Indented);
            await FileIO.WriteTextAsync(DbFile, json);
        }

    }
}