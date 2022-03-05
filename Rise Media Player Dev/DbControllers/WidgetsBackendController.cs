using Newtonsoft.Json;
using Rise.App.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;

namespace Rise.App.DbControllers
{
    public class WidgetsBackendController : BaseBackendController
    {
        public WidgetsBackendController(string dbName) : base(dbName)
        {

        }

        public WidgetsBackendController() : base("Widgets") { }

        public async Task<WidgetViewModel> GetAsync(Guid id)
        {
            string text = await FileIO.ReadTextAsync(await ApplicationData.Current.LocalCacheFolder.CreateFileAsync($"Widgets.json", CreationCollisionOption.OpenIfExists));
            if (!string.IsNullOrWhiteSpace(text))
            {
                ObservableCollection<WidgetViewModel> widgets = JsonConvert.DeserializeObject<ObservableCollection<WidgetViewModel>>(text);
                return widgets.FirstOrDefault(w => w.Model.Id.Equals(id));
            }
            else
            {
                return null;
            }
        }

        public async Task<ObservableCollection<WidgetViewModel>> GetAsync()
        {
            string text = await FileIO.ReadTextAsync(await ApplicationData.Current.LocalCacheFolder.CreateFileAsync($"Widgets.json", CreationCollisionOption.OpenIfExists));
            if (!string.IsNullOrWhiteSpace(text))
            {
                ObservableCollection<WidgetViewModel> playlists = JsonConvert.DeserializeObject<ObservableCollection<WidgetViewModel>>(text);
                return playlists;
            }
            else
            {
                return new ObservableCollection<WidgetViewModel>();
            }
        }

        public async Task InsertAsync(WidgetViewModel playlist)
        {
            Collection<WidgetViewModel> widgets = JsonConvert.DeserializeObject<Collection<WidgetViewModel>>(await FileIO.ReadTextAsync(await ApplicationData.Current.LocalCacheFolder.CreateFileAsync($"Widgets.json", CreationCollisionOption.OpenIfExists))) ?? new Collection<WidgetViewModel>();
            widgets.Add(widget);

            string json = JsonConvert.SerializeObject(playlists, Formatting.Indented);
            await FileIO.WriteTextAsync(await ApplicationData.Current.LocalCacheFolder.CreateFileAsync($"Playlists.json", CreationCollisionOption.OpenIfExists), json);
        }

        public async Task UpsertAsync(WidgetViewModel widget)
        {
            Collection<WidgetViewModel> widgets = JsonConvert.DeserializeObject<Collection<WidgetViewModel>>(await FileIO.ReadTextAsync(await ApplicationData.Current.LocalCacheFolder.CreateFileAsync($"Widgets.json", CreationCollisionOption.OpenIfExists))) ?? new Collection<WidgetViewModel>();

            bool widgetExists = widgets.Any(p =>
            {
                if (p != null)
                {
                    return p.Model.Equals(widget.Model);
                }
                else
                {
                    return false;
                }
            });

            if (widgetExists)
            {
                WidgetViewModel item = widgets.FirstOrDefault(i => i.Model.Equals(widget.Model));
                var oldIndex = widgets.IndexOf(item);
                widgets[oldIndex] = widget;
            }
            else
            {
                await InsertAsync(widget);
                return;
            }

            string json = JsonConvert.SerializeObject(widgets, Formatting.Indented);
            await FileIO.WriteTextAsync(await ApplicationData.Current.LocalCacheFolder.CreateFileAsync($"Widgets.json", CreationCollisionOption.OpenIfExists), json);
        }

        public async Task DeleteAsync(WidgetViewModel playlist)
        {
            string json = JsonConvert.SerializeObject(App.MViewModel.Widgets, Formatting.Indented);
            await FileIO.WriteTextAsync(await ApplicationData.Current.LocalCacheFolder.CreateFileAsync($"Widgets.json", CreationCollisionOption.OpenIfExists), json);
        }

    }
}