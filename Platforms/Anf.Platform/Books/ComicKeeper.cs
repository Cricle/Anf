using Anf.Easy.Store;
using Anf.Easy.Visiting;
using Anf.Engine;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Anf.Platform
{
    public class ComicKeeper : IDisposable
    {
        public static readonly string ComicIndexName = "[ComicIndex]";

        private readonly IStoreService storeService;
        private readonly IStoreService fetchService;
        private readonly IServiceScope scope;
        private readonly CancellationTokenSource tokenSource;

        public event Action<ComicEntity, ComicChapter, ComicPage, Exception, KeepingActionTypes> KeepingAction;

        public ComicKeeper(string address, string imageFolderName, IStoreService storeService = null, IStoreService fetchService=null)
        {
            Address = address;
            ImageFolderName = imageFolderName;
            HasImageFolderName = !string.IsNullOrEmpty(imageFolderName);
            scope = AppEngine.CreateScope();
            var engine = scope.ServiceProvider.GetRequiredService<ComicEngine>();
            var condition = engine.GetComicSourceProviderType(address);
            if (condition != null)
            {
                Provider = (IComicSourceProvider)scope.ServiceProvider.GetRequiredService(condition.ProviderType);
            }
            else
            {
                NotSupport = true;
            }
            this.storeService = storeService ?? scope.ServiceProvider.GetRequiredService<IStoreService>();
            this.fetchService = fetchService ?? this.storeService;
        }

        public bool NotSupport { get; }

        public string ImageFolderName { get; }

        public bool HasImageFolderName { get; }

        public IComicSourceProvider Provider { get; }

        public ComicStoreWriteModes WriteMode { get; }

        public string Address { get; }

        public async Task<ComicDetail> StoreAsync()
        {
            KeepingAction?.Invoke(null, null, null, null, KeepingActionTypes.BeginGetEntity);
            OnBeginGetEntity();
            var entity = await Provider.GetChaptersAsync(Address);
            try
            {
                if (entity is null)
                {
                    return null;
                }
                var detail = new ComicDetail { Entity = entity };
                var cwps = new List<ChapterWithPage>(entity.Chapters.Length);
                foreach (var item in entity.Chapters)
                {
                    if (tokenSource.IsCancellationRequested)
                    {
                        break;
                    }
                    KeepingAction?.Invoke(entity, item, null, null, KeepingActionTypes.BeginStoreChapter);
                    OnBeginStoreChapter(entity, item);
                    try
                    {
                        var cwp = await StoreChapterAsync(entity,item);
                        cwps.Add(cwp);
                    }
                    catch (Exception ex)
                    {
                        KeepingAction?.Invoke(entity, item, null, ex, KeepingActionTypes.StoreChapterException);
                        OnStoreChapterException(entity, item, ex);
                    }
                    KeepingAction?.Invoke(entity, item, null, null, KeepingActionTypes.EndStoreChapter);
                    OnEndStoreChapter(entity, item);
                }
                if (!tokenSource.IsCancellationRequested)
                {
                    var streamManager = AppEngine.GetRequiredService<RecyclableMemoryStreamManager>();
                    var str = JsonHelper.Serialize(detail);
                    var buffer = Encoding.UTF8.GetBytes(str);
                    var path = ComicIndexName;
                    using (var stream = streamManager.GetStream())
                    {
                        stream.Write(buffer, 0, buffer.Length);
                        stream.Seek(0, SeekOrigin.Begin);
                        await storeService.SaveAsync(path, stream);
                    }
                }
                return detail;
            }
            finally
            {
                KeepingAction?.Invoke(entity, null, null, null, KeepingActionTypes.EndGetEntity);
                OnEndGetEntity(entity);
            }
        }
        public void Cancel()
        {
            tokenSource.Cancel();
        }
        protected virtual async Task<ChapterWithPage> StoreChapterAsync(ComicEntity entity,ComicChapter chapter)
        {
            var pages = await Provider.GetPagesAsync(chapter.TargetUrl);
            if (pages != null)
            {
                foreach (var item in pages)
                {
                    if (tokenSource.IsCancellationRequested)
                    {
                        break;
                    }
                    await StorePageAsync(entity,chapter, item);
                }
            }
            return new ChapterWithPage(chapter, pages);
        }
        protected virtual async Task StorePageAsync(ComicEntity entity,ComicChapter chapter, ComicPage page)
        {
            KeepingAction?.Invoke(entity, chapter, page, null, KeepingActionTypes.BeginStorePage);
            OnBeginStorePage(chapter, page);
            try
            {
                var mode = WriteMode;
                if (mode == ComicStoreWriteModes.NotExists)
                {
                    var ok = await storeService.ExistsAsync(page.TargetUrl);
                    if (ok)
                    {
                        KeepingAction?.Invoke(entity, chapter, page, null, KeepingActionTypes.EndStorePage);
                        OnEndStorePage(chapter, page, false);
                        return;
                    }
                }
                var path = page.TargetUrl;
                if (HasImageFolderName)
                {
                    path = string.Concat(ImageFolderName, "/", page.TargetUrl);
                }
                var stream = await fetchService.GetStreamAsync(page.TargetUrl);
                if (stream is null)
                {
                    stream = await Provider.GetImageStreamAsync(page.TargetUrl);
                    if (fetchService != storeService)
                    {
                        await fetchService.SaveAsync(path, stream);
                    }
                }
                stream.Seek(0, SeekOrigin.Begin);
                await storeService.SaveAsync(path, stream);
            }
            catch (Exception ex)
            {
                KeepingAction?.Invoke(entity, chapter, page, ex, KeepingActionTypes.StorePageException);
                OnStorePageException(chapter, page, ex);
            }
            KeepingAction?.Invoke(entity, chapter, page, null, KeepingActionTypes.EndStorePage);
            OnEndStorePage(chapter, page, true);
        }

        protected virtual void OnStoreChapterException(ComicEntity entity, ComicChapter chapter, Exception ex)
        {

        }
        protected virtual void OnBeginStoreChapter(ComicEntity entity, ComicChapter chapter)
        {

        }
        protected virtual void OnEndStoreChapter(ComicEntity entity, ComicChapter chapter)
        {

        }
        protected virtual void OnBeginGetEntity()
        {

        }
        protected virtual void OnEndGetEntity(ComicEntity entity)
        {

        }
        protected virtual void OnStorePageException(ComicChapter chapter, ComicPage page, Exception ex)
        {

        }
        protected virtual void OnBeginStorePage(ComicChapter chapter, ComicPage page)
        {

        }
        protected virtual void OnEndStorePage(ComicChapter chapter, ComicPage page, bool write)
        {

        }

        public void Dispose()
        {
            scope.Dispose();
        }
        public static ComicKeeper FromDefault(string address, IStoreService storeService = null, IStoreService fetchService = null)
        {
            return new ComicKeeper(address, string.Empty, storeService,fetchService);
        }
    }
}
