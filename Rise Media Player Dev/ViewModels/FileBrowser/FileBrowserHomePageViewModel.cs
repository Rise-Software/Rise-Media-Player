using Rise.Data.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rise.App.ViewModels.FileBrowser
{
    public sealed class FileBrowserHomePageViewModel : ViewModel
    {
        public async Task EnumerateDrivesAsync()
        {
            foreach (var item in DriveInfo)
        }
    }
}
