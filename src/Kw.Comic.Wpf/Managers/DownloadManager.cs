using Kw.Comic.Visit;
using Kw.Comic.Visit.Interceptors;
using Kw.Core.Annotations;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Kw.Comic.Wpf.Managers
{
    [EnableService(ServiceLifetime = ServiceLifetime.Singleton)]
    public class DownloadManager : ComicPhysicalManager, IPageLoadInterceptor<SoftwareChapterVisitor>
    {
        public const string DownloadFolderName = "Downloads";
        public static string DownloadFolderPath => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, DownloadFolderName);

        private readonly SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);

        public DownloadManager()
            : base(DownloadFolderPath)
        {

        }

        public async Task LoadAsync(PageCursorBase<SoftwareChapterVisitor> pageCursor, SoftwareChapterVisitor visitor)
        {
            var comicName = pageCursor.ChapterCursor.Comic.Name;
            var comic = EnumerableComic().FirstOrDefault(x => x.Info.Name == comicName);
            PhysicalPage phyPage = null;
            if (comic != null)
            {
                var chapterName = pageCursor.ChapterCursor.Current.Chapter.Title;
                var chapter = comic.GetChapters().FirstOrDefault(x => x.Chapter.Title == chapterName);
                if (chapter != null)
                {
                    phyPage = chapter.GetPage(visitor.Page);
                    if (phyPage != null && phyPage.File.Exists)
                    {
                        await visitor.LoadFromFileAsync(phyPage.File.FullName);
                        return;
                    }
                }
            }
            if (phyPage == null)
            {
                var phyComic = AddComic(pageCursor.ChapterCursor.Comic);
                var chr = phyComic.GetChapter(pageCursor.ChapterCursor.Current.Chapter);
                chr.EnsureCreated();
                phyPage = chr.GetPage(visitor.Page);
            }
            await visitor.LoadAsync();
            var stream = visitor.GetStream();
            if (stream != null && stream.CanSeek)
            {
                Cache(phyPage, stream);
            }
        }
        private async void Cache(PhysicalPage page, Stream stream)
        {
            await semaphoreSlim.WaitAsync();
            try
            {
                stream.Seek(0, SeekOrigin.Begin);
                await page.UpdateAsync(stream);
                stream.Seek(0, SeekOrigin.Begin);
            }
            finally
            {
                semaphoreSlim.Release();
            }
        }
    }
}
