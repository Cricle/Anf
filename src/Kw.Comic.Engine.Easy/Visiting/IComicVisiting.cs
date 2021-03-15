using System;
using System.Threading.Tasks;

namespace Kw.Comic.Engine.Easy.Visiting
{
    public interface IComicVisiting : IDisposable
    {
        int? SharedCapacity { get; set; }

        string Address { get; }
        ComicEntity Entity { get; }
        Task LoadChapterAsync(int index);
        Task LoadAsync(string address);
        Task<IComicChapterManager> GetChapterManagerAsync(int index);
    }
}
