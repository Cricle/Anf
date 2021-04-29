using System;
using System.Collections.Generic;
using System.Text;

namespace Anf.Platform.Services
{
    public class CloudflareCDNOptions
    {
        public string UserId { get; set; }

        public string NameSpaceId { get; set; }

        public string Email { get; set; }

        public string Key { get; set; }

        public int? TTLMs { get; set; }
    }
}
