using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Windows.Foundation.Collections;

namespace Rise.Common.Helpers
{
    /// <summary>
    /// A class that implements <see cref="IObservableVector{T}"/>
    /// for .NET usage. Use this if you're writing a WinRT component
    /// in C# and need an observable vector to expose.
    /// </summary>
    public sealed class ObservableVector<T> : IObservableVector<T>
    {
        private readonly ObservableCollection<T> _base;

        public ObservableVector()
        {
            _base = new();
            _base.CollectionChanged += OnBaseCollectionChanged;
        }

        public ObservableVector(List<T> list)
        {
            _base = new(list);
            _base.CollectionChanged += OnBaseCollectionChanged;
        }

        public ObservableVector(IEnumerable<T> collection)
        {
            _base = new(collection);
            _base.CollectionChanged += OnBaseCollectionChanged;
        }

        public event VectorChangedEventHandler<T> VectorChanged;
        /// <summary>
        /// Handles and adapts collection changes.
        /// </summary>
        private void OnBaseCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var args = new VectorChangedEventArgs();
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    args.CollectionChange = CollectionChange.ItemInserted;
                    args.Index = (uint)e.NewStartingIndex;
                    break;

                case NotifyCollectionChangedAction.Remove:
                    args.CollectionChange = CollectionChange.ItemRemoved;
                    args.Index = (uint)e.OldStartingIndex;
                    break;

                case NotifyCollectionChangedAction.Replace:
                    args.CollectionChange = CollectionChange.ItemChanged;
                    args.Index = (uint)e.NewStartingIndex;
                    break;

                case NotifyCollectionChangedAction.Reset:
                case NotifyCollectionChangedAction.Move:
                    args.CollectionChange = CollectionChange.Reset;
                    break;
            }

            VectorChanged?.Invoke(this, args);
        }

        public int IndexOf(T item)
        {
            return _base.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            _base.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            _base.RemoveAt(index);
        }

        public T this[int index]
        {
            get => _base[index];
            set => _base[index] = value;
        }

        public void Add(T item)
        {
            _base.Add(item);
        }

        public void Clear()
        {
            _base.Clear();
        }

        public bool Contains(T item)
        {
            return _base.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _base.CopyTo(array, arrayIndex);
        }

        public bool Remove(T item)
        {
            return _base.Remove(item);
        }

        public int Count => _base.Count;

        public bool IsReadOnly { get; private set; }
            = false;

        public IEnumerator<T> GetEnumerator()
        {
            return _base.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _base.GetEnumerator();
        }
    }

    /// <summary>
    /// A class that implements <see cref="IVectorChangedEventArgs"/>
    /// for .NET usage. Use this if you're writing a WinRT component
    /// in C# and need an observable vector to expose.
    /// </summary>
    public sealed class VectorChangedEventArgs : IVectorChangedEventArgs
    {
        public CollectionChange CollectionChange { get; set; }
        public uint Index { get; set; }
    }
}
