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
    public sealed class PlaylistsBackendController : BaseBackendController
    {
        public PlaylistsBackendController() : base("Playlists") { }

        public async Task<PlaylistViewModel> GetAsync(Guid id)
        {
            string text = await FileIO.ReadTextAsync(DbFile);

            if (string.IsNullOrWhiteSpace(text))
                return null;

            var playlists = JsonConvert.DeserializeObject<List<PlaylistViewModel>>(text);
            return playlists.FirstOrDefault(p => p.Model.Id.Equals(id));
        }

        public async Task<IEnumerable<PlaylistViewModel>> GetAsync()
        {
            string text = await FileIO.ReadTextAsync(DbFile);

            if (!string.IsNullOrWhiteSpace(text))
                return JsonConvert.DeserializeObject<IEnumerable<PlaylistViewModel>>(text);

            return Enumerable.Empty<PlaylistViewModel>();
        }

        public async Task InsertAsync(PlaylistViewModel PlaylistViewModel)
        {
            var text = await FileIO.ReadTextAsync(DbFile);
            var playlists = JsonConvert.DeserializeObject<List<PlaylistViewModel>>(text) ?? new List<PlaylistViewModel>();
            playlists.Add(PlaylistViewModel);

            string json = JsonConvert.SerializeObject(playlists, Formatting.Indented);
            await FileIO.WriteTextAsync(DbFile, json);
        }

        public async Task UpsertAsync(PlaylistViewModel PlaylistViewModel)
        {
            var text = await FileIO.ReadTextAsync(DbFile);
            var playlists = JsonConvert.DeserializeObject<List<PlaylistViewModel>>(text) ?? new List<PlaylistViewModel>();

            var item = playlists.FirstOrDefault(i => i.Equals(PlaylistViewModel));
            if (item != null)
            {
                int oldIndex = playlists.IndexOf(item);
                playlists[oldIndex] = PlaylistViewModel;
            }
            else
            {
                await InsertAsync(PlaylistViewModel);
                return;
            }

            string json = JsonConvert.SerializeObject(playlists, Formatting.Indented);
            await FileIO.WriteTextAsync(DbFile, json);
        }

        public async Task DeleteAsync(PlaylistViewModel item)
        {
            var text = await FileIO.ReadTextAsync(DbFile);
            var list = JsonConvert.DeserializeObject<List<PlaylistViewModel>>(text) ?? new List<PlaylistViewModel>();

            _ = list.Remove(item);

            string json = JsonConvert.SerializeObject(list, Formatting.Indented);
            await FileIO.WriteTextAsync(DbFile, json);
        }
    }
}