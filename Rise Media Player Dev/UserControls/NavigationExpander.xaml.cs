using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace RMP.App.UserControls
{
    [ContentProperty(Name = "Controls")]
    public sealed partial class NavigationExpander : UserControl
    {
        public NavigationExpander()
        {
            InitializeComponent();
        }

        public string Title { get; set; }
        public string Description { get; set; }
        public int ControlKind { get; set; }

        public string ExpanderKind { get; set; }
        private readonly string Button = "Button";
        private readonly string Expander = "Expander";
        private readonly string Static = "Static";
        private readonly string Transparent = "Transparent";

        public string Icon { get; set; }

        public static DependencyProperty ControlsProperty =
            DependencyProperty.Register("Controls", typeof(object), typeof(NavigationExpander), null);

        public object Controls
        {
            get => GetValue(ControlsProperty);
            set => SetValue(ControlsProperty, value);
        }

        public event RoutedEventHandler Click;
        private void OnButtonClick(object sender, RoutedEventArgs e)
        {
            Click?.Invoke(this, e);
        }
    }
}
