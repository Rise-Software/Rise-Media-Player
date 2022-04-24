using Rise.Data.ViewModels;
using Rise.Storage;

namespace Rise.App.ViewModels.FileBrowser
{
    public sealed class FileBrowserDriveItemViewModel : ViewModel
    {
        private readonly IFolder _driveFolder;

        public FileBrowserDriveItemViewModel(IFolder driveFolder)
        {
            this._driveFolder = driveFolder;
        }
    }
}
