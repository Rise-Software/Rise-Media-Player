using Microsoft.Toolkit.Uwp.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Rise.Common.Extensions
{
    public static class CollectionExtensions
    {
        /// <summary>
        /// Move item at oldIndex to newIndex.
        /// </summary>
        public static void Move<T>(this IList<T> list, int oldIndex, int newIndex)
        {
            T item = list[oldIndex];

            list.RemoveAt(oldIndex);
            list.Insert(newIndex, item);
        }

        /// <summary>
        /// Moves a set of items to the end of the list.
        /// </summary>
        /// <param name="items">Items to move.</param>
        public static void MoveItemsToEnd<T>(this IList<T> list, T[] items)
        {
            foreach (var item in items)
            {
                int moveFrom = list.IndexOf(item);
                list.Move(moveFrom, list.Count - 1);
            }
        }

        /// <summary>
        /// Moves the items between <paramref name="startIndex"/> and
        /// <paramref name="endIndex"/> (inclusive) to the end of the list.
        /// </summary>
        public static void MoveRangeToEnd<T>(this IList<T> list,
            int startIndex, int endIndex)
        {
            if (startIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(startIndex));

            if (endIndex > list.Count - 1)
                throw new ArgumentOutOfRangeException(nameof(endIndex));

            for (int i = startIndex; i <= endIndex; i++)
            {
                list.Move(startIndex, list.Count - 1);
            }
        }

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
