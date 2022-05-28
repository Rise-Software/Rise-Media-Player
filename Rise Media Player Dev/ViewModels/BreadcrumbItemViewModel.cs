using Microsoft.Toolkit.Mvvm.Input;

namespace Rise.App.ViewModels
{
    public class BreadcrumbItemViewModel
    {
        public string SectionName { get; }

        public IRelayCommand? ItemClickedCommand { get; }

        public BreadcrumbItemViewModel(string sectionName, IRelayCommand? itemClickedCommand)
        {
            this.SectionName = sectionName;
            this.ItemClickedCommand = itemClickedCommand;
        }
    }
}
