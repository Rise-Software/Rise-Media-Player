using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Rise.App.UserControls
{
    public sealed partial class MicaTitleBar : UserControl
    {
        public MicaTitleBar()
        {
            InitializeComponent();
            Loaded += (s, e) =>
            {
                HandleSizeChanges();
                SizeChanged += (d, r) => HandleSizeChanges();
            };
        }

        private static readonly DependencyProperty IconProperty =
            DependencyProperty.Register("Icon", typeof(string), typeof(MicaTitleBar), new PropertyMetadata("ms-appx:///Assets/App/Titlebar.png"));

        public string Icon
        {
            get => (string)GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }

        private static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(MicaTitleBar), new PropertyMetadata(null));

        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set
            {
                SetValue(TitleProperty, value);
                HandleSizeChanges();
            }
        }

        public int AddLabelWidth { get; set; }
        public bool ShowIcon { get; set; }
        public double LabelWidth => AppData.DesiredSize.Width;

        private void HandleSizeChanges()
        {
            double width = Window.Current.Bounds.Width;

            Visibility vis = Visibility.Collapsed;
            if (width >= AddLabelWidth)
            {
                vis = Visibility.Visible;
            }

            if (Title != null)
            {
                AppTitle.Visibility = vis;
                DefaultTitle.Visibility = Visibility.Collapsed;
            }
            else
            {
                DefaultTitle.Visibility = vis;
                AppTitle.Visibility = Visibility.Collapsed;
            }
        }
    }
}
