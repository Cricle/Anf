using System;

namespace Anf.Easy.Store
{
    public interface IAddressToFileNameProvider : IDisposable
    {
        string Convert(string address);
    }
}
