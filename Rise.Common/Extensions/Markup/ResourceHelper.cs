﻿using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml.Markup;

namespace Rise.Common.Extensions.Markup
{
    /// <summary>
    /// A markup extension that gets a string from a resource.
    /// Can also be used outside of XAML through the static
    /// <see cref="GetString(string)"/> method.
    /// </summary>
    [MarkupExtensionReturnType(ReturnType = typeof(string))]
    public sealed class ResourceHelper : MarkupExtension
    {
        private static readonly ResourceLoader loader = ResourceLoader.GetForViewIndependentUse();

        /// <summary>
        /// Resource identifier to get the string from.
        /// </summary>
        public string Name { get; set; }

        protected override object ProvideValue()
        {
            return loader.GetString(Name);
        }

        /// <summary>
        /// Gets the string from the resource with the provided
        /// identifier.
        /// </summary>
        public static string GetString(string resource)
        {
            return loader.GetString(resource);
        }
    }
}
