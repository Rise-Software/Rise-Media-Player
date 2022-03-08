using Rise.Common.Constants;
using Windows.Storage.Search;

namespace Rise.Common
{
    public class QueryPresets
    {
        /// <summary>
        /// Query options for song indexing.
        /// </summary>
        public static readonly QueryOptions SongQueryOptions =
            new(CommonFileQuery.DefaultQuery, SupportedFileTypes.MusicFiles)
            {
                FolderDepth = FolderDepth.Deep
            };

        /// <summary>
        /// Query options for playlist indexing.
        /// </summary>
        public static readonly QueryOptions PlaylistQueryOptions =
            new(CommonFileQuery.DefaultQuery, SupportedFileTypes.PlaylistFiles)
            {
                FolderDepth = FolderDepth.Deep
            };

        /// <summary>
        /// Query options for video indexing.
        /// </summary>
        public static readonly QueryOptions VideoQueryOptions =
            new(CommonFileQuery.DefaultQuery, SupportedFileTypes.VideoFiles)
            {
                FolderDepth = FolderDepth.Deep
            };
    }
}
