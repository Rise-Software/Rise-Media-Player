using Windows.UI.Xaml.Controls;

namespace RMP.App.UserControls
{
    public sealed partial class MicaTitleBar : UserControl
    {
        public MicaTitleBar()
        {
            InitializeComponent();
        }

        public string Title { get; set; }
        public int AddLabelWidth { get; set; }
        public bool ShowIcon { get; set; }
        public double LabelWidth => AppData.DesiredSize.Width;
    }
}
