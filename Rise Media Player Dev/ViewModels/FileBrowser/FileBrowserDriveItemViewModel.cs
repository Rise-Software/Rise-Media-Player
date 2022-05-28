﻿using System.Threading.Tasks;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;
using Rise.App.Messages.FileBrowser;
using Rise.App.ViewModels.FileBrowser.Pages;
using Rise.Data.ViewModels;
using Rise.Storage.Devices;

namespace Rise.App.ViewModels.FileBrowser
{
    public sealed class FileBrowserDriveItemViewModel : ViewModel
    {
        private IMessenger Messenger { get; }

        public IDrive Drive { get; }

        public string Name { get; }

        public IAsyncRelayCommand OpenDriveCommand { get; }

        public FileBrowserDriveItemViewModel(IDrive drive, IMessenger messenger)
        {
            this.Messenger = messenger;
            this.Drive = drive;
            this.Name = drive.Name;

            OpenDriveCommand = new AsyncRelayCommand(OpenDrive);
        }

        private async Task OpenDrive()
        {
            var driveFolder = await Drive.GetRootFolderAsync();
            Messenger.Send(new FileBrowserNavigationRequestedMessage(new FileBrowserDirectoryPageViewModel(Messenger, driveFolder)));
        }
    }
}
