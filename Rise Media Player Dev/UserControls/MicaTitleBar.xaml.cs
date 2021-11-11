using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Rise.App.UserControls
{
    public sealed partial class MicaTitleBar : UserControl
    {
        public MicaTitleBar()
        {
            InitializeComponent();
            Loaded += ApplyTitle;
        }

        private void ApplyTitle(object sender, RoutedEventArgs e)
        {
            if (Title != null)
            {
                _ = FindName("AppTitle");
            }
            else
            {
                _ = FindName("DefaultTitle");
            }

            SizeChanged += UserControl_SizeChanged;
        }

        public string Title { get; set; }
        public int AddLabelWidth { get; set; }
        public bool ShowIcon { get; set; }
        public double LabelWidth => AppData.DesiredSize.Width;

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
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
            }
            else
            {
                DefaultTitle.Visibility = vis;
            }
        }
    }
}
