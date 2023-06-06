using Rise.Models;
using System;
using System.Threading.Tasks;
using Windows.Web.Http;

namespace Rise.App.Helpers
{
    public static class MusixmatchHelper
    {
        private static async Task<string> GetStringAsync(Uri url)
        {
            using HttpClient httpClient = new();

            HttpResponseMessage httpResponse = new();

            string httpResponseBody = null;

            try
            {
                httpResponse = await httpClient.GetAsync(url);
                httpResponse.EnsureSuccessStatusCode();
                httpResponseBody = await httpResponse.Content.ReadAsStringAsync();
            }
            catch
            {
                httpResponseBody = null;
            }

            return httpResponseBody;
        }

        public static async Task<MusixmatchLyrics> GetLyricsAsync(string trackName, string artistName, UserToken token = null)
        {
            string userToken = App.SViewModel.MusixmatchLyricsToken;

            if (token != null && token.Message.Header.StatusCode == 200 && !string.IsNullOrEmpty(token.Message.Body.Token))
            {
                userToken = token.Message.Body.Token;
            }

            Uri url = new($@"https://apic-desktop.musixmatch.com/ws/1.1/matcher.lyrics.get?format=json&q_track={Uri.EscapeDataString(trackName)}&q_artist={Uri.EscapeDataString(artistName)}&user_language=en&subtitle_format=lrc&app_id=web-desktop-app-v1.0&usertoken={userToken}");

            string response = await GetStringAsync(url);

            MusixmatchLyrics lyrics = MusixmatchLyrics.FromJson(response);

            return lyrics;
        }

        // Duration is in seconds
        public static async Task<MusixmatchLyrics> GetLyricsAsync(string trackName, string artistName, int duration, UserToken token = null)
        {
            string userToken = "190523f77464fba06fa5f82a9bfab0aa9dc201244ecf5124a06d95";

            if (token != null && token.Message.Header.StatusCode == 200 && !string.IsNullOrEmpty(token.Message.Body.Token))
            {
                userToken = token.Message.Body.Token;
            }

            Uri url = new($@"https://apic-desktop.musixmatch.com/ws/1.1/matcher.lyrics.get?format=json&q_track={Uri.EscapeDataString(trackName)}&q_artist={Uri.EscapeDataString(artistName)}&q_duration={duration}&user_language=en&subtitle_format=mxm&app_id=web-desktop-app-v1.0&usertoken={userToken}");

            string response = await GetStringAsync(url);

            MusixmatchLyrics lyrics = MusixmatchLyrics.FromJson(response);

            return lyrics;
        }

        public static async Task<MusixmatchLyrics> GetLyricsAsync(string id, UserToken token = null)
        {
            string userToken = "190523f77464fba06fa5f82a9bfab0aa9dc201244ecf5124a06d95";

            if (token != null && token.Message.Header.StatusCode == 200 && !string.IsNullOrEmpty(token.Message.Body.Token))
            {
                userToken = token.Message.Body.Token;
            }

            Uri url = new Uri($@"https://apic-desktop.musixmatch.com/ws/1.1/track.lyrics.get?format=json&track_id={id}&user_language=en&subtitle_format=mxm&app_id=web-desktop-app-v1.0&usertoken={userToken}");

            string response = await GetStringAsync(url);

            MusixmatchLyrics lyrics = MusixmatchLyrics.FromJson(response);

            return lyrics;
        }

        public static async Task<SyncedLyrics> GetSyncedLyricsAsync(string trackName, string artistName, UserToken token = null)
        {
            string userToken = "190523f77464fba06fa5f82a9bfab0aa9dc201244ecf5124a06d95";

            if (token != null && token.Message.Header.StatusCode == 200 && !string.IsNullOrEmpty(token.Message.Body.Token))
            {
                userToken = token.Message.Body.Token;
            }

            Uri url = new($@"https://apic-desktop.musixmatch.com/ws/1.1/matcher.subtitle.get?format=json&q_track={Uri.EscapeDataString(trackName)}&q_artist={Uri.EscapeDataString(artistName)}&user_language=en&subtitle_format=mxm&app_id=web-desktop-app-v1.0&usertoken={userToken}");

            string response = await GetStringAsync(url);

            SyncedLyrics lyrics = SyncedLyrics.FromJson(response);

            return lyrics;
        }

