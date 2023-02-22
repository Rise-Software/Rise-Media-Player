using CommunityToolkit.Mvvm.Input;
using Microsoft.Toolkit.Uwp.UI;
using Rise.Common.Extensions;
using Rise.Common.Helpers;
using Rise.Common.Interfaces;
using Rise.Data.ViewModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Rise.App.ViewModels
{
    /// <summary>
    /// A ViewModel for collections of items that can
    /// be played.
    /// </summary>
    public sealed partial class MediaCollectionViewModel : IDisposable
    {
        private readonly MediaPlaybackViewModel _player;
        private readonly IList<SongViewModel> _songs;

        public AdvancedCollectionView Items { get; }

        public ICommand SortByCommand { get; }
        public ICommand UpdateSortDirectionCommand { get; }

        /// <summary>
        /// Initializes a new instance of this ViewModel.
        /// </summary>
        /// <param name="defaultProperty">Name of the default property to sort
        /// the item collection.</param>
        /// <param name="itemSource">Source of items for the underlying
        /// ViewModel.</param>
        /// <param name="songs">The collection of songs to use. Only needed
        /// for albums, artists, and genres.</param>
        /// <param name="pvm">An instance of <see cref="MediaPlaybackViewModel"/>
        /// responsible for playback management.</param>
        /// <param name="canSort">A delegate indicating whether sorting
        /// is possible.</param>
        public MediaCollectionViewModel(string defaultProperty,
            IList itemSource,
            IList<SongViewModel> songs,
            MediaPlaybackViewModel pvm)
        {
            Items = new(itemSource);
            Items.SortDescriptions.Add(new(defaultProperty, SortDirection.Ascending));

            _songs = songs;
            _player = pvm;
        }

        public void Dispose()
        {
        }
    }

    // Playback
    public partial class MediaCollectionViewModel
    {
        /// <summary>
        /// Helps cancel a group of Tasks that starts playback.
        /// </summary>
        private readonly CancellableTaskHelper PlaybackCancelHelper = new();

        [RelayCommand(AllowConcurrentExecutions = true)]
        private async Task PlayFromItemAsync(object parameter)
        {
            try
            {
                await PlaybackCancelHelper.CompletePendingAsync(new CancellationToken());
                await PlaybackCancelHelper.RunAsync(GetPlayFromTask(parameter));
            }
            catch (OperationCanceledException) { }
        }

        [RelayCommand(AllowConcurrentExecutions = true)]
        private async Task PlaySingleItemAsync(object parameter)
        {
            if (parameter == null)
            {
                await PlayFromItemAsync(parameter);
                return;
            }

            try
            {
                await PlaybackCancelHelper.CompletePendingAsync(new CancellationToken());
                await PlaybackCancelHelper.RunAsync(GetPlaySingleTask(parameter));
            }
            catch (OperationCanceledException) { }
        }

        [RelayCommand(AllowConcurrentExecutions = true)]
        private Task ShuffleFromItemAsync(object parameter)
        {
            _player.ShuffleEnabled = true;
            return PlayFromItemAsync(parameter);
        }

        [RelayCommand(AllowConcurrentExecutions = true)]
        private Task ShuffleSingleItemAsync(object parameter)
        {
            _player.ShuffleEnabled = true;
            return PlaySingleItemAsync(parameter);
        }

        private Task GetPlaySingleTask(object parameter)
        {
            if (parameter is AlbumViewModel album)
                return PlaySingleAlbumAsync(album, PlaybackCancelHelper.Token);
            else if (parameter is ArtistViewModel artist)
                PlaySingleArtistAsync(artist, PlaybackCancelHelper.Token);
            else if (parameter is GenreViewModel genre)
                return PlaySingleGenreAsync(genre, PlaybackCancelHelper.Token);
            else if (parameter is PlaylistViewModel playlist)
                return PlaySinglePlaylistAsync(playlist, PlaybackCancelHelper.Token);

            return PlaySingleItemAsync((IMediaItem)parameter, PlaybackCancelHelper.Token);
        }

        private Task GetPlayFromTask(object parameter)
        {
            if (parameter is IMediaItem item)
                return PlayFromItemAsync(item, PlaybackCancelHelper.Token);
            else if (parameter is AlbumViewModel album)
                return PlayFromAlbumAsync(album, PlaybackCancelHelper.Token);
            else if (parameter is ArtistViewModel artist)
                return PlayFromArtistAsync(artist, PlaybackCancelHelper.Token);
            else if (parameter is GenreViewModel genre)
                return PlayFromGenreAsync(genre, PlaybackCancelHelper.Token);
            else if (parameter is PlaylistViewModel playlist)
                return PlayFromPlaylistAsync(playlist, PlaybackCancelHelper.Token);

            return GetDefaultPlayFromTask();
        }

        private Task GetDefaultPlayFromTask()
        {
            var type = Items.First().GetType();
            if (type == typeof(AlbumViewModel))
                return PlayFromAlbumAsync(null, PlaybackCancelHelper.Token);
            else if (type == typeof(ArtistViewModel))
                return PlayFromArtistAsync(null, PlaybackCancelHelper.Token);
            else if (type == typeof(GenreViewModel))
                return PlayFromGenreAsync(null, PlaybackCancelHelper.Token);
            else if (type == typeof(PlaylistViewModel))
                return PlayFromPlaylistAsync(null, PlaybackCancelHelper.Token);

            return PlayFromItemAsync(null, PlaybackCancelHelper.Token);
        }
    }

    // PlaySingle* methods
    public partial class MediaCollectionViewModel
    {
        public Task PlaySingleItemAsync(IMediaItem item, CancellationToken token)
        {
            return _player.PlaySingleItemAsync(item, token);
        }

        public Task PlaySingleAlbumAsync(AlbumViewModel album, CancellationToken token)
        {
            var acv = new AdvancedCollectionView(_songs.ToList());
            var songs = new List<SongViewModel>();
            token.ThrowIfCancellationRequested();

            acv.SortDescriptions.Add(new SortDescription("Disc", SortDirection.Ascending));
            acv.SortDescriptions.Add(new SortDescription("Track", SortDirection.Ascending));
            token.ThrowIfCancellationRequested();

            acv.Filter = s => ((SongViewModel)s).Album == album.Title;
            songs.AddRange(acv.CloneList<object, SongViewModel>());
            acv.Filter = null;
            token.ThrowIfCancellationRequested();

            return _player.PlayItemsAsync(songs, token);
        }

        public Task PlaySingleArtistAsync(ArtistViewModel artist, CancellationToken token)
        {
            var acv = new AdvancedCollectionView(_songs.ToList());
            var songs = new List<SongViewModel>();
            token.ThrowIfCancellationRequested();

            acv.SortDescriptions.Add(new SortDescription("Title", SortDirection.Ascending));
            token.ThrowIfCancellationRequested();

            acv.Filter = s => ((SongViewModel)s).Artist == artist.Name ||
                ((SongViewModel)s).AlbumArtist == artist.Name;
            songs.AddRange(acv.CloneList<object, SongViewModel>());
            acv.Filter = null;
            token.ThrowIfCancellationRequested();

            return _player.PlayItemsAsync(songs, token);
        }

        public Task PlaySingleGenreAsync(GenreViewModel genre, CancellationToken token)
        {
            var acv = new AdvancedCollectionView(_songs.ToList());
            var songs = new List<SongViewModel>();
            token.ThrowIfCancellationRequested();

            acv.SortDescriptions.Add(new SortDescription("Disc", SortDirection.Ascending));
            acv.SortDescriptions.Add(new SortDescription("Track", SortDirection.Ascending));
            token.ThrowIfCancellationRequested();

            acv.Filter = s => ((SongViewModel)s).Genres == genre.Name;
            songs.AddRange(acv.CloneList<object, SongViewModel>());
            acv.Filter = null;
            token.ThrowIfCancellationRequested();

            return _player.PlayItemsAsync(songs, token);
        }

        public Task PlaySinglePlaylistAsync(PlaylistViewModel playlist, CancellationToken token)
        {
            var toPlay = playlist.Songs.ToList<IMediaItem>();
            token.ThrowIfCancellationRequested();

            toPlay.AddRange(playlist.Videos);
            token.ThrowIfCancellationRequested();

            return _player.PlayItemsAsync(toPlay, token);
        }
    }

    // PlayFrom* methods
    public partial class MediaCollectionViewModel
    {
        public Task PlayFromItemAsync(IMediaItem item, CancellationToken token)
        {
            var items = Items.CloneList<object, IMediaItem>();
            token.ThrowIfCancellationRequested();

            if (item != null)
            {
                int index = items.IndexOf(item);
                token.ThrowIfCancellationRequested();

                items.MoveRangeToEnd(0, index - 1);
                token.ThrowIfCancellationRequested();
            }

            return _player.PlayItemsAsync(items, token);
        }

        public Task PlayFromAlbumAsync(AlbumViewModel album, CancellationToken token)
        {
            var items = Items.CloneList<object, AlbumViewModel>();
            var songs = _songs.CloneList<object, SongViewModel>().
                OrderBy(s => s.Disc).ThenBy(s => s.Track);

            token.ThrowIfCancellationRequested();

            var toPlay = new List<SongViewModel>();
            token.ThrowIfCancellationRequested();

            if (album != null)
            {
                int index = items.IndexOf(album);
                token.ThrowIfCancellationRequested();

                items.MoveRangeToEnd(0, index - 1);
                token.ThrowIfCancellationRequested();
            }

            foreach (var itm in items)
            {
                toPlay.AddRange(songs.Where(s => s.Album == itm.Title));
                token.ThrowIfCancellationRequested();
            }

            return _player.PlayItemsAsync(toPlay, token);
        }

        public Task PlayFromArtistAsync(ArtistViewModel artist, CancellationToken token)
        {
            var items = Items.CloneList<object, ArtistViewModel>();
            var songs = _songs.CloneList<object, SongViewModel>().OrderBy(s => s.Title);

            token.ThrowIfCancellationRequested();

            var toPlay = new List<SongViewModel>();
            token.ThrowIfCancellationRequested();

            if (artist != null)
            {
                int index = items.IndexOf(artist);
                token.ThrowIfCancellationRequested();

                items.MoveRangeToEnd(0, index - 1);
                token.ThrowIfCancellationRequested();
            }

            foreach (var itm in items)
            {
                toPlay.AddRange(songs.Where(s => s.Artist == itm.Name || s.AlbumArtist == itm.Name));
                token.ThrowIfCancellationRequested();
            }

            return _player.PlayItemsAsync(toPlay, token);
        }

        public Task PlayFromGenreAsync(GenreViewModel genre, CancellationToken token)
        {
            var items = Items.CloneList<object, GenreViewModel>();
            var songs = _songs.CloneList<object, SongViewModel>().OrderBy(s => s.Title);

            token.ThrowIfCancellationRequested();

            var toPlay = new List<SongViewModel>();
            token.ThrowIfCancellationRequested();

            if (genre != null)
            {
                int index = items.IndexOf(genre);
                token.ThrowIfCancellationRequested();

                items.MoveRangeToEnd(0, index - 1);
                token.ThrowIfCancellationRequested();
            }

            foreach (var itm in items)
            {
                toPlay.AddRange(songs.Where(s => s.Genres == itm.Name));
                token.ThrowIfCancellationRequested();
            }

            return _player.PlayItemsAsync(toPlay, token);
        }

        public Task PlayFromPlaylistAsync(PlaylistViewModel playlist, CancellationToken token)
        {
            var items = Items.CloneList<object, PlaylistViewModel>();
            token.ThrowIfCancellationRequested();

            var toPlay = new List<IMediaItem>();
            token.ThrowIfCancellationRequested();

            if (playlist != null)
            {
                int index = items.IndexOf(playlist);
                token.ThrowIfCancellationRequested();

                items.MoveRangeToEnd(0, index - 1);
                token.ThrowIfCancellationRequested();
            }

            foreach (var itm in items)
            {
                toPlay.AddRange(itm.Songs.OrderBy(s => s.Title));
                token.ThrowIfCancellationRequested();

                toPlay.AddRange(itm.Videos.OrderBy(v => v.Title));
                token.ThrowIfCancellationRequested();
            }

            return _player.PlayItemsAsync(toPlay, token);
        }
    }
}
