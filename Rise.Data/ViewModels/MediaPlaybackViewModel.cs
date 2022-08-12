using AudioVisualizer;
using Rise.Common.Helpers;
using Rise.Common.Interfaces;
using Rise.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Windows.Media.Playback;

namespace Rise.Data.ViewModels
{
    /// <summary>
    /// This class contains properties and methods that
    /// help when it comes to controlling media playback.
    /// </summary>
    public partial class MediaPlaybackViewModel : ViewModel
    {
        /// <summary>
        /// List of media items that are currently queued for playback.
        /// </summary>
        public readonly SafeObservableCollection<IMediaItem> QueuedItems = new();

        private readonly List<MediaPlayerEffect> _effects = new();
        /// <summary>
        /// Gets the added types of effects.
        /// </summary>
        public IReadOnlyCollection<MediaPlayerEffect> Effects => _effects.AsReadOnly();

        private IMediaItem _playingItem;
        /// <summary>
        /// Gets the media item that is currently playing.
        /// </summary>
        public IMediaItem PlayingItem
        {
            get => _playingItem;
            private set
            {
                Set(ref _playingItem, value);
                PlayingItemChanged?.Invoke(this, _playingItem);
            }
        }

        private MediaPlayer _player;
        /// <summary>
        /// Gets the current <see cref="MediaPlayer"/> instance.
        /// Lazily instantiated to prevent Windows from showing the
        /// SMTC as soon as the app is opened.
        /// </summary>
        public MediaPlayer Player
        {
            get
            {
                if (_player == null)
                {
                    _player = CreatePlayerInstance();
                    foreach (var eff in _effects)
                        AddEffectInternal(eff);
                }
                return _player;
            }
        }

        private bool _playerCreated;
        /// <summary>
        /// Whether the main player has been created.
        /// </summary>
        public bool PlayerCreated
        {
            get => _playerCreated;
            private set => Set(ref _playerCreated, value);
        }

        /// <summary>
        /// The media playback list. It is permanently associated with
        /// the player, due to the fact that we don't ever dispose it.
        /// </summary>
        private readonly MediaPlaybackList PlaybackList = new();

