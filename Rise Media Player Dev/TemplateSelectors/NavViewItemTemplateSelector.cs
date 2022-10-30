using Rise.Common.Enums;
using Rise.Common.Extensions;
using Rise.Data.ViewModels;
using System;
using Windows.UI.Xaml;

namespace Rise.App.TemplateSelectors
{
    internal sealed class NavViewItemTemplateSelector : TemplateSelectorBase<NavViewItemViewModel>
    {
        public DataTemplate GlyphIconTemplate { get; set; }
        public DataTemplate AssetIconTemplate { get; set; }
        public DataTemplate HeaderTemplate { get; set; }
        public DataTemplate SeparatorTemplate { get; set; }

        protected sealed override DataTemplate SelectTemplateCore(NavViewItemViewModel item)
        {
            switch (item.ItemType)
            {
                case NavViewItemType.Header:
                    return HeaderTemplate;

                case NavViewItemType.Item:
                    if (item.Icon.IsValidUri(UriKind.Absolute))
                    {
                        return AssetIconTemplate;
                    }

                    return GlyphIconTemplate;

                case NavViewItemType.Separator:
                    return SeparatorTemplate;

                default:
                    return null;
            }
        }
    }
}
