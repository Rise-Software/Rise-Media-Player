using Microsoft.Toolkit.Uwp.UI;
using System.Collections.ObjectModel;

namespace Rise.App.Helpers
{
    public static class CollectionHelpers
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

        public static bool AddIfNotExists<T>(this Collection<T> collection, T item)
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
