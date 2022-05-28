using System;

namespace Rise.App.Models
{
    public sealed class EnumerationSource<TEnumeration>
    {
        private readonly Func<TEnumeration, bool> _predicate;

        private readonly Func<IEnumerationDestination<TEnumeration>> _initializer;

        private IEnumerationDestination<TEnumeration>? _destination;

        public EnumerationSource(Func<TEnumeration, bool> predicate, Func<IEnumerationDestination<TEnumeration>> initializer)
        {
            this._predicate = predicate;
            this._initializer = initializer;
        }

        public void ResetData()
        {
            _destination = null;
        }

        public bool Predicate(TEnumeration enumeration)
        {
            return _predicate(enumeration);
        }

        public IEnumerationDestination<TEnumeration> GetOrCreateEnumerationDestination()
        {
            _destination ??= _initializer();

            return _destination;
        }
    }
}
