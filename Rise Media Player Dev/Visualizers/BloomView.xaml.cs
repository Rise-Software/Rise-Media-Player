using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Rise.App.Visualizers
{
    public sealed partial class BloomView : UserControl
    {
        public BloomView()
        {
            InitializeComponent();

            BloomWebView.Source = new("ms-appx-web:///Visualizers/Bloom.html");

            BloomWebView.Height = ActualHeight;
            BloomWebView.Width = ActualWidth;
        }

        private void Bloom_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            BloomWebView.Height = ActualHeight;
            BloomWebView.Width = ActualWidth;
        }
    }
}
