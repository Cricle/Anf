using System;
using System.Threading.Tasks;

namespace Anf.Easy.Visiting
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

        event Action<ComicVisiting<TResource>, string> Loading;
        event Action<ComicVisiting<TResource>, ComicEntity> Loaded;
        event Action<ComicVisiting<TResource>, int> LoadingChapter;
        event Action<ComicVisiting<TResource>, ChapterWithPage> LoadedChapter;

        void EraseChapter(int index);
        Task LoadChapterAsync(int index);
        Task<bool> LoadAsync(string address);
        Task<IComicChapterManager<TResource>> GetChapterManagerAsync(int index);
    }
}
