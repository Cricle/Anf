using System;
using System.Collections.Generic;
using System.Text;

namespace Anf.WebService
{
    public class ReadingOptions
    {
        public TimeSpan ReadingTimeout { get; set; } = TimeSpan.FromMinutes(10);
    }
}
