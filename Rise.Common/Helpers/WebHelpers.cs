using Rise.Common.Constants;
using System;
using System.Threading.Tasks;
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
            return connectionProfile != null 
                && (connectionProfile.GetNetworkConnectivityLevel() == NetworkConnectivityLevel.InternetAccess
                || connectionProfile.GetNetworkConnectivityLevel() == NetworkConnectivityLevel.ConstrainedInternetAccess);
        }

        /// <summary>
        /// Gets a <see cref="MediaPlaybackItem"/> from the provided song Uri.
        /// </summary>
        public static async Task<MediaPlaybackItem> GetSongFromUriAsync(Uri uri, string title = null, string subtitle = null, string thumbnail = null, bool fetchInfo = false)
        {
            var source = MediaSource.CreateFromUri(uri);

            if (fetchInfo)
                await source.OpenAsync();
            else
            {
                source.CustomProperties["Title"] = title ?? "Online song";
                source.CustomProperties["Artists"] = "UnknownArtistResource";
                source.CustomProperties["Year"] = 0u;
            }

            source.CustomProperties["Length"] = source.Duration;

            var media = new MediaPlaybackItem(source);
            var props = media.GetDisplayProperties();

            if (fetchInfo)
            {
                source.CustomProperties["Title"] = title ?? props.MusicProperties.Title;
                source.CustomProperties["Artists"] = subtitle ?? props.MusicProperties.Artist;
                source.CustomProperties["Year"] = 0u;
            }
            else
            {
                props.Type = MediaPlaybackType.Music;
                props.MusicProperties.Title = title ?? "Online song";
                props.MusicProperties.Artist = subtitle ?? "UnknownArtistResource";
            }

            props.Thumbnail ??= RandomAccessStreamReference.CreateFromUri(new(thumbnail ?? URIs.MusicThumb));
            media.ApplyDisplayProperties(props);

            return media;
        }

        /// <summary>
        /// Gets a <see cref="MediaPlaybackItem"/> from the provided video Uri.
        /// </summary>
        public static async Task<MediaPlaybackItem> GetVideoFromUriAsync(Uri uri, string title = null, string subtitle = null, string thumbnail = null, bool fetchInfo = false)
        {
            var source = MediaSource.CreateFromUri(uri);

            if (fetchInfo)
                await source.OpenAsync();
            else
            {
                source.CustomProperties["Title"] = title ?? "Online video";
                source.CustomProperties["Artists"] = "UnknownArtistResource";
                source.CustomProperties["Year"] = 0u;
            }

            source.CustomProperties["Length"] = source.Duration;

            var media = new MediaPlaybackItem(source);
            var props = media.GetDisplayProperties();

            if (fetchInfo)
            {
                source.CustomProperties["Title"] = title ?? props.VideoProperties.Title;
                source.CustomProperties["Artists"] = subtitle ?? props.VideoProperties.Subtitle;
            } else
            {
                props.Type = MediaPlaybackType.Video;
                props.VideoProperties.Title = title ?? "Online video";
                props.VideoProperties.Subtitle = subtitle ?? "UnknownArtistResource";
            }

            props.Thumbnail ??= RandomAccessStreamReference.CreateFromUri(new(thumbnail ?? URIs.VideoThumb));
            media.ApplyDisplayProperties(props);

            return media;
        }
    }
}
