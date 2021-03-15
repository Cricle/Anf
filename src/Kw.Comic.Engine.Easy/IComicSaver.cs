using System;
using System.Threading.Tasks;

namespace Kw.Comic.Engine.Easy
{
    public interface IComicSaver
    {
        bool NeedToSave(ComicDownloadContext context);

        Task SaveAsync(ComicDownloadContext context);
    }
}
