using System;

namespace Kw.Comic.Engine.Easy.Store
{
    public interface IAddressToFileNameProvider : IDisposable
    {
        string Convert(string address);
    }
}
