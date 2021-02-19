using System;

namespace Kw.Comic.Visit
{
    public interface ISimpleCacher<TKey,TValue>:IDisposable
    {
        void SetCache(TKey i, TValue data);

        TValue GetCache(TKey i);

        void Reset();
    }
}
