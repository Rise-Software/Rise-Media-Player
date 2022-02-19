using Windows.Storage.Search;

namespace Rise.App.Indexing
{
    public class QueryPresets
    {
        /// <summary>
        /// Query options for song indexing.
        /// </summary>
        public static readonly QueryOptions SongQueryOptions =
            new(CommonFileQuery.DefaultQuery,
            new string[]
            {
                ".mp3", ".wma", ".wav", ".ogg", ".flac", ".aiff", ".aac", ".m4a", ".wm", ".3gp", ".3gp2"
            })
            {
                FolderDepth = FolderDepth.Deep
            };

        /// <summary>
        /// Query options for playlist indexing.
        /// </summary>
        public static readonly QueryOptions PlaylistQueryOptions =
            new(CommonFileQuery.DefaultQuery,
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
            new(CommonFileQuery.DefaultQuery,
            new string[]
            {
                ".m2v", ".m4v", ".mp4", ".mov", ".asf", ".avi", ".wmv", ".mkv", ".mp4v", ".mod", ".wm", ".mpg4", ".mpv2", ".ogm", ".ogv", ".mpeg", ".mpg", ".ogx", ".mpe", ".m1v", ".m2ts"
            })
            {
                FolderDepth = FolderDepth.Deep
            };
    }
}
