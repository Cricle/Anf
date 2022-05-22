using System;

namespace Anf.Hitokoto.Caching
{
    public class WordCacheOptions
    {
        public TimeSpan CacheTime { get; set; } = TimeSpan.FromSeconds(2);
    }
}
