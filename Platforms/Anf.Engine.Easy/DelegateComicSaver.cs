using System;
using System.Threading.Tasks;

namespace Anf.Easy
{
    public class DelegateComicSaver : IComicSaver
    {
        private readonly Func<ComicDownloadContext, bool> needToSave;
        private readonly Func<ComicDownloadContext, Task> save;

        public DelegateComicSaver(Func<ComicDownloadContext, Task> save)
            :this(_=>true,save)
        {
        }

        public DelegateComicSaver(Func<ComicDownloadContext, bool> needToSave, Func<ComicDownloadContext, Task> save)
        {
            this.needToSave = needToSave ?? throw new ArgumentNullException(nameof(needToSave));
            this.save = save ?? throw new ArgumentNullException(nameof(save));
        }

        public bool NeedToSave(ComicDownloadContext context)
        {
            return needToSave(context);
        }

        public Task SaveAsync(ComicDownloadContext context)
        {
            return save(context);
        }
    }
}
