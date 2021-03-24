using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Kw.Comic.Engine.Easy.Visiting
{
    public class PageSlots<TResource> : BlockSlots<IComicVisitPage<TResource>>
    {
        public IComicChapterManager<TResource> ChapterManager { get; }

        public PageSlots(IComicChapterManager<TResource> chapterManager)
            :base(chapterManager.ChapterWithPage.Pages.Length)
        {
            ChapterManager = chapterManager ?? throw new ArgumentNullException(nameof(chapterManager));
        }

        protected override Task<IComicVisitPage<TResource>> OnLoadAsync(int index)
        {
            return ChapterManager.GetVisitPageAsync(index);
        }
    }
}
