using Windows.Foundation.Collections;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml;
using Microsoft.Xaml.Interactivity;
using Microsoft.Toolkit.Uwp.UI;

#nullable enable

namespace Rise.Common.Behaviors;

public sealed class AlternatingListViewBehavior : Behavior<ListViewBase>
{
    public static readonly DependencyProperty AlternateBackgroundProperty = DependencyProperty.Register(
        nameof(AlternateBackground),
        typeof(Brush),
        typeof(AlternatingListViewBehavior),
        new PropertyMetadata(default(Brush)));

    public static readonly DependencyProperty AlternateBorderThicknessProperty = DependencyProperty.Register(
        nameof(AlternateBorderThickness),
        typeof(Thickness),
        typeof(AlternatingListViewBehavior),
        new PropertyMetadata(default(Thickness)));

    public static readonly DependencyProperty AlternateBorderBrushProperty = DependencyProperty.Register(
        nameof(AlternateBorderBrush),
        typeof(Brush),
        typeof(AlternatingListViewBehavior),
        new PropertyMetadata(default(Brush?)));

    public static readonly DependencyProperty BackgroundProperty = DependencyProperty.Register(
        nameof(Background),
        typeof(Brush),
        typeof(AlternatingListViewBehavior),
        new PropertyMetadata(default(Brush)));

    public static readonly DependencyProperty BorderThicknessProperty = DependencyProperty.Register(
        nameof(BorderThickness),
        typeof(Thickness),
        typeof(AlternatingListViewBehavior),
        new PropertyMetadata(default(Thickness)));

    public static readonly DependencyProperty BorderBrushProperty = DependencyProperty.Register(
        nameof(BorderBrush),
        typeof(Brush),
        typeof(AlternatingListViewBehavior),
        new PropertyMetadata(default(Brush?)));

    public Brush? AlternateBorderBrush
    {
        get => (Brush?)GetValue(AlternateBorderBrushProperty);
        set => SetValue(AlternateBorderBrushProperty, value);
    }

    public Thickness AlternateBorderThickness
    {
        get => (Thickness)GetValue(AlternateBorderThicknessProperty);
        set => SetValue(AlternateBorderThicknessProperty, value);
    }

    public Brush? AlternateBackground
    {
        get => (Brush?)GetValue(AlternateBackgroundProperty);
        set => SetValue(AlternateBackgroundProperty, value);
    }

    public Brush? BorderBrush
    {
        get => (Brush?)GetValue(BorderBrushProperty);
        set => SetValue(BorderBrushProperty, value);
    }

    public Thickness BorderThickness
    {
        get => (Thickness)GetValue(BorderThicknessProperty);
        set => SetValue(BorderThicknessProperty, value);
    }

    public Brush? Background
    {
        get => (Brush?)GetValue(BackgroundProperty);
        set => SetValue(BackgroundProperty, value);
    }

    protected override void OnAttached()
    {
        base.OnAttached();

        AssociatedObject.ActualThemeChanged += OnActualThemeChanged;
        AssociatedObject.ContainerContentChanging += OnContainerContentChanging;
        if (AssociatedObject.Items != null)
            AssociatedObject.Items.VectorChanged += ItemsOnVectorChanged;
    }

    protected override void OnDetaching()
    {
        base.OnDetaching();

        AssociatedObject.ActualThemeChanged -= OnActualThemeChanged;
        AssociatedObject.ContainerContentChanging -= OnContainerContentChanging;

        if (AssociatedObject.Items != null)
            AssociatedObject.Items.VectorChanged -= ItemsOnVectorChanged;
    }

    private void OnActualThemeChanged(FrameworkElement sender, object args)
    {
        if (AssociatedObject.Items == null) return;
        for (int i = 0; i < AssociatedObject.Items.Count; i++)
        {
            if (AssociatedObject.ContainerFromIndex(i) is not SelectorItem itemContainer)
                continue;

            UpdateAlternateLayout(itemContainer, i);
        }
    }

    private void ItemsOnVectorChanged(IObservableVector<object> sender, IVectorChangedEventArgs args)
    {
        // If the index is at the end we can ignore
        if (args.Index == (sender.Count - 1))
            return;

        // Only need to handle Inserted and Removed because we'll handle everything else in the
        // OnContainerContentChanging method
        if (args.CollectionChange is CollectionChange.ItemInserted or CollectionChange.ItemRemoved)
        {
            for (int i = (int)args.Index; i < sender.Count; i++)
            {
                if (AssociatedObject.ContainerFromIndex(i) is not SelectorItem itemContainer)
                    continue;

                UpdateAlternateLayout(itemContainer, i);
            }
        }
    }

    private void OnContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args)
    {
        if (args.Phase > 0 || args.InRecycleQueue) return;
        UpdateAlternateLayout(args.ItemContainer, args.ItemIndex);
    }

    private void UpdateAlternateLayout(SelectorItem itemContainer, int itemIndex)
    {
        if (itemIndex < 0 || AlternateBackground == null) return;

        var evenBackground = AlternateBackground;
        itemContainer.Background = itemIndex % 2 == 0 ? evenBackground : null;

        if (itemContainer.FindDescendant<Border>() is not { } border) return;

        if (itemIndex % 2 == 0)
        {
            border.Background = evenBackground;
            border.BorderBrush = AlternateBorderBrush;
            border.BorderThickness = AlternateBorderThickness;

            return;
        }

        border.Background = Background;
        border.BorderBrush = BorderBrush;
        border.BorderThickness = BorderThickness;
    }
}