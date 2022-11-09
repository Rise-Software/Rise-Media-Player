using Rise.Common.Enums;
using Rise.Data.ViewModels;
using System.ComponentModel;
using Windows.UI.Xaml;

namespace Rise.App.TemplateSelectors
{
    internal sealed class NavViewItemTemplateSelector : TemplateSelectorBase<NavViewItemViewModel>
    {
        public DataTemplate ItemTemplate { get; set; }
        public DataTemplate SubItemTemplate { get; set; }
        public DataTemplate HeaderTemplate { get; set; }
        public DataTemplate SeparatorTemplate { get; set; }

        protected sealed override DataTemplate SelectTemplateCore(NavViewItemViewModel item)
            => item.ItemType switch
            {
                NavViewItemType.Item => ItemTemplate,
                NavViewItemType.SubItem => SubItemTemplate,
                NavViewItemType.Header => HeaderTemplate,
                NavViewItemType.Separator => SeparatorTemplate,
                _ => throw new InvalidEnumArgumentException("The specified item type is invalid.", (int)item.ItemType, typeof(NavViewItemType)),
            };
    }
}
