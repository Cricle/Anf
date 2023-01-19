using System;
using System.Collections.Generic;

namespace Anf
{
    public interface IJsonVisitor : IDisposable, IEnumerable<KeyValuePair<string, IJsonVisitor>>
    {
        IJsonVisitor this[string key] { get; }

        bool HasValue { get; }

        bool IsArray { get; }

        IEnumerable<IJsonVisitor> ToEnumerable();
    }
}
