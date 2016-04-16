using System;

namespace PowerArhitecture.Common.Enums
{
    [Flags]
    public enum PatternOptions
    {
        None = 0,
        StartsWith = 2,
        EndsWith = 4,
        Contains = 8
    }
}