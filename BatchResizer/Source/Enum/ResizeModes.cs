using System;

namespace BatchResizer.Enum
{
    [Flags]
    public enum ResizeModes
    {
        NotSet = -1,
        Percentage = 0,
        Specified = 1
    }
}