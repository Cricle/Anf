using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anf.Core
{
    public class VisitRankFetcherOptions
    {
        public static readonly TimeSpan DefaultIntervalTime = TimeSpan.FromMilliseconds(500);

        public static readonly int DefaultIntervalCount = 50;

        public TimeSpan IntervalTime { get; set; } = DefaultIntervalTime;

        public int IntervalCount { get; set; } = DefaultIntervalCount;
    }
}