        /// <summary>
        /// Whether the items in the playback list are played in a random
        /// order.
        /// </summary>
        public bool ShuffleEnabled
        {
            get => PlaybackList.ShuffleEnabled;
            set
            {
                PlaybackList.ShuffleEnabled = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Playback source for visualizers.
        /// </summary>
        public PlaybackSource VisualizerPlaybackSource { get; private set; }
    }

    // Events
    public partial class MediaPlaybackViewModel
    {
        /// <summary>
        /// Occurs when the media player is disposed and recreated. Also
        /// gets fired when the initial lazy instance takes place.
        /// </summary>
        public event EventHandler<MediaPlayer> MediaPlayerRecreated;

        /// <summary>
        /// Occurs when the currently playing item changes.
        /// </summary>
        public event EventHandler<IMediaItem> PlayingItemChanged;
    }

    // Methods, Event handling
    public partial class MediaPlaybackViewModel
    {
        /// <summary>
        /// Helps cancel a group of Tasks that starts playback.
        /// </summary>
        private readonly CancellableTaskHelper PlaybackCancelHelper = new();

        /// <summary>
        /// Adds the specified effect to the player.
        /// </summary>
        /// <remarks>If the player hasn't yet been created, the effect will still
        /// be applied as soon as it is.</remarks>
        public void AddEffect(MediaPlayerEffect effect)
        {
            AddEffectInternal(effect);
            _effects.Add(effect);
        }

        private void AddEffectInternal(MediaPlayerEffect effect)
        {
            if (_player != null)
            {
                if (effect.IsAudioEffect)
                    _player.AddAudioEffect(effect.EffectClassType.FullName, effect.IsOptional, effect.Configuration);
                else
                    _player.AddVideoEffect(effect.EffectClassType.FullName, effect.IsOptional, effect.Configuration);
            }
        }

        /// <summary>
        /// Removes all the effects from the player.
        /// </summary>
        public void ClearEffects()
        {
            if (_playerCreated)
                _player.RemoveAllEffects();

            _effects.Clear();
        }

        /// <summary>
        /// Begins playback of an <see cref="IMediaItem"/>.
        /// </summary>
        /// <param name="item">Item to play.</param>
        /// <remarks>This method will automatically be canceled if necessary
        /// without throwing <see cref="OperationCanceledException"/>.</remarks>
        public async Task PlaySingleItemAsync(IMediaItem item)
        {
            try
            {
                await PlaySingleItemAsync(item, new CancellationToken());
            }
            catch (OperationCanceledException) { }
        }

        /// <summary>
        /// Begins playback of an <see cref="IMediaItem"/>.
        /// </summary>
        /// <param name="item">Item to play.</param>
        public async Task PlaySingleItemAsync(IMediaItem item, CancellationToken token)
        {
            await PlaybackCancelHelper.CompletePendingAsync(token);
            await PlaybackCancelHelper.RunAsync(PlaySingleItemImpl(item, PlaybackCancelHelper.Token));
        }

        /// <summary>
        /// Begins playback of a collection of <see cref="IMediaItem"/>.
        /// </summary>
        /// <param name="items">Items to play.</param>
        /// <remarks>This method will automatically be canceled if necessary
        /// without throwing <see cref="OperationCanceledException"/>.</remarks>
        public async Task PlayItemsAsync(IEnumerable<IMediaItem> items)
        {
            try
            {
                await PlayItemsAsync(items, new CancellationToken());
            }
            catch (OperationCanceledException) { }
        }

        /// <summary>
        /// Begins playback of a collection of <see cref="IMediaItem"/>.
        /// </summary>
        /// <param name="items">Items to play.</param>
        public async Task PlayItemsAsync(IEnumerable<IMediaItem> items, CancellationToken token)
        {
            await PlaybackCancelHelper.CompletePendingAsync(token);
            await PlaybackCancelHelper.RunAsync(PlayItemsImpl(items, PlaybackCancelHelper.Token));
        }

        private async Task PlaySingleItemImpl(IMediaItem item, CancellationToken token)
        {
            ResetPlayback();

            token.ThrowIfCancellationRequested();
            var playItem = await item.AsPlaybackItemAsync();

            // We add stuff manually here because the playback list
            // isn't used for single items
            QueuedItems.Add(item);
            PlayingItem = item;

            token.ThrowIfCancellationRequested();
            Player.Source = playItem;
            Player.Play();
        }

        private async Task PlayItemsImpl(IEnumerable<IMediaItem> items, CancellationToken token)
        {
            ResetPlayback();

            int i = 0;
            foreach (var item in items)
            {
                token.ThrowIfCancellationRequested();

                var playItem = await item.AsPlaybackItemAsync();
                PlaybackList.Items.Add(playItem);
                QueuedItems.Add(item);

                // Start playback right after adding the first item...
                if (i == 0)
                {
                    token.ThrowIfCancellationRequested();
                    Player.Source = PlaybackList;
                    Player.Play();

                    // ...and never again.
                    i++;
                }
            }
        }

        private void OnCurrentItemChanged(MediaPlaybackList sender, CurrentMediaPlaybackItemChangedEventArgs args)
        {
            if (args.Reason == MediaPlaybackItemChangedReason.Error)
            {
                System.Diagnostics.Debug.WriteLine("Error when going to next item.");
                return;
            }

            if (sender.CurrentItem != null)
            {
                PlayingItem = QueuedItems[(int)sender.CurrentItemIndex];
            }
        }
    }

    // Constructors, Initializers
    public partial class MediaPlaybackViewModel
    {
        public MediaPlaybackViewModel()
        {
            PlaybackList.CurrentItemChanged += OnCurrentItemChanged;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="MediaPlayer"/>,
        /// invokes the <see cref="MediaPlayerRecreated"/> event and returns
        /// the new instance.
        /// </summary>
        private MediaPlayer CreatePlayerInstance()
        {
            var player = new MediaPlayer();

            PlayerCreated = true;
            MediaPlayerRecreated?.Invoke(this, player);

            VisualizerPlaybackSource = PlaybackSource.CreateFromMediaPlayer(player);

            return player;
        }

        /// <summary>
        /// Fully resets playback by clearing lists and setting the current
        /// item to null.
        /// </summary>
        private void ResetPlayback()
        {
            PlaybackList.Items.Clear();
            QueuedItems.Clear();

            PlayingItem = null;
        }
    }
}
