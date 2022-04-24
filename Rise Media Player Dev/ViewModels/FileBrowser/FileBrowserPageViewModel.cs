using Rise.Data.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rise.App.ViewModels.FileBrowser
{
    public sealed class FileBrowserPageViewModel : ViewModel
    {
        public FileBrowserHeaderViewModel FileBrowserHeaderViewModel { get; }

        public FileBrowserPageViewModel()
        {
            FileBrowserHeaderViewModel = new();
        }
    }
}
