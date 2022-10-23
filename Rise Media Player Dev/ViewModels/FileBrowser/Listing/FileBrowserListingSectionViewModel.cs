using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Rise.App.Models;
using Rise.Common.Enums;
using Rise.Storage;
using System.Collections.ObjectModel;
using System.Linq;

namespace Rise.App.ViewModels.FileBrowser.Listing
{
    [ObservableObject]
    public sealed partial class FileBrowserListingSectionViewModel : IEnumerationDestination<IBaseStorage>
    {
        private readonly IMessenger _messenger;

        [ObservableProperty]
        private ObservableCollection<FileBrowserListingItemViewModel> _Items;

        [ObservableProperty]
        private string _SectionName;

        [ObservableProperty]
        private FileBrowserSectionType _SectionType;

        public FileBrowserListingSectionViewModel(IMessenger messenger, string sectionName, FileBrowserSectionType sectionType)
        {
            _messenger = messenger;
            _SectionName = sectionName;
            _SectionType = sectionType;
            _Items = new();
        }

        public void AddFromEnumeration(IBaseStorage enumeration)
        {
            var item = new FileBrowserListingItemViewModel(enumeration, _messenger, _SectionType);

            if (!Items.Contains(item))
                Items.Add(item);
        }
    }
}
