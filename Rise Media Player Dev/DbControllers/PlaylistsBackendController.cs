using Newtonsoft.Json;
using Rise.App.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
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

        public async Task<ObservableCollection<PlaylistViewModel>> GetAsync()
        {
            string text = await FileIO.ReadTextAsync(await ApplicationData.Current.LocalCacheFolder.CreateFileAsync($"Playlists.json", CreationCollisionOption.OpenIfExists));
            if (!string.IsNullOrWhiteSpace(text))
            {
                ObservableCollection<PlaylistViewModel> playlists = JsonConvert.DeserializeObject<ObservableCollection<PlaylistViewModel>>(await FileIO.ReadTextAsync(DbFile));
                return playlists;
            } else
            {
                return new ObservableCollection<PlaylistViewModel>();
            }
        }

        public async Task InsertAsync(PlaylistViewModel playlist)
        {
            Collection<PlaylistViewModel> playlists = JsonConvert.DeserializeObject<Collection<PlaylistViewModel>>(await FileIO.ReadTextAsync(DbFile)) ?? new Collection<PlaylistViewModel>();
            playlists.Add(playlist);

            string json = JsonConvert.SerializeObject(playlists, Formatting.Indented);
            await FileIO.WriteTextAsync(DbFile, json);
        }

        public async Task UpdateAsync(PlaylistViewModel playlist, int index)
        {
            Collection<PlaylistViewModel> playlists = JsonConvert.DeserializeObject<Collection<PlaylistViewModel>>(await FileIO.ReadTextAsync(DbFile)) ?? new Collection<PlaylistViewModel>();
            playlists[index] = playlist;

            string json = JsonConvert.SerializeObject(playlists, Formatting.Indented);
            await FileIO.WriteTextAsync(DbFile, json);
        }

        public async Task DeleteAsync(PlaylistViewModel playlist)
        {
            string json = JsonConvert.SerializeObject(App.MViewModel.Playlists, Formatting.Indented);
            await FileIO.WriteTextAsync(DbFile, json);
        }

    }
}