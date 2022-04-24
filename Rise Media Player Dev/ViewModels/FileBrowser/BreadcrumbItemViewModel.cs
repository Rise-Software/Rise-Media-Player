using System;

namespace Rise.App.ViewModels.FileBrowser
{
    public sealed class BreadcrumbItemViewModel
    {
        public int Index { get; }

        public Action<BreadcrumbItemViewModel?> NavigationAction { get; }

        public string SectionName { get; }

        public BreadcrumbItemViewModel(int index, Action<BreadcrumbItemViewModel?> navigationAction, string sectionName)
        {
            this.Index = index;
            this.NavigationAction = navigationAction;
            this.SectionName = sectionName;
        }
    }
}
