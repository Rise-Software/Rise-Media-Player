using Rise.Common.Enums;

namespace Rise.Models
{
    public class Widget : DbObject
    {
        public string Title { get; set; }

        public string IconGlyph { get; set; }

        public string WidgetClassName { get; set; }

        public WidgetType WidgetType { get; set; }

        public bool ShowTitle { get; set; }

        public bool ShowIcon { get; set; }
    }
}
