using System;
using System.Threading.Tasks;
using Microsoft.Toolkit.Uwp.UI.Helpers;
using Windows.Media.Playback;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Rise.App.UserControls
{
    /// <summary>
    /// Custom media player element implementation for RiseMP.
    /// </summary>
    public sealed partial class RiseMediaPlayerElement : MediaPlayerElement
    {
        private DependencyPropertyWatcher<MediaPlayer> playerWatcher;

        /// <summary>
        /// Gets or sets the player's visibility.
        /// </summary>
        public Visibility MediaPlayerVisibility
        {
            get => (Visibility)GetValue(MediaPlayerVisibilityProperty);
            set => SetValue(MediaPlayerVisibilityProperty, value);
        }
    }

    // Dependency Properties
    public sealed partial class RiseMediaPlayerElement : MediaPlayerElement
    {
        public readonly static DependencyProperty MediaPlayerVisibilityProperty =
            DependencyProperty.Register(nameof(MediaPlayerVisibility), typeof(Visibility),
                typeof(RiseMediaPlayerElement), new PropertyMetadata(Visibility.Visible));
    }

    // Event handlers
    public sealed partial class RiseMediaPlayerElement : MediaPlayerElement
    {
        private async void OnVolumeChanged(MediaPlayer sender, object args)
        {
            if (!sender.IsMuted)
                await HandleVolumeChangedAsync(sender.Volume);
        }

        private async void OnIsMutedChanged(MediaPlayer sender, object args)
        {
            if (!sender.IsMuted)
                await HandleVolumeChangedAsync(sender.Volume);
            else
                await HandleMutedAsync();
        }

        private async Task HandleVolumeChangedAsync(double newVolume)
        {
            var state = newVolume switch
            {
                0 => "NoVolumeState",
                < 0.33 => "LowVolumeState",
                < 0.66 => "MidVolumeState",
                _ => "HighVolumeState",
            };

            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                VisualStateManager.GoToState(TransportControls, state, true);
            });
        }

        private async Task HandleMutedAsync()
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                VisualStateManager.GoToState(TransportControls, "MuteState", true);
            });
        }

        private void RegisterVolumeChangedCallbacks()
        {
            MediaPlayer.VolumeChanged += OnVolumeChanged;
            MediaPlayer.IsMutedChanged += OnIsMutedChanged;
        }

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (MediaPlayer != null)
            {
                RegisterVolumeChangedCallbacks();
            }
            else
            {
                playerWatcher = new(this, "MediaPlayer");
                playerWatcher.PropertyChanged += (s, e) => RegisterVolumeChangedCallbacks();
            }

            await HandleVolumeChangedAsync(1);
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            playerWatcher.Dispose();
        }
    }

    // Constructor
    public sealed partial class RiseMediaPlayerElement : MediaPlayerElement
    {
        public RiseMediaPlayerElement()
        {
            DefaultStyleKey = typeof(RiseMediaPlayerElement);

            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
        }
    }
}
