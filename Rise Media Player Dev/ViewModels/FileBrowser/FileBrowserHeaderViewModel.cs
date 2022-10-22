using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Rise.App.Messages.FileBrowser;
using Rise.App.Services;
using Rise.App.ViewModels.FileBrowser.Pages;
using Rise.Storage;
using System.Collections.ObjectModel;
using System.IO;

namespace Rise.App.ViewModels.FileBrowser
{
    [ObservableObject]
    public sealed partial class FileBrowserHeaderViewModel : IRecipient<FileBrowserNavigationRequestedMessage>, IRecipient<FileBrowserDirectoryNavigationRequestedMessage>
    {
        private readonly IMessenger _messenger;
        private IFolder? _currentFolder;

        public ObservableCollection<FileBrowserBreadcrumbItemViewModel> Items { get; }

        [ObservableProperty]
        private string? _CurrentLocation;
        
        [ObservableProperty]
        private bool _CanPinToSidebar;
       
        [ObservableProperty]
        private bool _CanOpenInFileExplorer;

        [ObservableProperty]
        private bool _CanGoBack;

        public FileBrowserHeaderViewModel(IMessenger messenger)
        {
            this._messenger = messenger;
            this.Items = new();

            _messenger.Register<FileBrowserNavigationRequestedMessage>(this);
            _messenger.Register<FileBrowserDirectoryNavigationRequestedMessage>(this);
        }

        [RelayCommand]
        private void GoBack()
        {
            if (Items.Count == 1)
            {
                // Root drive, go to homepage
                _messenger.Send(new FileBrowserNavigationRequestedMessage(FileBrowserHomePageViewModel.GetOrCreate(_messenger)));
            }
            else
            {
                // Execute breadcrumb item command (that is a hack and could be done better, but it works :D)
                var breadcrumbItem = Items[Items.Count - 2];
                breadcrumbItem.ItemClickedCommand?.Execute(breadcrumbItem);
            }
        }

        [RelayCommand]
        private void PinToSidebar()
        {
        }

        [RelayCommand]
        private void OpenInFileExplorer()
        {
            _messenger.Send(new OpenInFileExplorerMessage(_currentFolder));
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
            CurrentLocation = folder?.Name ?? "Home";
            _currentFolder = folder;

            Items.Clear();
            if (folder is null)
            {
                Items.Add(new("Home", null, null));
                CanOpenInFileExplorer = false;
                CanGoBack = false;
                return;
            }

            CanOpenInFileExplorer = true;
            CanGoBack = true;
            var path = string.Empty;

            foreach (var item in folder.Path.Split(Path.DirectorySeparatorChar))
            {
                if (string.IsNullOrEmpty(item))
                    continue; // Will trigger when attempting to split root path, e.g.: "C:\\"

                path += $"{item}{Path.DirectorySeparatorChar}";
                Items.Add(new(item, new AsyncRelayCommand<FileBrowserBreadcrumbItemViewModel>(async x =>
                {
                    if (x?.Path is null)
                        return;

                    var storageService = Ioc.Default.GetRequiredService<IStorageService>(); // TODO: Move it somewhere else?
                    var folderToNavigate = await storageService.GetFolderAsync(x.Path);
                    if (folderToNavigate is null)
                        return;

                    _messenger.Send(new FileBrowserDirectoryNavigationRequestedMessage(folderToNavigate));
                }), path));
            }
        }
    }
}
