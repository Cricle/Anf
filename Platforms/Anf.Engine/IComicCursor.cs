using System;
using System.Threading.Tasks;

namespace Anf
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
