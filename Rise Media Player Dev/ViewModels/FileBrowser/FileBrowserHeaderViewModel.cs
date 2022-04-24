using Rise.Data.ViewModels;
using Rise.Storage;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rise.App.ViewModels.FileBrowser
{
    public sealed class FileBrowserHeaderViewModel : ViewModel
    {
        public ObservableCollection<BreadcrumbItemViewModel> Items { get; }

        private IFolder _CurrentFolder;
        public IFolder CurrentFolder
        {
            get => _CurrentFolder;
            set
            {
                if (_CurrentFolder != value)
                {
                    _CurrentFolder = value;
                    OnPropertyChanged(nameof(CurrentFolderName));
                }
            }
        }

        public string CurrentFolderName
        {
            get => _CurrentFolder.Name;
        }

        public FileBrowserHeaderViewModel()
        {
            Items = new();
        }
    }
}
