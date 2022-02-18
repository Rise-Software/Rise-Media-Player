using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rise.App.ViewModels
{
    public class DeviceViewModel : ViewModel
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public bool Online { get; set; }

    }
}
