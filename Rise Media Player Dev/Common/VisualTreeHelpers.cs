using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace RMP.App.Common
{
    public static class VisualTreeHelpers
    {
        /// <summary>
        /// Tries to find the specified visual child.
        /// </summary>
        /// <typeparam name="childItem">The kind of item to find.</typeparam>
        /// <param name="obj">Object where search will happen.</param>
        /// <returns>The item if it's found, null otherwise.</returns>
        public static childItem FindVisualChild<childItem>(this DependencyObject obj)
            where childItem : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);

                if (child != null && child is childItem item)
                {
                    return item;
                }
                else
                {
                    childItem childOfChild = child.FindVisualChild<childItem>();

                    if (childOfChild != null)
                    {
                        return childOfChild;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Tries to find all children of specified kind.
        /// </summary>
        /// <param name="parent">Where to find the item.</param>
        /// <param name="recurse">Whether or not the search should be recursive.</param>
        /// <returns>All child items found.</returns>
        public static IEnumerable<childItem> GetChildren<childItem>
            (this DependencyObject parent, bool recurse = false)
                where childItem : DependencyObject
        {
            if (parent != null)
            {
                int count = VisualTreeHelper.GetChildrenCount(parent);
                for (int i = 0; i < count; i++)
                {
                    // Retrieve child visual at specified index value.
                    DependencyObject child = VisualTreeHelper.GetChild(parent, i);

                    if (child != null && child is childItem item)
                    {
                        yield return item;

                        if (recurse)
                        {
                            foreach (var grandChild in child.GetChildren<childItem>(true))
                            {
                                yield return grandChild;
                            }
                        }
                    }
                }
            }
        }
    }
}
