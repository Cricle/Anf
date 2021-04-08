using System;
using System.Collections.Generic;
namespace Anf.Test
{
    internal class ValueServiceProvider : IServiceProvider
    {

        public Dictionary<Type, Func<object>> Factory { get; set; }
        public object GetService(Type serviceType)
        {
            return Factory[serviceType]();
        }
    }
}
