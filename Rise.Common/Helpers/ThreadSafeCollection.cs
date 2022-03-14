using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Threading;

namespace Rise.Common.Helpers
{
    public sealed class ThreadSafeCollection<T> : ObservableCollection<T>
    {
        private readonly Dictionary<NotifyCollectionChangedEventHandler, SynchronizationContext>
            CollectionChangedEvents = new();

        private readonly Dictionary<PropertyChangedEventHandler, SynchronizationContext>
            PropertyChangedEvents = new();

        public override event NotifyCollectionChangedEventHandler CollectionChanged
        {
            add
            {
                CollectionChangedEvents.Add(value, SynchronizationContext.Current);
            }
            remove
            {
                CollectionChangedEvents.Remove(value);
            }
        }

        protected override event PropertyChangedEventHandler PropertyChanged
        {
            add
            {
                PropertyChangedEvents.Add(value, SynchronizationContext.Current);
            }
            remove
            {
                PropertyChangedEvents.Remove(value);
            }
        }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            foreach (KeyValuePair<NotifyCollectionChangedEventHandler, SynchronizationContext> @event in CollectionChangedEvents)
            {
                if (@event.Value == null)
                {
                    @event.Key.Invoke(this, e);
                }
                else
                {
                    @event.Value.Post(s => @event.Key.Invoke(s, e), this);
                }
            }
        }

        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            foreach (KeyValuePair<PropertyChangedEventHandler, SynchronizationContext> @event in PropertyChangedEvents)
            {
                if (@event.Value == null)
                {
                    @event.Key.Invoke(this, e);
                }
                else
                {
                    @event.Value.Post(s => @event.Key.Invoke(s, e), this);
                }
            }
        }
    }
}
