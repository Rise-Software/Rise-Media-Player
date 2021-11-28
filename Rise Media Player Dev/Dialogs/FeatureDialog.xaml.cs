using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Rise.App.Dialogs
{
    public sealed partial class FeatureDialog : ContentDialog
    {
        public FeatureDialog()
        {
            InitializeComponent();
        }

        private readonly static DependencyProperty ImageUriProperty =
            DependencyProperty.Register("ImageUri", typeof(string), typeof(FeatureDialog), null);

        public string ImageUri
        {
            get => (string)GetValue(ImageUriProperty);
            set => SetValue(ImageUriProperty, value);
        }
    }
}
