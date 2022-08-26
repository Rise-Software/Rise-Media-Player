using Newtonsoft.Json;
using Rise.App.UserControls;
using Rise.Common.Enums;
using Rise.Data.ViewModels;
using Rise.Models;

namespace Rise.App.ViewModels
{
    public class WidgetViewModel : ViewModel<Widget>
    {
        [JsonIgnore]
        public string Title
        {
            get => Model.Title;
            set
            {
                if (Model.Title != value)
                {
                    Model.Title = value;
                    OnPropertyChanged(nameof(Title));
                }
            }
        }

        [JsonIgnore]
        public string IconGlyph
        {
            get => Model.IconGlyph;
            set
            {
                if (Model.IconGlyph != value)
                {
                    Model.IconGlyph = value;
                    OnPropertyChanged(nameof(IconGlyph));
                }
            }
        }

        [JsonIgnore]
        public string WidgetClassName
        {
            get => Model.WidgetClassName;
            set
            {
                if (Model.WidgetClassName != value)
                {
                    Model.WidgetClassName = value;
                    OnPropertyChanged(nameof(WidgetClassName));
                }
            }
        }

        [JsonIgnore]
        public WidgetType WidgetType
        {
            get => Model.WidgetType;
            set
            {
                if (Model.WidgetType != value)
                {
                    Model.WidgetType = value;
                    OnPropertyChanged(nameof(WidgetType));
                }
            }
        }

        [JsonIgnore]
        public bool ShowTitle
        {
            get => Model.ShowTitle;
            set
            {
                if (Model.ShowTitle != value)
                {
                    Model.ShowTitle = value;
                    OnPropertyChanged(nameof(ShowTitle));
                }
            }
        }

        [JsonIgnore]
        public bool ShowIcon
        {
            get => Model.ShowIcon;
            set
            {
                if (Model.ShowIcon != value)
                {
                    Model.ShowIcon = value;
                    OnPropertyChanged(nameof(ShowIcon));
                }
            }
        }

        private object _content;

        [JsonIgnore]
        public object Content
        {
            get => DetectContentFromType(WidgetType);
            set
            {
                if (_content != value)
                {
                    _content = value;
                    OnPropertyChanged(nameof(Content));
                }
            }
        }

        private object DetectContentFromType(WidgetType type)
        {
            return type switch
            {
                WidgetType.TopTracks => new TopTracksWidgetContentControl(),
                WidgetType.RecentlyPlayed => new RecentlyPlayedWidgetContentControl(),
                WidgetType.AppInfo => new AppInfoWidgetContentControl(),
                _ => _content,
            };
        }
    }
}
