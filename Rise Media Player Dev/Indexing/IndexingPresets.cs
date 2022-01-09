using Windows.Storage.Search;

namespace Rise.App.Indexing
{
    public class QueryPresets
    {
        /// <summary>
        /// Query options for song indexing.
        /// </summary>
        public static readonly QueryOptions SongQueryOptions =
            new QueryOptions(CommonFileQuery.DefaultQuery,
            new string[]
            {
                ".mp3", ".wma", ".wav", ".ogg", ".flac", ".aiff", ".aac", ".m4a"
            })
            {
                FolderDepth = FolderDepth.Deep
            };

        /// <summary>
        /// Query options for playlist indexing.
        /// </summary>
        public static readonly QueryOptions PlaylistQueryOptions =
            new QueryOptions(CommonFileQuery.DefaultQuery,
            new string[]
            {
                ".m3u", ".m3u8", // ".wpl", ".zpl", ".asx", ".pls", ".xspf"
            })
            {
                FolderDepth = FolderDepth.Deep
            };

        /// <summary>
        /// Query options for video indexing.
        /// </summary>
        public static readonly QueryOptions VideoQueryOptions =
            new QueryOptions(CommonFileQuery.DefaultQuery,
            new string[]
            {
                ".m4v", ".mp4", ".mov", ".asf", ".avi", ".wmv", ".mkv"
            })
            {
                FolderDepth = FolderDepth.Deep
            };
    }
}
