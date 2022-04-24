using Rise.Data.ViewModels;
using System.Collections.ObjectModel;
using Microsoft.Toolkit.Mvvm.Input;
using System.Threading.Tasks;
using Rise.App.Services;
using Microsoft.Toolkit.Mvvm.DependencyInjection;

namespace Rise.App.ViewModels.FileBrowser
{
    public sealed class FileBrowserHeaderViewModel : ViewModel
    {
        private IFileExplorerService FileExplorerService { get; } = Ioc.Default.GetRequiredService<IFileExplorerService>();

        public ObservableCollection<BreadcrumbItemViewModel> Items { get; }
        
        private string _CurrentLocation;
        public string CurrentLocation
        {
            get => _CurrentLocation;
            set => Set(ref _CurrentLocation, value);
        }

        public IRelayCommand GoBackCommand { get; }

        public IRelayCommand PinToSidebarCommand { get; }

        public IRelayCommand OpenInFileExplorerCommand { get; }

        public FileBrowserHeaderViewModel()
        {
            Items = new();

            GoBackCommand = new RelayCommand(GoBack);
            PinToSidebarCommand = new RelayCommand(PinToSidebar);
            OpenInFileExplorerCommand = new AsyncRelayCommand(OpenInFileExplorer);
        }

        private void GoBack()
        {
        }

        private void PinToSidebar()
        {
        }

        private async Task OpenInFileExplorer()
        {
        }
    }
}
