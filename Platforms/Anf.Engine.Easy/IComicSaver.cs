using System;
using System.Threading.Tasks;

namespace Anf.Easy
{
    public interface IComicSaver
    {
        bool NeedToSave(ComicDownloadContext context);

        Task SaveAsync(ComicDownloadContext context);
    }
}
