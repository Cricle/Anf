using System.Threading.Tasks;

namespace Anf.Easy.Test
{
    internal class NullSaver : IComicSaver
    {
        public bool NeedToSave(ComicDownloadContext context)
        {
            return true;
        }

        public async Task SaveAsync(ComicDownloadContext context)
        {
        }
    }
}
