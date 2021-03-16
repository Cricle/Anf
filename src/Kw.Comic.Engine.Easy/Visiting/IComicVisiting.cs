using System;
using System.Threading.Tasks;

namespace Kw.Comic.Engine.Easy.Visiting
{
    public interface IComicVisiting : IDisposable
    {
        IComicVisitingInterceptor VisitingInterceptor { get; set; }
        IResourceFactoryCreator ResourceFactoryCreator { get; set; }
        IResourceFactory ResourceFactory { get; }
        string Address { get; }
        IComicHost Host { get; }
        ComicEntity Entity { get; }


        Task LoadChapterAsync(int index);
        Task LoadAsync(string address);
        Task<IComicChapterManager> GetChapterManagerAsync(int index);
    }
}
