using System;
using System.Collections.Generic;

namespace Kw.Comic.Engine
{
    public interface IJsonVisitor : IDisposable
    {
        IJsonVisitor this[string key] { get; }

        IEnumerable<IJsonVisitor> ToArray();
    }
}
