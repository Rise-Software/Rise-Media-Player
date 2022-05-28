using Rise.Data.ViewModels;
using System.Collections.ObjectModel;
using System.IO;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;
using Rise.App.Messages.FileBrowser;
using Rise.App.Services;
using Rise.App.ViewModels.FileBrowser.Pages;
using Rise.Storage;

namespace Rise.App.ViewModels.FileBrowser
{
    public sealed class FileBrowserHeaderViewModel : ViewModel,
        IRecipient<FileBrowserNavigationRequestedMessage>,
        IRecipient<FileBrowserDirectoryNavigationRequestedMessage>
    {
        private IMessenger Messenger { get; }

        public ObservableCollection<FileBrowserBreadcrumbItemViewModel> Items { get; }
        
        private string? _CurrentLocation;
        public string? CurrentLocation
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

            Messenger.Register<FileBrowserNavigationRequestedMessage>(this);
            Messenger.Register<FileBrowserDirectoryNavigationRequestedMessage>(this);

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

        public void Receive(FileBrowserNavigationRequestedMessage message)
        {
            switch (message.Value)
            {
                case FileBrowserHomePageViewModel:
                    SetCurrentLocation(null);
                    break;

                case FileBrowserDirectoryPageViewModel directoryPageViewModel:
                    SetCurrentLocation(directoryPageViewModel.CurrentFolder);
                    break;
            }
        }

        public void Receive(FileBrowserDirectoryNavigationRequestedMessage message)
        {
            SetCurrentLocation(message.Value);
        }

        private void SetCurrentLocation(IFolder? folder)
        {
            Items.Clear();
            if (folder is null)
            {
                Items.Add(new("Home", null, null));
                CurrentLocation = "Home";
                return;
            }

            CurrentLocation = folder.Name;

            var path = string.Empty;
            foreach (var item in folder.Path.Split(Path.DirectorySeparatorChar))
            {
                path += $"{item}{Path.DirectorySeparatorChar}";
                Items.Add(new(item, new AsyncRelayCommand<FileBrowserBreadcrumbItemViewModel>(async item =>
                {
                    var storageService = Ioc.Default.GetRequiredService<IStorageService>(); // TODO: Move it somewhere else?
                    var folderToNavigate = await storageService.GetFolderAsync(item.Path);

                    Messenger.Send(new FileBrowserDirectoryNavigationRequestedMessage(folderToNavigate));
                }), path));
            }
        }
    }
}
