using Rise.Common.Extensions;
using Rise.Common.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using Windows.Foundation.Collections;
using Windows.UI.Xaml.Data;

namespace Rise.Data.Collections;

/// <summary>
/// An implementation for <see cref="ICollectionView"/> with grouping
/// and incremental loading support.
/// </summary>
public sealed partial class GroupedCollectionView : ICollectionView, ISupportIncrementalLoading,
    INotifyPropertyChanged, IComparer<object>, IDisposable
{
    private readonly List<object> _view = new();

    private Func<object, object> _groupDelegate;
    /// <summary>
    /// A delegate for grouping.
    /// </summary>
    public Func<object, object> GroupDelegate
    {
        get => _groupDelegate;
        set
        {
            if (_groupDelegate != value)
            {
                _groupDelegate = value;
                OnGroupChanged();
            }
        }
    }

    private SortDirection _groupsSortDirection = SortDirection.Ascending;
    /// <summary>
    /// The sort direction of the collection's groups.
    /// Ascending by default.
    /// </summary>
    public SortDirection GroupsSortDirection
    {
        get => _groupsSortDirection;
        set
        {
            if (_groupsSortDirection != value)
            {
                _groupsSortDirection = value;
                OnGroupChanged();
            }
        }
    }

    private readonly ObservableVector<object> _collectionGroups = new();
    public IObservableVector<object> CollectionGroups => _collectionGroups;

    private readonly ObservableCollection<SortDescription> _sortDescriptions;
    /// <summary>
    /// A list of objects that define sorting rules for visible items.
    /// </summary>
    public ObservableCollection<SortDescription> SortDescriptions => _sortDescriptions;

    private Predicate<object> _filter;
    /// <summary>
    /// A predicate to filter objects from the collection.
    /// </summary>
    public Predicate<object> Filter
    {
        get => _filter;
        set
        {
            if (_filter != value)
            {
                _filter = value;
                OnFilterChanged();
            }
        }
    }

    private IList _source;
    /// <summary>
    /// Gets or sets the source for this collection view.
    /// </summary>
    public IList Source
    {
        get => _source;
        set
        {
            if (value == null)
                throw new ArgumentNullException("Source cannot be null.");

            if (_source == value)
                return;

            if (_source is INotifyCollectionChanged oldNcc)
                oldNcc.CollectionChanged -= OnSourceCollectionChanged;

            _source = value;
            if (_source is INotifyCollectionChanged ncc)
                ncc.CollectionChanged += OnSourceCollectionChanged;

            OnSourceChanged();
        }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="GroupedCollectionView"/>
    /// with an empty list.
    /// </summary>
    public GroupedCollectionView()
    {
        _sortDescriptions = new();
        _sortDescriptions.CollectionChanged += OnSortDescriptionsChanged;

        _source = new List<object>();
    }

    /// <summary>
    /// Initializes a new instance of <see cref="GroupedCollectionView"/>
    /// with the provided data source.
    /// </summary>
    public GroupedCollectionView(IList source)
    {
        _sortDescriptions = new();
        _sortDescriptions.CollectionChanged += OnSortDescriptionsChanged;

        Source = source;
    }

    /// <summary>
    /// Initializes a new instance of <see cref="GroupedCollectionView"/>
    /// with the provided data source and sort descriptions.
    /// </summary>
    public GroupedCollectionView(IList source, IEnumerable<SortDescription> sorts)
    {
        _sortDescriptions = new(sorts);
        _sortDescriptions.CollectionChanged += OnSortDescriptionsChanged;

        Source = source;
    }

    /// <summary>
    /// Initializes a new instance of <see cref="GroupedCollectionView"/>
    /// with the provided data source, sort descriptions, and grouping delegate.
    /// </summary>
    public GroupedCollectionView(IList source, IEnumerable<SortDescription> sorts, Func<object, object> group)
    {
        _groupDelegate = group;

        _sortDescriptions = new(sorts);
        _sortDescriptions.CollectionChanged += OnSortDescriptionsChanged;

        Source = source;
    }

    /// <summary>
    /// Replaces all sort descriptions with the provided ones.
    /// </summary>
    public void ReplaceSorting(IEnumerable<SortDescription> sorts)
        => ReplaceSortingAndGrouping(sorts, _groupDelegate, _groupsSortDirection);

    /// <summary>
    /// Replaces grouping related params with the provided ones.
    /// </summary>
    public void ReplaceGrouping(Func<object, object> groupDel, SortDirection groupDirection)
    {
        _groupDelegate = groupDel;
        _groupsSortDirection = groupDirection;

        OnGroupChanged();
    }

    /// <summary>
    /// Replaces all sort descriptions with the provided ones, and uses
    /// the provided delegate for grouping.
    /// </summary>
    public void ReplaceSortingAndGrouping(IEnumerable<SortDescription> sorts, Func<object, object> groupDel)
        => ReplaceSortingAndGrouping(sorts, groupDel, _groupsSortDirection);

    /// <summary>
    /// Replaces all sort descriptions with the provided ones, uses the
    /// provided delegate for grouping, and updates the grouping direction.
    /// </summary>
    public void ReplaceSortingAndGrouping(IEnumerable<SortDescription> sorts, Func<object, object> groupDel, SortDirection groupDirection)
    {
        _groupDelegate = groupDel;
        _groupsSortDirection = groupDirection;

        _sortDescriptions.CollectionChanged -= OnSortDescriptionsChanged;

        _sortDescriptions.Clear();
        foreach (var sort in sorts)
            _sortDescriptions.Add(sort);

        _sortDescriptions.CollectionChanged += OnSortDescriptionsChanged;
        OnSortChanged();
    }

    private void OnSortDescriptionsChanged(object sender, NotifyCollectionChangedEventArgs e)
        => OnSortChanged();

    private void OnSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
                if (e.NewItems?.Count == 1)
                    OnItemAdded(e.NewStartingIndex, e.NewItems[0]);
                else
                    OnSourceChanged();
                break;

            case NotifyCollectionChangedAction.Remove:
                if (e.OldItems?.Count == 1)
                    OnItemRemoved(e.OldStartingIndex, e.OldItems[0]);
                else
                    OnSourceChanged();
                break;

            case NotifyCollectionChangedAction.Replace:
                if (e.OldItems?.Count == 1)
                {
                    OnItemRemoved(e.OldStartingIndex, e.OldItems[0]);
                    OnItemAdded(e.OldStartingIndex, e.NewItems[0]);
                }
                else
                {
                    OnSourceChanged();
                }
                break;

            case NotifyCollectionChangedAction.Move:
                if (e.OldItems?.Count == 1)
                {
                    OnItemRemoved(e.OldStartingIndex, e.OldItems[0]);
                    OnItemAdded(e.NewStartingIndex, e.OldItems[0]);

                    MoveCurrentToIndex(e.NewStartingIndex);
                }
                else
                {
                    OnSourceChanged();
                }
                break;

            case NotifyCollectionChangedAction.Reset:
                OnSourceChanged();
                break;
        }
    }

    private void OnSourceChanged()
    {
        _incrementalSource = _source as ISupportIncrementalLoading;

        var current = CurrentItem;
        _view.Clear();
        _collectionGroups.Clear();

        foreach (var item in _source)
        {
            if (_filter != null && !_filter(item))
                continue;

            _view.Add(item);

            if (_groupDelegate != null)
                AddItemToGroup(item);
        }

        _view.Sort(this);

        OnVectorChanged(CollectionChange.Reset, 0);
        _ = MoveCurrentTo(current);
    }

    private void OnGroupChanged()
    {
        _collectionGroups.Clear();
        if (_groupDelegate == null)
            return;

        var grouped = _view.GroupBy(_groupDelegate);
        var comparer = ItemGroupComparer.Get(_groupsSortDirection);

        foreach (var group in grouped)
        {
            int index = _collectionGroups.BinarySearch(group, comparer);
            if (index < 0)
                index = ~index;

            _collectionGroups.Insert(index, new CollectionViewGroup(group));
        }
    }

    private void OnSortChanged()
    {
        _view.Sort(this);
        OnGroupChanged();

        OnVectorChanged(CollectionChange.Reset, 0);
    }

    private void OnFilterChanged()
    {
        if (_filter != null)
        {
            for (int index = 0; index < _view.Count; index++)
            {
                var item = _view.ElementAt(index);
                if (_filter(item)) continue;

                RemoveFromView(index);
                index--;
            }
        }

        var viewHash = new HashSet<object>(_view);

        int viewIndex = 0;
        for (int index = 0; index < _source.Count; index++)
        {
            var item = _source[index];
            if (viewHash.Contains(item))
            {
                viewIndex++;
                continue;
            }

            if (OnItemAdded(index, item, viewIndex))
                viewIndex++;
        }
    }

    public int Compare(object x, object y)
    {
        foreach (var desc in _sortDescriptions)
        {
            var del = desc.ValueDelegate;

            object xVal = del(x);
            object yVal = del(y);

            var result = desc.Comparer.Compare(xVal, yVal);
            if (result != 0)
                return desc.SortDirection == SortDirection.Ascending ? +result : -result;
        }

        return 0;
    }

    public void Dispose()
    {
        _filter = null;
        if (_source is INotifyCollectionChanged ncc)
            ncc.CollectionChanged -= OnSourceCollectionChanged;
    }

    private sealed class CollectionViewGroup : ICollectionViewGroup
    {
        public object Group { get; }
        public IObservableVector<object> GroupItems { get; }

        public CollectionViewGroup(object group)
        {
            Group = group;
            GroupItems = new ObservableVector<object>();
        }

        public CollectionViewGroup(IGrouping<object, object> group)
        {
            Group = group.Key;
            GroupItems = new ObservableVector<object>(group);
        }

        public CollectionViewGroup(object group, IEnumerable<object> items)
        {
            Group = group;
            GroupItems = new ObservableVector<object>(items);
        }
    }
}
