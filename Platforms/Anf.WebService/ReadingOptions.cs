using System;

namespace Anf.WebService
{
    public class ReadingOptions
    {
        public TimeSpan ReadingTimeout { get; set; } = TimeSpan.FromMinutes(5);
    }
}
