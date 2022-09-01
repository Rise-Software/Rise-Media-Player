using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Rise.App.UserControls
{
    public sealed partial class TopTracksWidgetContentControl : UserControl
    {
        public static readonly DependencyProperty ItemsSourceProperty
            = DependencyProperty.Register(nameof(ItemsSource), typeof(object),
                typeof(TopTracksWidgetContentControl), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the widget content.
        /// </summary>
        public object ItemsSource
        {
            get => GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        public TopTracksWidgetContentControl()
        {
            InitializeComponent();
        }
    }
}
