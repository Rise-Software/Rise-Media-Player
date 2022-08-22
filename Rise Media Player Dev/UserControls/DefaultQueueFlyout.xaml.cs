using Rise.Common.Interfaces;
using Windows.Media.Playback;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Rise.App.UserControls
{
    /// <summary>
    /// A flyout that shows queued media. Supports displaying queues with
    /// <see cref="IMediaItem"/>.
    /// </summary>
    public sealed partial class DefaultQueueFlyout : UserControl
    {
        public static readonly DependencyProperty QueuedItemsSourceProperty =
            DependencyProperty.Register(nameof(QueuedItemsSource), typeof(object),
                typeof(DefaultQueueFlyout), new PropertyMetadata(null));

        public object QueuedItemsSource
        {
            get => GetValue(QueuedItemsSourceProperty);
            set => SetValue(QueuedItemsSourceProperty, value);
        }

        public DefaultQueueFlyout()
        {
            InitializeComponent();
        }
    }
}
