using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Anf.Hitokoto
{
    public static class TimeHelper
    {
        private static readonly DateTime zeroTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);

        public static long GetTimeStamp(in DateTime time)
        {
            return (long)(time - zeroTime).TotalMilliseconds;
        }
        public static long GetTimeStamp()
        {
            return (long)(DateTime.UtcNow - zeroTime).TotalMilliseconds;
        }
        public static DateTime GetCsTime(long timestamp)
        {
            return zeroTime.AddMilliseconds(timestamp).ToLocalTime();
        }
    }
}
