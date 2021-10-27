namespace RMP.App.Common
{
    public static class Enums
    {
        /// <summary>
        /// Different sorting methods for the lists/grids.
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
            /// Sorts based on year.
            /// </summary>
            Year,

            /// <summary>
            /// Sorts based on genre.
            /// </summary>
            Genre,

            /// <summary>
            /// Sorts randomly.
            /// </summary>
            Random
        }

        /// <summary>
        /// The different expanders available for the Expander UserControl.
        /// </summary>
        public enum ExpanderStyles
        {
            /// <summary>
            /// The default expander style.
            /// </summary>
            Default,

            /// <summary>
            /// Static style, doesn't expand.
            /// </summary>
            Static,

            /// <summary>
            /// Button style, supports click event handlers.
            /// </summary>
            Button,

            /// <summary>
            /// Transparent, same as static but without a background or border.
            /// </summary>
            Transparent
        }
    }
}
