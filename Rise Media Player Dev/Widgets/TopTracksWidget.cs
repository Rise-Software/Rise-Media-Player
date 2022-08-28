using System;
using Rise.Data.ViewModels;
using Windows.UI.Xaml.Controls;

namespace Rise.App.Widgets
{
    public sealed class TopTracksWidget : WidgetViewModel
    {
        public static readonly Guid WidgetId = new("d771ffa2-405f-4fe1-8d84-93811348eb6a");

        public TopTracksWidget()
            : base(WidgetId)
        {
            Title = "TopTracks";
            Author = "Rise Software";
            Description = "WDTopTracks";
            Icon = new FontIcon { Glyph = "\uF49A" };
        }
    }
}
