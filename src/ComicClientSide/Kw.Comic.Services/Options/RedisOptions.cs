using System;
using System.Collections.Generic;
using System.Text;

namespace Kw.Comic.Services.Options
{
    public class RedisOptions
    {
        public bool AbortOnConnectFail { get; set; }

        public int KeepAlive { get; set; }

        public string[] Endpoints { get; set; }
    }
}
