namespace BatchResizer
{
    public static class Settings
    {
        // Constant definition rules for resizing.

        /// <summary>
        /// Max image height when using <see cref="BatchResizer.Enum.ResizeModes.Specified"/> mode.
        /// </summary>
        public const int MAX_IMAGE_HEIGHT = 10000;

        /// <summary>
        /// Max image width when using <see cref="BatchResizer.Enum.ResizeModes.Specified"/> mode.
        /// </summary>
        public const int MAX_IMAGE_WIDTH = 10000;

        /// <summary>
        /// Max resize percentage when using <see cref="BatchResizer.Enum.ResizeModes.Percentage"/> mode.
        /// </summary>
        public const int MAX_RESIZE_PERCENTAGE = 5000;

        /// <summary>
        /// Min image height when using <see cref="BatchResizer.Enum.ResizeModes.Specified"/> mode.
        /// </summary>
        public const int MIN_IMAGE_HEIGHT = 1;

        /// <summary>
        /// Min image width when using <see cref="BatchResizer.Enum.ResizeModes.Specified"/> mode.
        /// </summary>
        public const int MIN_IMAGE_WIDTH = 1;

        /// <summary>
        /// Min resize percentage when using <see cref="BatchResizer.Enum.ResizeModes.Percentage"/> mode.
        /// </summary>
        public const int MIN_RESIZE_PERCENTAGE = 1;
    }
}