using System;

namespace Kw.Comic.Engine.Easy
{
    [Flags]
    public enum NetworkAdapterTypes
    {
#if !NETSTANDARD1_4
        WebRequest,
#endif
        HttpClient
    }
}
