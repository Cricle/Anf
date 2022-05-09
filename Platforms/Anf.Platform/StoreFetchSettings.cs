using System;

namespace Anf.Platform
{
    public class StoreFetchSettings
    {
        public static readonly TimeSpan DefaultExpiresTime = TimeSpan.FromDays(1);

        public static readonly StoreFetchSettings NoCache = new StoreFetchSettings(true, null);

        public static readonly StoreFetchSettings DefaultCache = new StoreFetchSettings(false, DefaultExpiresTime);
       
        public static readonly StoreFetchSettings DefaultNoDisposeStream = new StoreFetchSettings(false, DefaultExpiresTime) { DisposeStream=false};

        public StoreFetchSettings(bool forceNoCache, TimeSpan? expiresTime)
        {
            ForceNoCache = forceNoCache;
            ExpiresTime = expiresTime;
        }

        public bool ForceNoCache { get; }

        public TimeSpan? ExpiresTime { get; }

        public bool DisposeStream { get; set; } = true;

        public StoreFetchSettings Clone()
        {
            return new StoreFetchSettings(ForceNoCache, ExpiresTime) { DisposeStream = DisposeStream };
        }
    }
}
