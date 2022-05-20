using System;

namespace Anf.ChannelModel.Entity
{
    [Flags]
    public enum TimeTypes
    {
        Hour = 1,
        Day = Hour << 2,
        Week = Hour << 3,
        Month = Hour << 4,
        Quarter = Hour << 5,
        Year = Hour << 6,
    }
}
