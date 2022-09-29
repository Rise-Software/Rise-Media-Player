using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Messaging;
using Rise.App.Models;
using Rise.Common.Constants;
using Rise.Data.ViewModels;
using Rise.Storage;

namespace Rise.App.ViewModels.FileBrowser.Listing
{
    public sealed class FileBrowserListingViewModel : ViewModel
    {
        private IMessenger Messenger { get; }

        private FileBrowserEnumerationModel EnumerationModel { get; }

        public ObservableCollection<FileBrowserListingSectionViewModel> Sections { get; }

        public FileBrowserListingViewModel(IMessenger messenger)
        {
            this.Messenger = messenger;
            this.Sections = new();

            this.EnumerationModel = GetModel();
        }

        public Task StartEnumerationAsync(IFolder folder, CancellationToken cancellationToken)
        {
            // Clear
            foreach (var item in Sections)
            {
                item.Items.Clear();
            }
            Sections.Clear();
            EnumerationModel.ResetSources();

            // Enumerate
            return EnumerationModel.EnumerateFolderAsync(folder, cancellationToken);
        }

        private FileBrowserEnumerationModel GetModel()
        {
            return new FileBrowserEnumerationModel(new EnumerationSource<IBaseStorage>[]
            {
                // Folders
                new(baseStorage => baseStorage is IFolder, () =>
                {
                    var section = new FileBrowserListingSectionViewModel(Messenger)
                    {
                        SectionName = "Folders"
                    };
                    Sections.Insert(0, section);
                    return section;
                }),

                // Music
                new(baseStorage => baseStorage is IFile file && SupportedFileTypes.MusicFiles.Contains(file.Extension), () =>
                {
                    var section = new FileBrowserListingSectionViewModel(Messenger)
                    {
                        SectionName = "Music"
                    };
                    Sections.Insert(Math.Min(Sections.Count, 1), section);
                    return section;
                }),

                // Videos
                new(baseStorage => baseStorage is IFile file && SupportedFileTypes.VideoFiles.Contains(file.Extension), () =>
                {
                    var section = new FileBrowserListingSectionViewModel(Messenger)
                    {
                        SectionName = "Videos"
                    };
                    Sections.Insert(Math.Min(Sections.Count, 2), section);
                    return section;
                })
            });
        }
    }
}
