using SimpleService;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Anf.Platform.Test
{
    internal class PlatformHost : SimpleServiceRunner
    {
        public PlatformHost() 
            : base(new DirectoryInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"Resources")))
        {
        }
    }
}
