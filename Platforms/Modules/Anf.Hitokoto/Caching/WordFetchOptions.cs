using System;

namespace Anf.Hitokoto
{
    public class WordFetchOptions
    {
        public static readonly TimeSpan DefaultIntervalTime = TimeSpan.FromMilliseconds(500);

        public static readonly int DefaultIntervalCount = 9;

        public TimeSpan IntervalTime { get; set; } = DefaultIntervalTime;

        public int IntervalCount { get; set; } = DefaultIntervalCount;
    }
}
