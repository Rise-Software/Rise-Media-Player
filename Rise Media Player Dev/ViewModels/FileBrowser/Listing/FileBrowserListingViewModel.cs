using CommunityToolkit.Mvvm.Messaging;
using Rise.App.Models;
using Rise.Common.Constants;
using Rise.Common.Enums;
using Rise.Data.ViewModels;
using Rise.Storage;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Rise.App.ViewModels.FileBrowser.Listing
{
    public sealed class FileBrowserListingViewModel : ViewModel
    {
        private readonly IMessenger _messenger;
        private readonly FileBrowserEnumerationModel _enumerationModel;

        public ObservableCollection<FileBrowserListingSectionViewModel> Sections { get; }

        public FileBrowserListingViewModel(IMessenger messenger)
        {
            this._messenger = messenger;
            this.Sections = new();
            this._enumerationModel = GetModel();
        }

        public Task StartEnumerationAsync(IFolder folder, CancellationToken cancellationToken)
        {
            // Clear
            foreach (var item in Sections)
            {
                item.Items.Clear();
            }
            Sections.Clear();
            _enumerationModel.ResetSources();

            // Enumerate
            return _enumerationModel.EnumerateFolderAsync(folder, cancellationToken);
        }

        private FileBrowserEnumerationModel GetModel()
        {
            return new FileBrowserEnumerationModel(new EnumerationSource<IBaseStorage>[]
            {
                // Folders
                new(baseStorage => baseStorage is IFolder, () =>
                {
                    var section = new FileBrowserListingSectionViewModel(_messenger, "Folders", FileBrowserSectionType.Folders);
                    Sections.Insert(0, section);
                    return section;
                }),

                // Music
                new(baseStorage => baseStorage is IFile file && SupportedFileTypes.MusicFiles.Contains(file.Extension), () =>
                {
                    var section = new FileBrowserListingSectionViewModel(_messenger, "Music", FileBrowserSectionType.Music);
                    Sections.Insert(Math.Min(Sections.Count, 1), section);
                    return section;
                }),

                // Videos
                new(baseStorage => baseStorage is IFile file && SupportedFileTypes.VideoFiles.Contains(file.Extension), () =>
                {
                    var section = new FileBrowserListingSectionViewModel(_messenger, "Videos", FileBrowserSectionType.Videos);
                    Sections.Insert(Math.Min(Sections.Count, 2), section);
                    return section;
                })
            });
        }
    }
}
