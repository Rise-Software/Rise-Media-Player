using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Rise.App.UserControls
{
    public sealed partial class MicaTitleBar : UserControl
    {
        public MicaTitleBar()
        {
            InitializeComponent();
            Loaded += (s, e) => ApplyTitle();
        }

        private void ApplyTitle()
        {
            if (Title != null)
            {
                _ = FindName("AppTitle");
            }
            else
            {
                _ = FindName("DefaultTitle");
            }

            HandleSizeChanges();
            SizeChanged += (s, e) => HandleSizeChanges();
        }

        public string Title { get; set; }
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
            }
            else
            {
                DefaultTitle.Visibility = vis;
            }
        }
    }
}
