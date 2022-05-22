using System;
using System.Collections.Generic;

namespace Anf.ChannelModel
{
    public class RandomWordResult
    {        
        public List<WordResponse> Words { get; set; }

        public int HitCount { get; set; }

        public DateTime CreateTime { get; set; }

        public TimeSpan LifeTime { get; set; }
    }
}
