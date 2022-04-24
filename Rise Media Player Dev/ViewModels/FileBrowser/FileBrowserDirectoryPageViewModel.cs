using Rise.Data.ViewModels;
using Rise.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rise.App.ViewModels.FileBrowser
{
    public sealed class FileBrowserDirectoryPageViewModel : ViewModel
    {
        public IFolder CurrentFolder { get; private set; }
    }
}
