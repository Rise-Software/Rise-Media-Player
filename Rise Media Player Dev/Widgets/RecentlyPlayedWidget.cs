using System;
using Rise.Data.ViewModels;
using Windows.UI.Xaml.Controls;

namespace Rise.App.Widgets
{
    public sealed class RecentlyPlayedWidget : WidgetViewModel
    {
        public static readonly Guid WidgetId = new("66f0e86d-4bf5-4be2-bdb7-14d9d1418440");

        public RecentlyPlayedWidget()
            : base(WidgetId)
        {
            Title = "RecentlyPlayed";
            Author = "Rise Software";
            Description = "WDRecentlyPlayed";
            Icon = new FontIcon { Glyph = "\uE823" };
        }
    }
}
