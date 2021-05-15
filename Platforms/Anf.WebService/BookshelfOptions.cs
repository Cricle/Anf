using System;

namespace Anf.WebService
{
    public class BookshelfOptions
    {
        public static readonly TimeSpan DefaultCacheTimeout = TimeSpan.FromMinutes(5);

        public TimeSpan BookshelfTimeout { get; set; } = DefaultCacheTimeout;

        public TimeSpan BookshelfItemTimeout { get; set; } = DefaultCacheTimeout;

        public TimeSpan BookshelfCreateCacheTimeout { get; set; } = DefaultCacheTimeout;
    }
}
