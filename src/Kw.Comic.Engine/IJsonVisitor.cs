using System.Collections.Generic;

namespace Kw.Comic.Engine
{
    public interface IJsonVisitor
    {
        IJsonVisitor this[string key] { get; }

        IEnumerable<IJsonVisitor> ToArray();
    }
}
