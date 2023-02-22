using System;
using System.Collections;

namespace Rise.Data.Collections;

public sealed class SortDescription
{
    /// <summary>
    /// Gets the delegate that returns the value of the property
    /// to sort on.
    /// </summary>
    public Func<object, object> ValueDelegate { get; }

    /// <summary>
    /// Gets the way sorting should be handled.
    /// </summary>
    public SortDirection SortDirection { get; }

    /// <summary>
    /// Comparer to use for sorting.
    /// </summary>
    public IComparer Comparer { get; }

    /// <summary>
    /// Creates a new sort description.
    /// </summary>
    /// <param name="direction">The way sorting should be handled.</param>
    public SortDescription(SortDirection direction)
        : this(direction, (o) => o, null) { }

    /// <summary>
    /// Creates a new sort description.
    /// </summary>
    /// <param name="direction">The way sorting should be handled.</param>
    /// <param name="valueDelegate">A delegate that accepts an object as a
    /// parameter and returns the value to sort on.</param>
    public SortDescription(SortDirection direction, Func<object, object> valueDelegate)
        : this(direction, valueDelegate, null) { }

    /// <summary>
    /// Creates a new sort description.
    /// </summary>
    /// <param name="direction">The way sorting should be handled.</param>
    /// <param name="valueDelegate">A delegate that accepts an object as a
    /// parameter and returns the value to sort on.</param>
    /// <param name="comparer">Comparer to use. If null, a default comparer
    /// will be used.</param>
    public SortDescription(SortDirection direction, Func<object, object> valueDelegate, IComparer comparer)
    {
        SortDirection = direction;
        ValueDelegate = valueDelegate;
        Comparer = comparer ?? ObjectComparer.Default;
    }

    private SortDescription(SortDescription desc)
    {
        SortDirection = desc.SortDirection == SortDirection.Ascending ?
            SortDirection.Descending : SortDirection.Ascending;

        ValueDelegate = desc.ValueDelegate;
        Comparer = desc.Comparer;
    }

    /// <summary>
    /// Creates a sort description with the direction inverted.
    /// </summary>
    public SortDescription Invert()
        => new(this);

    private sealed class ObjectComparer : IComparer
    {
        public static readonly ObjectComparer Default = new();
        private ObjectComparer() { }

        public int Compare(object x, object y)
        {
            var cx = x as IComparable;
            var cy = y as IComparable;

            return cx == cy ? 0 : cx == null ? -1 : cy == null ? +1 : cx.CompareTo(cy);
        }
    }
}
