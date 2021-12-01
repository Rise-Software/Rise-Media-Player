using Windows.UI.Xaml.Controls;

namespace Rise.App.Common
{
    public static class Enums
    {
        /// <summary>
        /// The different expanders available for the
        /// Expander UserControl
        /// </summary>
        public enum ExpanderStyles
        {
            /// <summary>
            /// The default expander style
            /// </summary>
            Default,

            /// <summary>
            /// Static style, doesn't expand
            /// </summary>
            Static,

            /// <summary>
            /// Button style, supports click event
            /// handlers
            /// </summary>
            Button,

            /// <summary>
            /// Transparent, same as static but without
            /// a background or border
            /// </summary>
            Transparent,

            /// <summary>
            /// Disabled style, uses secondary colors,
            /// does not show content
            /// </summary>
            Disabled
        }

        /// <summary>
        /// Specifies what to do with already open dialogs.
        /// Used in <see cref="ContentDialogHelpers"/>
        /// </summary>
        public enum ExistingDialogOptions
        {
            /// <summary>
            /// Closes any already open dialog and returns
            /// <see cref="ContentDialogResult.None"/>
            /// </summary>
            CloseExisting,

            /// <summary>
            /// Dialog will be enqueued and shown once all
            /// previous dialogs are closed
            /// </summary>
            Enqueue
        }
    }
}
