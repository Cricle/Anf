using System.Collections.Generic;
using System.IO;

namespace Anf.Networks
{
    public class RequestSettings
    {
        public string Address { get; set; }

        public string Host { get; set; }

        public string Method { get; set; }

        public string Accept { get; set; }

        public string Referrer { get; set; }

        public int Timeout { get; set; }

        public Stream Data { get; set; }

        public IReadOnlyDictionary<string,string> Headers { get; set; }
    }
}
