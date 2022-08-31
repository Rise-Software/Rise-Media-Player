using System;
using Rise.Data.ViewModels;
using Windows.UI.Xaml.Controls;

namespace Rise.App.Widgets
{
    /// <summary>
    /// Shows info about the app.
    /// </summary>
    public sealed class AppInfoWidget : WidgetViewModel
    {
        public static readonly Guid WidgetId = new("fc488476-55cd-4ebf-ac62-ade7bfe3d20b");

        public AppInfoWidget()
            : base(WidgetId)
        {
            Title = "AppInfo";
            Author = "Rise Software";
            Description = "WDAppInfo";
            Icon = new FontIcon { Glyph = "\uE946" };
        }
    }
}
