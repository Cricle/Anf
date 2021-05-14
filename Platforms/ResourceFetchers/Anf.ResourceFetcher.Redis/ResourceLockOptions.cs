using Anf.ChannelModel.KeyGenerator;
using System;
using System.Collections.Generic;
using System.Text;

namespace Anf.ResourceFetcher.Redis
{
    public class ResourceLockOptions
    {
        public TimeSpan ResourceLockTimeout { get; set; } = RedisKeyGenerator.RedKeyOutTime;
    }
}
