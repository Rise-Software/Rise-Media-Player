using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Rise.App.Visualizers
{
    public sealed partial class BloomView : UserControl
    {
        public BloomView()
        {
            InitializeComponent();

            BloomWebView.Height = Window.Current.Bounds.Height;
            BloomWebView.Width = Window.Current.Bounds.Width;
        }

        private void Bloom_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            BloomWebView.Height = Window.Current.Bounds.Height;
            BloomWebView.Width = Window.Current.Bounds.Width;
        }
    }
}
