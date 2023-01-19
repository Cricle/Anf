using System.Threading.Tasks;

namespace Anf.Easy.Test
{
    internal class NullSaver : IComicSaver
    {
        public bool IsNeedToSave { get; set; } = true;
        public bool NeedToSave(ComicDownloadContext context)
        {
            return IsNeedToSave;
        }

        public async Task SaveAsync(ComicDownloadContext context)
        {
        }
    }
}
