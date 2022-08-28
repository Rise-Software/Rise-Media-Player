using System;
using Newtonsoft.Json;
using Windows.UI.Xaml.Controls;

namespace Rise.Data.ViewModels
{
    public class WidgetViewModel : ViewModel
    {
        [JsonIgnore]
        private Guid _id;
        /// <summary>
        /// Gets or sets a unique identifier for the widget.
        /// </summary>
        public Guid Id
        {
            get => _id;
            set => Set(ref _id, value);
        }

        [JsonIgnore]
        private string _title;
        /// <summary>
        /// Gets or sets the widget's title.
        /// </summary>
        [JsonIgnore]
        public string Title
        {
            get => _title;
            set => Set(ref _title, value);
        }

        [JsonIgnore]
        private string _author;
        /// <summary>
        /// Gets or sets the widget's author.
        /// </summary>
        [JsonIgnore]
        public string Author
        {
            get => _author;
            set => Set(ref _author, value);
        }

        [JsonIgnore]
        private string _description;
        /// <summary>
        /// Gets or sets the widget's description.
        /// </summary>
        [JsonIgnore]
        public string Description
        {
            get => _description;
            set => Set(ref _description, value);
        }

        [JsonIgnore]
        private bool _enabled;
        /// <summary>
        /// Whether the widget is enabled.
        /// </summary>
        public bool Enabled
        {
            get => _enabled;
            set => Set(ref _enabled, value);
        }

        [JsonIgnore]
        private IconElement _icon;
        /// <summary>
        /// Gets or sets the icon for the widget.
        /// </summary>
        [JsonIgnore]
        public IconElement Icon
        {
            get => _icon;
            set => Set(ref _icon, value);
        }

        [JsonIgnore]
        private object _content;
        /// <summary>
        /// Gets or sets the contents of the widget, usually
        /// an XAML control.
        /// </summary>
        [JsonIgnore]
        public object Content
        {
            get => _content;
            set => Set(ref _content, value);
        }

        public WidgetViewModel()
        {
            _id = Guid.NewGuid();
        }

        public WidgetViewModel(Guid id)
        {
            _id = id;
        }
    }
}
