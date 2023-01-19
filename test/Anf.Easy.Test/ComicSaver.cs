using System.Threading.Tasks;

namespace Anf.Easy.Test
{
    class ComicSaver : IComicSaver
    {
        public bool NeedToSave(ComicDownloadContext context)
        {
            return true;
        }

        public Task SaveAsync(ComicDownloadContext context)
        {
            return Task.CompletedTask;
        }
    }
}
