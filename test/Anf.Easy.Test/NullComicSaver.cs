using System.Threading.Tasks;

namespace Anf.Easy.Test
{
    internal class NullComicSaver : IComicSaver
    {
        public bool NeedToSave(ComicDownloadContext context)
        {
            return true;
        }

        public Task SaveAsync(ComicDownloadContext context)
        {
            return Task.FromResult(1);
        }
    }
}
