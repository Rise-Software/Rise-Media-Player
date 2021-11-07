using Windows.Storage.Search;

namespace RMP.App.Indexing
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
        /// Query options for video indexing.
        /// </summary>
        public static readonly QueryOptions VideoQueryOptions =
            new QueryOptions(CommonFileQuery.DefaultQuery,
            new string[]
            {
                ".mp4"
            })
            {
                FolderDepth = FolderDepth.Deep
            };
    }
}