        // Duration is in seconds
        public static async Task<SyncedLyrics> GetSyncedLyricsAsync(string trackName, string artistName, int duration, UserToken token = null)
        {
            string userToken = "190523f77464fba06fa5f82a9bfab0aa9dc201244ecf5124a06d95";

            if (token != null && token.Message.Header.StatusCode == 200 && !string.IsNullOrEmpty(token.Message.Body.Token))
            {
                userToken = token.Message.Body.Token;
            }

            Uri url = new($@"https://apic-desktop.musixmatch.com/ws/1.1/matcher.subtitle.get?format=json&q_track={Uri.EscapeDataString(trackName)}&q_artist={Uri.EscapeDataString(artistName)}&q_duration={duration}&user_language=en&subtitle_format=mxm&app_id=web-desktop-app-v1.0&usertoken={userToken}");

            string response = await GetStringAsync(url);

            SyncedLyrics lyrics = SyncedLyrics.FromJson(response);

            return lyrics;
        }

        public static async Task<SyncedLyrics> GetSyncedLyricsAsync(string id, UserToken token = null)
        {
            string userToken = "190523f77464fba06fa5f82a9bfab0aa9dc201244ecf5124a06d95";

            if (token != null && token.Message.Header.StatusCode == 200 && !string.IsNullOrEmpty(token.Message.Body.Token))
            {
                userToken = token.Message.Body.Token;
            }

            Uri url = new($@"https://apic-desktop.musixmatch.com/ws/1.1/track.subtitle.get?format=json&track_id={id}&user_language=en&subtitle_format=mxm&app_id=web-desktop-app-v1.0&usertoken={userToken}");

            string response = await GetStringAsync(url);

            SyncedLyrics lyrics = SyncedLyrics.FromJson(response);

            return lyrics;
        }

        public static async Task<MusixmatchTrack> GetTrackAsync(string trackName, string artistName, UserToken token = null)
        {
            string userToken = "190523f77464fba06fa5f82a9bfab0aa9dc201244ecf5124a06d95";

            if (token != null && token.Message.Header.StatusCode == 200 && !string.IsNullOrEmpty(token.Message.Body.Token))
            {
                userToken = token.Message.Body.Token;
            }

            Uri url = new($@"https://apic-desktop.musixmatch.com/ws/1.1/matcher.track.get?format=json&q_track={Uri.EscapeDataString(trackName)}&q_artist={Uri.EscapeDataString(artistName)}&user_language=en&subtitle_format=mxm&app_id=web-desktop-app-v1.0&usertoken={userToken}");

            string response = await GetStringAsync(url);

            MusixmatchTrack track = MusixmatchTrack.FromJson(response);

            return track;
        }

        // Duration is in seconds
        public static async Task<MusixmatchTrack> GetTrackAsync(string trackName, string artistName, int duration, UserToken token = null)
        {
            string userToken = "190523f77464fba06fa5f82a9bfab0aa9dc201244ecf5124a06d95";

            if (token != null && token.Message.Header.StatusCode == 200 && !string.IsNullOrEmpty(token.Message.Body.Token))
            {
                userToken = token.Message.Body.Token;
            }

            Uri url = new($@"https://apic-desktop.musixmatch.com/ws/1.1/matcher.track.get?format=json&q_track={Uri.EscapeDataString(trackName)}&q_artist={Uri.EscapeDataString(artistName)}&q_duration={duration}&user_language=en&subtitle_format=mxm&app_id=web-desktop-app-v1.0&usertoken={userToken}");

            string response = await GetStringAsync(url);

            MusixmatchTrack track = MusixmatchTrack.FromJson(response);

            return track;
        }

        public static async Task<UserToken> GetUserTokenAsync()
        {
            Uri url = new(@"https://apic.musixmatch.com/ws/1.1/token.get?app_id=community-app-v1.0&guid=56495545-24fd-4a2f-95dc-947a250c4550&signature=NlaOMebAhGJEjN0zRTwyD%2FSspUo%3D&signature_protocol=sha1");

            string response = await GetStringAsync(url);

            UserToken token = UserToken.FromJson(response);

            return token;
        }
    }
}
