using Rise.Data.ViewModels;
using Rise.Storage;
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

        public IFolder CurrentFolder { get; private set; }

        public FileBrowserPageViewModel()
        {
            FileBrowserHeaderViewModel = new();
        }
    }
}
