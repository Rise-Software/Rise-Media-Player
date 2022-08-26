using Rise.App.ViewModels;
using Rise.Common.Enums;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Rise.App.Dialogs
{
    public sealed partial class WidgetsDialogContent : Page
    {
        public WidgetsDialogContent()
        {
            InitializeComponent();

            TopTracksToggleSwitch.IsOn = App.MViewModel.Widgets.Any(i => i.WidgetType == WidgetType.TopTracks);
            RecentlyPlayedToggleSwitch.IsOn = App.MViewModel.Widgets.Any(i => i.WidgetType == WidgetType.RecentlyPlayed);
            AppInfoToggleSwitch.IsOn = App.MViewModel.Widgets.Any(i => i.WidgetType == WidgetType.AppInfo);
        }

        private async void AppInfoToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            var item = App.MViewModel.Widgets.FirstOrDefault(w => w.WidgetType == WidgetType.AppInfo);

            if (AppInfoToggleSwitch.IsOn)
            {
                var item1 = new WidgetViewModel()
                {
                    Model = new()
                    {
                        Title = "App Info",
                        IconGlyph = "\uE946",
                        WidgetType = WidgetType.AppInfo,
                        ShowIcon = true,
                        ShowTitle = true
                    }
                };

                if (!App.MViewModel.Widgets.Contains(item))
                {
                    App.MViewModel.Widgets.Add(item1);

                    await App.WBackendController.AddOrUpdateAsync(item1);
                }
            }
            else
            {
                _ = App.MViewModel.Widgets.Remove(item);

                await App.WBackendController.DeleteAsync(item);
            }
        }

        private async void RecentlyPlayedToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            var item = App.MViewModel.Widgets.FirstOrDefault(w => w.WidgetType == WidgetType.RecentlyPlayed);

            if (RecentlyPlayedToggleSwitch.IsOn)
            {
                var item1 = new WidgetViewModel()
                {
                    Model = new()
                    {
                        Title = "Recently Played",
                        IconGlyph = "\uE823",
                        WidgetType = WidgetType.RecentlyPlayed,
                        ShowIcon = true,
                        ShowTitle = true
                    }
                };

                if (!App.MViewModel.Widgets.Contains(item))
                {
                    App.MViewModel.Widgets.Add(item1);

                    await App.WBackendController.AddOrUpdateAsync(item1);
                }
            }
            else
            {
                _ = App.MViewModel.Widgets.Remove(item);

                await App.WBackendController.DeleteAsync(item);
            }
        }

        private async void TopTracksToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            var item = App.MViewModel.Widgets.FirstOrDefault(w => w.WidgetType == WidgetType.TopTracks);

            if (TopTracksToggleSwitch.IsOn)
            {
                var item1 = new WidgetViewModel()
                {
                    Model = new()
                    {
                        Title = "Top Tracks",
                        IconGlyph = "\uF49A",
                        WidgetType = WidgetType.TopTracks,
                        ShowIcon = true,
                        ShowTitle = true
                    }
                };

                if (!App.MViewModel.Widgets.Contains(item))
                {
                    App.MViewModel.Widgets.Add(item1);

                    await App.WBackendController.AddOrUpdateAsync(item1);
                }
            } else
            {
                _ = App.MViewModel.Widgets.Remove(item);

                await App.WBackendController.DeleteAsync(item);
            }
        }
    }
}
