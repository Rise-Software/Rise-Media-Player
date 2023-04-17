using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace Rise.Data.Collections
{
    /// <summary>
    /// A collection that wraps an observable collection
    /// with basic support for variance.
    /// </summary>
    public sealed class ObservableWrapper<TBase, TActual> : IList, IList<TBase>, INotifyCollectionChanged
        where TBase : class
        where TActual : class, TBase
    {
        private readonly ObservableCollection<TActual> _base;
        private readonly IList _listBase;

        object IList.this[int index]
        {
            get => _base[index];
            set => _listBase[index] = value;
        }

        public TBase this[int index]
        {
            get => _base[index];
            set => _base[index] = (TActual)value;
        }

        public int Count => _base.Count;

        public bool IsFixedSize => _listBase.IsFixedSize;
        public bool IsReadOnly => _listBase.IsReadOnly;

        public bool IsSynchronized => _listBase.IsSynchronized;
        public object SyncRoot => _listBase.SyncRoot;

        public ObservableWrapper(ObservableCollection<TActual> collection)
        {
            _base = collection;
            _listBase = collection;

            _base.CollectionChanged += OnBaseCollectionChanged;
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;
        private void OnBaseCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
            => CollectionChanged?.Invoke(this, e);

        int IList.Add(object value)
        {
            int index = _base.Count;
            Add((TBase)value);

            return index;
        }

        public void Add(TBase value)
            => _base.Add((TActual)value);

        public void Clear()
            => _base.Clear();

        bool IList.Contains(object value)
            => _listBase.Contains(value);

        public bool Contains(TBase value)
            => _base.Contains((TActual)value);

        int IList.IndexOf(object value)
            => _listBase.IndexOf(value);

        public int IndexOf(TBase value)
            => _base.IndexOf((TActual)value);

        void IList.Insert(int index, object value)
            => Insert(index, (TBase)value);

        public void Insert(int index, TBase value)
            => _base.Insert(index, (TActual)value);

        void IList.Remove(object value)
            => _listBase.Remove(value);

        public bool Remove(TBase value)
            => _base.Remove((TActual)value);

        public void RemoveAt(int index)
            => _base.RemoveAt(index);

        void ICollection.CopyTo(Array array, int index)
            => _listBase.CopyTo(array, index);

        public void CopyTo(TBase[] array, int index)
            => _base.CopyTo((TActual[])array, index);

        public IEnumerator<TBase> GetEnumerator()
            => _base.Cast<TBase>().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => _base.GetEnumerator();
    }
}
