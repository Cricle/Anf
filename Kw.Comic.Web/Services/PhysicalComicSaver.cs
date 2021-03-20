using Kw.Comic.Engine.Easy;
using System.Threading.Tasks;

namespace KwC.Services
{
    public class PhysicalComicSaver : IComicSaver
    {
        private readonly IStoreService storeService;

        public PhysicalComicSaver(IStoreService storeService)
        {
            this.storeService = storeService;
        }

        public bool NeedToSave(ComicDownloadContext context)
        {
            var exists = storeService.ExistsAsync(context.Page.TargetUrl).GetAwaiter().GetResult();
            return !exists;
        }

        public Task SaveAsync(ComicDownloadContext context)
        {
            return storeService.SaveAsync(context.Page.TargetUrl, context.SourceStream);
        }
    }
}
