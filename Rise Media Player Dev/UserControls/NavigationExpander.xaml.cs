using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;
using static RMP.App.Enums;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace RMP.App.UserControls
{
    [ContentProperty(Name = "Controls")]
    public sealed partial class NavigationExpander : UserControl
    {
        public NavigationExpander()
        {
            InitializeComponent();
            Loaded += ApplyExpanderStyle;
        }

        private void ApplyExpanderStyle(object sender, RoutedEventArgs e)
        {
            switch (ExpanderStyle)
            {
                case ExpanderStyles.Static:
                    _ = FindName("RootBorder");
                    break;

                case ExpanderStyles.Button:
                    _ = FindName("RootButton");
                    break;

                case ExpanderStyles.Transparent:
                    _ = FindName("RootTransparent");
                    break;

                default:
                    _ = FindName("RootExpander");
                    break;
            }
        }

        public string Title { get; set; }
        public string Description { get; set; }

        public ExpanderStyles ExpanderStyle { get; set; }

        public string Icon { get; set; }

        public static DependencyProperty ControlsProperty =
            DependencyProperty.Register("Controls", typeof(object), typeof(NavigationExpander), null);

        public object Controls
        {
            get => GetValue(ControlsProperty);
            set => SetValue(ControlsProperty, value);
        }

        public static DependencyProperty HeaderControlsProperty =
            DependencyProperty.Register("HeaderControls", typeof(object), typeof(NavigationExpander), null);

        public object HeaderControls
        {
            get => GetValue(HeaderControlsProperty);
            set => SetValue(HeaderControlsProperty, value);
        }

        public event RoutedEventHandler Click;
        private void OnButtonClick(object sender, RoutedEventArgs e)
        {
            Click?.Invoke(this, e);
        }
    }
}
