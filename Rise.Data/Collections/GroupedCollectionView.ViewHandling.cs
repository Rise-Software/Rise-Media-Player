using Rise.Common.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml.Data;

namespace Rise.Data.Collections;

public sealed partial class GroupedCollectionView
{
    public object this[int index]
    {
        get => _view[index];
        set
        {
            _view[index] = value;

            RemoveItemFromGroup(_view[index]);
            AddItemToGroup(_view[index]);

            OnVectorChanged(CollectionChange.ItemChanged, (uint)index);
        }
    }

    public int Count => _view.Count;
    public bool IsReadOnly => _source.IsReadOnly;

    public int IndexOf(object item)
        => _view.IndexOf(item);

    public void Add(object item)
        => _source.Add(item);

    public void Insert(int index, object item)
        => _source.Insert(index, item);

    public bool Remove(object item)
    {
        int index = _source.IndexOf(item);
        if (index > -1)
        {
            _source.RemoveAt(index);
            return true;
        }

        return false;
    }

    public void RemoveAt(int index)
        => Remove(_view[index]);

    public void Clear()
        => _source.Clear();

    public bool Contains(object item)
        => _view.Contains(item);

    public void CopyTo(object[] array, int arrayIndex)
        => _view.CopyTo(array, arrayIndex);

    public IEnumerator<object> GetEnumerator() => _view.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => _view.GetEnumerator();

    // View handling
    private void RemoveFromView(int itemIndex)
    {
        var item = _view[itemIndex];

        _view.RemoveAt(itemIndex);
        RemoveItemFromGroup(item);

        if (itemIndex <= CurrentPosition)
            CurrentPosition--;

        OnVectorChanged(CollectionChange.ItemRemoved, (uint)itemIndex);
    }

    private bool OnItemAdded(int newStartingIndex, object newItem, int? viewIndex = null)
    {
        if (_filter != null && !_filter(newItem))
            return false;

        int newIndex = _view.Count;
        if (_sortDescriptions.Any())
        {
            newIndex = _view.BinarySearch(newItem, this);
            if (newIndex < 0)
                newIndex = ~newIndex;
        }
        else if (_filter != null)
        {
            if (newStartingIndex == 0 || _view.Count == 0)
                newIndex = 0;
            else if (newStartingIndex == _source.Count - 1)
                newIndex = _view.Count - 1;
            else if (viewIndex.HasValue)
                newIndex = viewIndex.Value;
            else
            {
                for (int i = 0, j = 0; i < _source.Count; i++)
                {
                    if (i == newStartingIndex)
                    {
                        newIndex = j;
                        break;
                    }

                    if (_view[j] == _source[i])
                        j++;
                }
            }
        }

        _view.Insert(newIndex, newItem);
        AddItemToGroup(newItem);

        if (newIndex <= CurrentPosition)
            CurrentPosition++;

        OnVectorChanged(CollectionChange.ItemInserted, (uint)newIndex);
        return true;
    }

    private void OnItemRemoved(int oldStartingIndex, object oldItem)
    {
        if (_filter != null && !_filter(oldItem))
            return;

        if (oldStartingIndex < 0 || oldStartingIndex >= _view.Count || !Equals(_view[oldStartingIndex], oldItem))
            oldStartingIndex = _view.IndexOf(oldItem);

        if (oldStartingIndex < 0)
            return;

        RemoveFromView(oldStartingIndex);
    }

    // Group handling
    private object GetItemGroup(object item)
        => _groupDelegate?.Invoke(item);

    private void AddItemToGroup(object item)
    {
        object key = GetItemGroup(item);
        if (key == null)
            return;

        var group = _collectionGroups.Cast<CollectionViewGroup>().FirstOrDefault(g => Equals(g.Group, key));
        if (group == null)
        {
            group = new(key);
            var comparer = ItemGroupComparer.Get(_groupsSortDirection);

            int groupIndex = _collectionGroups.BinarySearch(group, comparer);
            if (groupIndex < 0)
                groupIndex = ~groupIndex;

            _collectionGroups.Insert(groupIndex, group);
        }

        var items = group.GroupItems;
        int index = items.Count;

        if (_sortDescriptions.Any())
        {
            index = items.BinarySearch(item, this);
            if (index < 0)
                index = ~index;
        }

        items.Insert(index, item);
    }

    private void RemoveItemFromGroup(object item)
    {
        object key = GetItemGroup(item);
        if (key == null)
            return;

        var group = _collectionGroups.Cast<CollectionViewGroup>().FirstOrDefault(g => g.Group == key);
        _ = group?.GroupItems.Remove(item);
    }

    // ISupportIncrementalLoading
    private ISupportIncrementalLoading _incrementalSource;

    public bool HasMoreItems => _incrementalSource?.HasMoreItems ?? false;
    public IAsyncOperation<LoadMoreItemsResult> LoadMoreItemsAsync(uint count)
        => _incrementalSource?.LoadMoreItemsAsync(count);

    private sealed class ItemGroupComparer : IComparer, IComparer<object>, IComparer<ICollectionViewGroup>
    {
        public static readonly ItemGroupComparer AscendingComparer = new(SortDirection.Ascending);
        public static readonly ItemGroupComparer DescendingComparer = new(SortDirection.Descending);

        public static ItemGroupComparer Get(SortDirection direction)
            => direction == SortDirection.Ascending ? AscendingComparer : DescendingComparer;

        private readonly SortDirection _direction;
        private ItemGroupComparer(SortDirection direction)
        {
            _direction = direction;
        }

        public int Compare(ICollectionViewGroup x, ICollectionViewGroup y)
        {
            var gx = x.Group as IComparable;
            var gy = y.Group as IComparable;

            int result = 0;
            if (gx == gy)
                return result;

            if (gx == null)
                result = -1;
            else if (gy == null)
                result = 1;
            else
                result = gx.CompareTo(gy);

            return _direction == SortDirection.Ascending ? +result : -result;
        }

        public int Compare(object x, object y)
            => Compare((ICollectionViewGroup)x, (ICollectionViewGroup)y);
    }
}
