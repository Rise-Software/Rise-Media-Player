using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Rise.App.Models;
using Rise.Common.Constants;
using Rise.Data.ViewModels;
using Rise.Storage;

namespace Rise.App.ViewModels.FileBrowser.Listing
{
    public sealed class FileBrowserListingViewModel : ViewModel
    {
        private FileBrowserEnumerationModel EnumerationModel { get; }

        public ObservableCollection<FileBrowserListingSectionViewModel> Sections { get; }

        public FileBrowserListingViewModel()
        {
            Sections = new();

            EnumerationModel = GetModel();
        }

        public Task StartEnumerationAsync(IFolder folder)
        {
            // Clear
            foreach (var item in Sections)
            {
                item.Items.Clear();
            }
            Sections.Clear();

            // Enumerate
            return EnumerationModel.EnumerateFolderAsync(folder);
        }

        private static FileBrowserEnumerationModel GetModel()
        {
            return new FileBrowserEnumerationModel(new EnumerationSource<IBaseStorage>[]
            {
                // Folders
                new(baseStorage => baseStorage is IFolder, () => new FileBrowserListingSectionViewModel()),

                // Music
                new(baseStorage => baseStorage is IFile file && SupportedFileTypes.MusicFiles.Contains(file.Extension), () => new FileBrowserListingSectionViewModel()),

                // Videos
                new(baseStorage => baseStorage is IFile file && SupportedFileTypes.VideoFiles.Contains(file.Extension), () => new FileBrowserListingSectionViewModel())
            });
        }
    }
}
