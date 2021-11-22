using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Rise.App.UserControls
{
    public sealed partial class MicaTitleBar : UserControl
    {
        public MicaTitleBar()
        {
            InitializeComponent();
            Icon = "ms-appx:///Assets/App/Titlebar.png";

            Loaded += (s, e) => ApplyTitle();
        }

        private void ApplyTitle()
        {
            if (Title == null)
            {
                AppTitle.Visibility = Visibility.Collapsed;
                DefaultTitle.Visibility = Visibility.Visible;
            }
            else
            {
                AppTitle.Visibility = Visibility.Visible;
                DefaultTitle.Visibility = Visibility.Collapsed;
            }

            HandleSizeChanges();
            SizeChanged += (s, e) => HandleSizeChanges();
        }

        public static DependencyProperty IconProperty =
            DependencyProperty.Register("Icon", typeof(string), typeof(MicaTitleBar), null);

        public string Icon
        {
            get => (string)GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }

        public static DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(MicaTitleBar), null);

        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set
            {
                SetValue(TitleProperty, value);
                if (value == null)
                {
                    AppTitle.Visibility = Visibility.Collapsed;
                    DefaultTitle.Visibility = Visibility.Visible;
                }
                else
                {
                    AppTitle.Visibility = Visibility.Visible;
                    DefaultTitle.Visibility = Visibility.Collapsed;
                }
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
            }
            else
            {
                DefaultTitle.Visibility = vis;
            }
        }
    }
}
