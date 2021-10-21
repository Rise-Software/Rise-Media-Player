namespace RMP.App
{
    public static class Enums
    {
        /// <summary>
        /// Different sorting methods for the song list.
        /// </summary>
        public enum SortMethods
        {
            /// <summary>
            /// Sorts based on Disc, then based on track.
            /// </summary>
            Default,

            /// <summary>
            /// Sorts based on title.
            /// </summary>
            Title,

            /// <summary>
            /// Sorts based on album.
            /// </summary>
            Album,

            /// <summary>
            /// Sorts based on artist.
            /// </summary>
            Artist,

            /// <summary>
            /// Sorts based on album artist.
            /// </summary>
            AlbumArtist,

            /// <summary>
            /// Sorts randomly.
            /// </summary>
            Random
        }
    }
}
