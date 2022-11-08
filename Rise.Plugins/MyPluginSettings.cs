using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rise.Plugins
{
    internal class MyPluginSettings : PluginSettings
    {
        string AConfig {
            get => Get("the param");
            set { }
        }

    }
}
