using System;
using System.Collections.Generic;
using System.Text;

namespace Anf.ResourceFetcher
{
    public class FetchOptions
    {
        public static readonly TimeSpan DefaultDataTimeout = TimeSpan.FromHours(3);

        public TimeSpan DataTimeout { get; set; } = DefaultDataTimeout;

        public TimeSpan CacheTimeout { get; set; } = TimeSpan.FromMinutes(30);

        public TimeSpan ChapterMapTimeout { get; set; } = TimeSpan.FromMinutes(30);
    }
}
