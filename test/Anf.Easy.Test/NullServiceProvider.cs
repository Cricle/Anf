using System;
using System.Collections.Generic;
using System.Text;

namespace Anf.Easy.Test
{
    internal class NullServiceProvider : IServiceProvider
    {
        public object GetService(Type serviceType)
        {
            return null;
        }
    }
}
