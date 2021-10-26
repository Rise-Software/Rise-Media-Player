using RMP.App.Commands;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace RMP.App.UserControls
{
    /// <summary>
    /// A container for ListView items, used to support hover states easily.
    /// </summary>
    public sealed partial class ListItemContainer : UserControl
    {
        public ListItemContainer()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty IsPointerOverProperty =
            DependencyProperty.Register("IsPointerOver", typeof(bool), typeof(ListItemContainer),
                new PropertyMetadata(false));

        public bool IsPointerOver
        {
            get => (bool)GetValue(IsPointerOverProperty);
            set => SetValue(IsPointerOverProperty, value);
        }

        protected override void OnPointerEntered(PointerRoutedEventArgs e)
        {
            base.OnPointerEntered(e);
            IsPointerOver = true;
        }

        protected override void OnPointerCanceled(PointerRoutedEventArgs e)
        {
            base.OnPointerCanceled(e);
            IsPointerOver = false;
        }

        protected override void OnPointerExited(PointerRoutedEventArgs e)
        {
            base.OnPointerExited(e);
            IsPointerOver = false;
        }

        public event RoutedEventHandler Click;
        private void OnButtonClick(object sender, RoutedEventArgs e)
        {
            Click?.Invoke(this, e);
        }
    }
}
