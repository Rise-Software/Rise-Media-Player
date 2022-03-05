using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Rise.App.Materials_Visualisers
{
    public sealed partial class BloomView : UserControl
    {
        public BloomView()
        {
            this.InitializeComponent();
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
