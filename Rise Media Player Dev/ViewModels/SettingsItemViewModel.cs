using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rise.App.ViewModels
{
    public class SettingsItemViewModel
    {
        public string Title { get; set; }
        public string Icon { get; set; }
        public string Tag { get; set; }

        public SettingsItemViewModel(string title, string icon, string tag)
        {
            Title = title;
            Icon = icon;
            Tag = tag;
        }
    }
}
