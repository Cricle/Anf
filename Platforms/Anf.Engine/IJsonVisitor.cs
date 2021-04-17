using System;
using System.Collections.Generic;

namespace Anf
{
    public interface IJsonVisitor : IDisposable
    {
        IJsonVisitor this[string key] { get; }

        IEnumerable<IJsonVisitor> ToArray();
    }
}
