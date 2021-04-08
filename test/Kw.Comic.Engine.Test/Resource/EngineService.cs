using SimpleService;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Kw.Comic.Engine.Test.Resource
{
    internal class EngineService : SimpleServiceRunner
    {
        public EngineService() 
            : base(new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory))
        {
        }
    }
}
