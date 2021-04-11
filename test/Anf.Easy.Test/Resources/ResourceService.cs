using Newtonsoft.Json;
using SimpleService;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Anf.Easy.Test.Resources
{
    public static class ResourceServiceHelper
    {
        public static SimpleServiceRunner Create()
        {
            return new SimpleServiceRunner(new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory));
        }
    }
}
