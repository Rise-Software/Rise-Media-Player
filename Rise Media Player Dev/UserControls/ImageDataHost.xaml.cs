using Rise.App.Helpers;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Rise.App.UserControls
{
    /// <summary>
    /// This control allows to host <see cref="SoftwareBitmap"/>
    /// without any added effort. It'll display a <see cref="ProgressRing"/>
    /// while loading.
    /// </summary>
    public sealed partial class ImageDataHost : UserControl
    {
        private readonly static DependencyProperty BitmapProperty =
            DependencyProperty.Register("Data", typeof(object), typeof(ImageDataHost), new PropertyMetadata(null));

        public object Data
        {
            get => GetValue(BitmapProperty);
            set => SetValue(BitmapProperty, value);
        }

        public ImageDataHost()
        {
            InitializeComponent();
            RegisterPropertyChangedCallback(BitmapProperty, SourceChanged);

            SourceChanged(null, null);
        }

        private async void SourceChanged(DependencyObject sender, DependencyProperty dp)
            => await ReloadImage();

        private async Task ReloadImage()
        {
            if (Data != null)
            {
                Spin.IsActive = true;
                var type = Data.GetType();

                if (type == typeof(SoftwareBitmap))
                {
                    MainImage.Source = await ((SoftwareBitmap)Data).AsBitmapSourceAsync();
                }
                else if (type == typeof(byte[]))
                {
                    MainImage.Source = await ((byte[])Data).ToBitmapImageAsync();
                }

                Spin.IsActive = false;
            }
        }
    }
}
