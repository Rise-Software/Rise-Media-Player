using Newtonsoft.Json;
using Rise.App.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;

namespace Rise.App.DbControllers
{
    public class PlaylistsBackendController : BaseBackendController
    {
        public PlaylistsBackendController(string dbName) : base(dbName)
        {

        }

        public PlaylistsBackendController() : base("Playlists") { }

        public async Task<PlaylistViewModel> GetAsync(Guid id)
        {
            string text = await FileIO.ReadTextAsync(await ApplicationData.Current.LocalCacheFolder.CreateFileAsync($"Playlists.json", CreationCollisionOption.OpenIfExists));
            if (!string.IsNullOrWhiteSpace(text))
            {
                List<PlaylistViewModel> playlists = JsonConvert.DeserializeObject<List<PlaylistViewModel>>(text);
                return playlists.FirstOrDefault(p => p.Model.Id.Equals(id));
            }
            else
            {
                return null;
            }
        }

        public async Task<List<PlaylistViewModel>> GetAsync()
        {
            string text = await FileIO.ReadTextAsync(await ApplicationData.Current.LocalCacheFolder.CreateFileAsync($"Playlists.json", CreationCollisionOption.OpenIfExists));
            if (!string.IsNullOrWhiteSpace(text))
            {
                List<PlaylistViewModel> playlists = JsonConvert.DeserializeObject<List<PlaylistViewModel>>(text);
                return playlists;
            }
            else
            {
                return new List<PlaylistViewModel>();
            }
        }

        public async Task InsertAsync(PlaylistViewModel playlist)
        {
            List<PlaylistViewModel> playlists = JsonConvert.DeserializeObject<List<PlaylistViewModel>>(await FileIO.ReadTextAsync(await ApplicationData.Current.LocalCacheFolder.CreateFileAsync($"Playlists.json", CreationCollisionOption.OpenIfExists))) ?? new List<PlaylistViewModel>();
            playlists.Add(playlist);

            string json = JsonConvert.SerializeObject(playlists, Formatting.Indented);
            await FileIO.WriteTextAsync(await ApplicationData.Current.LocalCacheFolder.CreateFileAsync($"Playlists.json", CreationCollisionOption.OpenIfExists), json);
        }

        public async Task UpsertAsync(PlaylistViewModel playlist)
        {
            List<PlaylistViewModel> playlists = JsonConvert.DeserializeObject<List<PlaylistViewModel>>(await FileIO.ReadTextAsync(await ApplicationData.Current.LocalCacheFolder.CreateFileAsync($"Playlists.json", CreationCollisionOption.OpenIfExists))) ?? new List<PlaylistViewModel>();

            bool playlistExists = playlists.Any(p =>
            {
                if (p != null)
                {
                    return p.Model.Equals(playlist.Model);
                }
                else
                {
                    return false;
                }
            });

            if (playlistExists)
            {
                PlaylistViewModel item = playlists.FirstOrDefault(i => i.Model.Equals(playlist.Model));
                var oldIndex = playlists.IndexOf(item);
                playlists[oldIndex] = playlist;
            }
            else
            {
                await InsertAsync(playlist);
                return;
            }

            string json = JsonConvert.SerializeObject(playlists, Formatting.Indented);
            await FileIO.WriteTextAsync(await ApplicationData.Current.LocalCacheFolder.CreateFileAsync($"Playlists.json", CreationCollisionOption.OpenIfExists), json);
        }

        public async Task DeleteAsync(PlaylistViewModel playlist)
        {
            string json = JsonConvert.SerializeObject(App.MViewModel.Playlists, Formatting.Indented);
            await FileIO.WriteTextAsync(await ApplicationData.Current.LocalCacheFolder.CreateFileAsync($"Playlists.json", CreationCollisionOption.OpenIfExists), json);
        }

    }
}