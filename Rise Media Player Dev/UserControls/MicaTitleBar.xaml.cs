using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

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
        public double LabelWidth => AppData.DesiredSize.Width;
    }
}
