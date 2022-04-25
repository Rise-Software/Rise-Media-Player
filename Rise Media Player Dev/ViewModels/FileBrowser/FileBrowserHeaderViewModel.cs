using Rise.Data.ViewModels;
using System.Collections.ObjectModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;
using Rise.App.Messages.FileBrowser;

namespace Rise.App.ViewModels.FileBrowser
{
    public sealed class FileBrowserHeaderViewModel : ViewModel
    {
        private IMessenger Messenger { get; }

        public ObservableCollection<BreadcrumbItemViewModel> Items { get; }
        
        private string _CurrentLocation;
        public string CurrentLocation
        {
            get => _CurrentLocation;
            set => Set(ref _CurrentLocation, value);
        }

        private bool _CanPinToSidebar;
        public bool CanPinToSidebar
        {
            get => _CanPinToSidebar;
            set => Set(ref _CanPinToSidebar, value);
        }

        private bool _CanOpenInFileExplorer;
        public bool CanOpenInFileExplorer
        {
            get => _CanOpenInFileExplorer;
            set => Set(ref _CanOpenInFileExplorer, value);
        }

        public IRelayCommand GoBackCommand { get; }

        public IRelayCommand PinToSidebarCommand { get; }

        public IRelayCommand OpenInFileExplorerCommand { get; }

        public FileBrowserHeaderViewModel(IMessenger messenger)
        {
            this.Messenger = messenger;
            this.Items = new();

            GoBackCommand = new RelayCommand(GoBack);
            PinToSidebarCommand = new RelayCommand(PinToSidebar);
            OpenInFileExplorerCommand = new RelayCommand(OpenInFileExplorer);
        }

        private void GoBack()
        {
        }

        private void PinToSidebar()
        {
        }

        private void OpenInFileExplorer()
        {
            Messenger.Send(new OpenInFileExplorerMessage(null));
        }
    }
}
