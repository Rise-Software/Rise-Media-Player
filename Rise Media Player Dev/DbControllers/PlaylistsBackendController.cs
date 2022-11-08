using Newtonsoft.Json;
using Rise.App.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;

namespace Rise.App.DbControllers
{
    public sealed class PlaylistsBackendController : BaseBackendController
    {
        private MainViewModel ViewModel => App.MViewModel;
        public PlaylistsBackendController() : base("Playlists") { }

        public async Task<PlaylistViewModel> GetAsync(Guid id)
        {
            string text = await FileIO.ReadTextAsync(DbFile);
            if (!string.IsNullOrWhiteSpace(text))
            {
                var playlists = JsonConvert.DeserializeObject<List<PlaylistViewModel>>(text);
                return playlists.FirstOrDefault(p => p.Model.Id.Equals(id));
            }

            return null;
        }

        public async Task<List<PlaylistViewModel>> GetAsync()
        {
            string text = await FileIO.ReadTextAsync(DbFile);
            if (!string.IsNullOrWhiteSpace(text))
            {
                var playlists = JsonConvert.DeserializeObject<List<PlaylistViewModel>>(text);
                return playlists;
            }

            return new List<PlaylistViewModel>();
        }

        public async Task InsertAsync(PlaylistViewModel playlist)
        {
            var playlists = JsonConvert.DeserializeObject<List<PlaylistViewModel>>(await FileIO.ReadTextAsync(DbFile)) ?? new List<PlaylistViewModel>();
            playlists.Add(playlist);

            string json = JsonConvert.SerializeObject(playlists, Formatting.Indented);
            await FileIO.WriteTextAsync(DbFile, json);
        }

        public async Task UpsertAsync(PlaylistViewModel playlist)
        {
            var playlists = JsonConvert.DeserializeObject<List<PlaylistViewModel>>(await FileIO.ReadTextAsync(DbFile)) ?? new List<PlaylistViewModel>();

            var item = playlists.FirstOrDefault(i => i.Model.Equals(playlist.Model));
            if (item != null)
            {
                int oldIndex = playlists.IndexOf(item);
                playlists[oldIndex] = playlist;
            }
            else
            {
                await InsertAsync(playlist);
                return;
            }

            string json = JsonConvert.SerializeObject(playlists, Formatting.Indented);
            await FileIO.WriteTextAsync(DbFile, json);
        }

        public async Task DeleteAsync(PlaylistViewModel playlist)
        {
            string json = JsonConvert.SerializeObject(ViewModel.Playlists, Formatting.Indented);
            await FileIO.WriteTextAsync(DbFile, json);
        }
    }
}