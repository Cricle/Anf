using System;
using System.Threading.Tasks;

namespace Kw.Comic.Engine
{
    public interface IComicCursor : IDisposable
    {
        string Keyword { get; }

        int Index { get; }

        int Take { get; }

        SearchComicResult Current { get; }

        Task<bool> MoveNextAsync();
    }
}
