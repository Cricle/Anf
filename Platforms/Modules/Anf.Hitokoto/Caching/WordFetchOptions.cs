using System;

namespace Anf.Hitokoto
{
    public class WordFetchOptions
    {
        public static readonly TimeSpan DefaultIntervalTime = TimeSpan.FromMilliseconds(600);

        public static readonly int DefaultIntervalCount = 1;

        public TimeSpan IntervalTime { get; set; } = DefaultIntervalTime;

        public int IntervalCount { get; set; } = DefaultIntervalCount;
    }
}
