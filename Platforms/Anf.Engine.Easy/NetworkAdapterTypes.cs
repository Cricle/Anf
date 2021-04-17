using System;

namespace Anf.Easy
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
