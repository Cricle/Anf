using System;
using System.Threading.Tasks;

namespace Kw.Comic.Engine.Easy.Visiting
{
    public interface IComicVisiting<TResource> : IDisposable
    {
        IComicVisitingInterceptor<TResource> VisitingInterceptor { get; set; }
        IResourceFactoryCreator<TResource> ResourceFactoryCreator { get; set; }
        IResourceFactory<TResource> ResourceFactory { get; }
        IComicSourceProvider SourceProvider { get; }
        string Address { get; }
        IServiceProvider Host { get; }
        ComicEntity Entity { get; }


        Task LoadChapterAsync(int index);
        Task<bool> LoadAsync(string address);
        Task<IComicChapterManager<TResource>> GetChapterManagerAsync(int index);
    }
}
