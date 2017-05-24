using System;

namespace BatchResizer.Enum
{
    [Flags]
    public enum ResizeModes
    {
        NotSet = 0,
        Percentage = 1,
        Specified = 2
    }
}