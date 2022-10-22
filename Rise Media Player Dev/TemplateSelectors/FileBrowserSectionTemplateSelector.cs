using Rise.App.ViewModels.FileBrowser.Listing;
using Rise.Common.Enums;
using Windows.UI.Xaml;

namespace Rise.App.TemplateSelectors
{
    internal sealed class FileBrowserSectionTemplateSelector : TemplateSelectorBase<FileBrowserListingItemViewModel>
    {
        public DataTemplate FoldersSectionDataTemplate { get; set; }

        public DataTemplate MusicSectionDataTemplate { get; set; }

        public DataTemplate VideosSectionDataTemplate { get; set; }

        /// <inheritdoc/>
        protected override DataTemplate SelectTemplateCore(FileBrowserListingItemViewModel item)
        {
            return item.SectionType switch
            {
                FileBrowserSectionType.Folders => FoldersSectionDataTemplate,
                FileBrowserSectionType.Music => MusicSectionDataTemplate,
                FileBrowserSectionType.Videos => VideosSectionDataTemplate,
                _ => base.SelectTemplateCore(item)
            };
        }
    }
}
