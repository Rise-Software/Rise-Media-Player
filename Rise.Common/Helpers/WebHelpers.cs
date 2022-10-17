using Rise.Common.Constants;
using System;
using Windows.Media;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Networking.Connectivity;
using Windows.Storage.Streams;

namespace Rise.Common.Helpers
{
    public static class WebHelpers
    {
        /// <summary>
        /// Checks whether internet access is available.
        /// </summary>
        /// <returns>true if access is available, false otherwise.</returns>
        /// <remarks>This method only checks for the available network resources.</remarks>
        public static bool IsInternetAccessAvailable()
        {
            ConnectionProfile connectionProfile = NetworkInformation.GetInternetConnectionProfile();
            return connectionProfile != null && connectionProfile.
                GetNetworkConnectivityLevel() == NetworkConnectivityLevel.InternetAccess;
        }

        /// <summary>
        /// Gets a <see cref="MediaPlaybackItem"/> from the provided song Uri.
        /// </summary>
        public static MediaPlaybackItem GetSongFromUri(Uri uri)
        {
            var media = GetMediaFromUri(uri, "Online song");
            var props = media.GetDisplayProperties();

            props.Type = MediaPlaybackType.Music;
            props.MusicProperties.Title = "Online song";
            props.MusicProperties.Artist = "UnknownArtistResource";
            props.Thumbnail = RandomAccessStreamReference.CreateFromUri(new(URIs.MusicThumb));

            media.ApplyDisplayProperties(props);
            return media;
        }

        /// <summary>
        /// Gets a <see cref="MediaPlaybackItem"/> from the provided video Uri.
        /// </summary>
        public static MediaPlaybackItem GetVideoFromUri(Uri uri)
        {
            var media = GetMediaFromUri(uri, "Online video");
            var props = media.GetDisplayProperties();

            props.Type = MediaPlaybackType.Video;
            props.VideoProperties.Title = "Online video";
            props.VideoProperties.Subtitle = "UnknownArtistResource";
            props.Thumbnail = RandomAccessStreamReference.CreateFromUri(new(URIs.MusicThumb));

            media.ApplyDisplayProperties(props);
            return media;
        }

        private static MediaPlaybackItem GetMediaFromUri(Uri uri, string title)
        {
            var source = MediaSource.CreateFromUri(uri);

            source.CustomProperties["Title"] = title;
            source.CustomProperties["Artists"] = "UnknownArtistResource";
            source.CustomProperties["Length"] = source.Duration;
            source.CustomProperties["Year"] = 0u;

            var media = new MediaPlaybackItem(source);
            return media;
        }
    }
}
