using System;
using System.Collections.Generic;

namespace Anf.Easy.Test
{
    internal class ValueServiceProvider : IServiceProvider
    {
        public Dictionary<Type,Func<object>> ServiceMap { get; set; }

        public object GetService(Type serviceType)
        {
            return ServiceMap[serviceType]();
        }
    }
}
