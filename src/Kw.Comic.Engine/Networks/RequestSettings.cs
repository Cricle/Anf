using System.Collections.Generic;
using System.IO;

namespace Kw.Comic.Engine.Networks
{
    public class RequestSettings
    {
        public string Address { get; set; }

        public string Host { get; set; }

        public string Method { get; set; }

        public string ContentType { get; set; }

        public string Referrer { get; set; }

        public int Timeout { get; set; }

        public Stream Data { get; set; }

        public IReadOnlyDictionary<string,string> Headers { get; set; }
    }
}
