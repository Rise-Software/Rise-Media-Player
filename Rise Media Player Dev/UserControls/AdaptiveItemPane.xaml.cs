using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace RMP.App.UserControls
{
    public sealed partial class AdaptiveItemPane : UserControl
    {
        private double Breakpoint { get; set; }

        public AdaptiveItemPane()
        {
            InitializeComponent();
            Loaded += AdaptiveItemPane_Loaded;
        }

        private void AdaptiveItemPane_Loaded(object sender, RoutedEventArgs e)
        {
            Breakpoint = Left.DesiredSize.Width + Right.DesiredSize.Width;
            PerformResize(ActualWidth);

            FirstDefinition.Width = new GridLength(1, GridUnitType.Star);
        }

        public static DependencyProperty LeftPaneProperty =
            DependencyProperty.Register("LeftPane", typeof(object), typeof(AdaptiveItemPane), null);

        public object LeftPane
        {
            get => GetValue(LeftPaneProperty);
            set
            {
                SetValue(LeftPaneProperty, value);
                Breakpoint = Left.DesiredSize.Width + Right.DesiredSize.Width;
            }
        }

        public static DependencyProperty RightPaneProperty =
            DependencyProperty.Register("RightPane", typeof(object), typeof(AdaptiveItemPane), null);

        public object RightPane
        {
            get => GetValue(RightPaneProperty);
            set
            {
                SetValue(RightPaneProperty, value);
                Breakpoint = Left.DesiredSize.Width + Right.DesiredSize.Width;
            }
        }

        private void Pane_SizeChanged(object sender, SizeChangedEventArgs e)
            => PerformResize(e.NewSize.Width);

        private void PerformResize(double width)
        {
            if (width - 12 < Breakpoint)
            {
                Right.SetValue(Grid.RowProperty, 1);
                Right.SetValue(Grid.ColumnProperty, 0);
                Right.HorizontalAlignment = HorizontalAlignment.Left;
            }
            else
            {
                Right.SetValue(Grid.RowProperty, 0);
                Right.SetValue(Grid.ColumnProperty, 1);
                Right.HorizontalAlignment = HorizontalAlignment.Right;
            }
        }
    }
}
