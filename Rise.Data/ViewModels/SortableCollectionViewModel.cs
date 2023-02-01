using CommunityToolkit.Mvvm.Input;
using Microsoft.Toolkit.Uwp.UI;
using Rise.Common.Extensions;
using System;
using System.Collections;

namespace Rise.Data.ViewModels
{
    /// <summary>
    /// A ViewModel for pages that contain collections
    /// of data. Allows for sorting through commands
    /// and methods, thanks to <see cref="AdvancedCollectionView"/>.
    /// </summary>
    public partial class SortableCollectionViewModel : ViewModel, IDisposable
    {
        private Func<object, bool> CanSort { get; set; }
        private bool CanSortBy(string parameter)
            => CanSort == null || CanSort(parameter);
        private bool CanUpdateDirection(SortDirection parameter)
            => CanSort == null || CanSort(parameter);

        /// <summary>
        /// The collection of sorted items.
        /// </summary>
        public AdvancedCollectionView Items { get; private set; }

        /// <summary>
        /// Initializes a new instance of this class with
        /// a source of items.
        /// </summary>
        /// <param name="itemSource">Source of items for this
        /// ViewModel. Observable sources are preferrable as
        /// they update automatically.</param>
        public SortableCollectionViewModel(IList itemSource)
            : this(itemSource, null) { }

        /// <summary>
        /// Initializes a new instance of this class with
        /// a source of items and sorting related defaults.
        /// </summary>
        /// <param name="itemSource">Source of items for this
        /// ViewModel. Observable sources are preferrable as
        /// they update automatically.</param>
        /// <param name="defaultProperty">The default property
        /// name by which <see cref="Items"/> is sorted.</param>
        /// <param name="defaultDirection">The default direction
        /// in which <see cref="Items"/> is sorted.</param>
        public SortableCollectionViewModel(IList itemSource, string defaultProperty,
            SortDirection defaultDirection)
            : this(itemSource, null, defaultProperty, defaultDirection) { }

        /// <summary>
        /// Initializes a new instance of this class with
        /// a source of items and a delegate that indicates
        /// whether sorting is currently possible.
        /// </summary>
        /// <param name="itemSource">Source of items for this
        /// ViewModel. Observable sources are preferrable as
        /// they update automatically.</param>
        /// <param name="canSort">A delegate that indicates whether
        /// sorting is currently possible.</param>
        public SortableCollectionViewModel(IList itemSource, Func<object, bool> canSort)
        {
            Items = new AdvancedCollectionView(itemSource);
            CanSort = canSort;
        }

        /// <summary>
        /// Initializes a new instance of this class with
        /// a source of items, a delegate that indicates
        /// whether sorting is currently possible, and
        /// sorting related defaults.
        /// </summary>
        /// <param name="itemSource">Source of items for this
        /// ViewModel. Observable sources are preferrable as
        /// they update automatically.</param>
        /// <param name="canSort">A delegate that indicates whether
        /// sorting is currently possible.</param>
        /// <param name="defaultProperty">The default property
        /// name by which <see cref="Items"/> is sorted.</param>
        /// <param name="defaultDirection">The default direction
        /// in which <see cref="Items"/> is sorted.</param>
        public SortableCollectionViewModel(IList itemSource, Func<object, bool> canSort,
            string defaultProperty, SortDirection defaultDirection)
            : this(itemSource, canSort)
        {
            if (!string.IsNullOrWhiteSpace(defaultProperty))
            {
                Items.SortDescriptions.Add(new SortDescription(defaultProperty, defaultDirection));
                CurrentSortProperty = defaultProperty;
            }

            CurrentSortDirection = defaultDirection;
        }

        public void Dispose()
        {
            Items.ClearFilter();
        }
    }

    // Sorting
    public partial class SortableCollectionViewModel
    {
        private string _currentSortProperty;
        /// <summary>
        /// The current property by which <see cref="Items"/> is sorted.
        /// </summary>
        public string CurrentSortProperty
        {
            get => _currentSortProperty;
            private set => Set(ref _currentSortProperty, value);
        }

        private SortDirection _currentSortDirection;
        /// <summary>
        /// The current direction in which <see cref="Items"/> is sorted.
        /// Can be ascending or descending.
        /// </summary>
        public SortDirection CurrentSortDirection
        {
            get => _currentSortDirection;
            private set => Set(ref _currentSortDirection, value);
        }

        /// <summary>
        /// Sorts items based on the given property name and sort direction.
        /// </summary>
        public void Sort(string prop, SortDirection direction)
        {
            Items.SortDescriptions.Clear();
            Items.SortDescriptions.Add(new SortDescription(prop, direction));

            CurrentSortProperty = prop;
            CurrentSortDirection = direction;
        }

        /// <summary>
        /// Sorts items based on the given property name.
        /// </summary>
        [RelayCommand(CanExecute = nameof(CanSortBy))]
        public void SortBy(string prop)
        {
            Items.SortDescriptions.Clear();
            Items.SortDescriptions.Add(new SortDescription(prop, _currentSortDirection));
            CurrentSortProperty = prop;
        }

        /// <summary>
        /// Updates the sort direction of items.
        /// </summary>
        /// <param name="direction">New sort direction to use.</param>
        [RelayCommand(CanExecute = nameof(CanUpdateDirection))]
        public void UpdateSortDirection(SortDirection direction)
        {
            Items.SortDescriptions.Clear();
            Items.SortDescriptions.Add(new SortDescription(_currentSortProperty, direction));
            CurrentSortDirection = direction;
        }
    }
}
