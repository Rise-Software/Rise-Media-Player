using Microsoft.Toolkit.Uwp.UI;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Rise.Common.Extensions
{
    public static class CollectionExtensions
    {
        public static bool MoveItemInCollection<T>(this Collection<T> collection, T item, int newIndex)
        {
            if (collection?.Remove(item) ?? false)
            {
                collection.Insert(newIndex, item);
                return true;
            }

            return false;
        }

        public static bool AddIfNotExists<T>(this ICollection<T> collection, T item)
        {
            if (!collection.Contains(item))
            {
                collection.Add(item);
                return true;
            }

            return false;
        }

        public static bool AddIfNotExists<T>(this AdvancedCollectionView collection, T item)
        {
            if (!collection.Contains(item))
            {
                collection.Add(item);
                return true;
            }

            return false;
        }
    }
}
