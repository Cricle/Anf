using System.Threading.Tasks;

namespace Kw.Comic.Engine.Easy.Visiting
{
    public class ChapterSlots<TResource> : BlockSlots<IComicChapterManager<TResource>>
    {
        public IComicVisiting<TResource> Visiting { get; }

        public ChapterSlots(IComicVisiting<TResource> visiting)
            : base(visiting.Entity.Chapters.Length)
        {
            Visiting = visiting;
        }

        protected override Task<IComicChapterManager<TResource>> OnLoadAsync(int index)
        {
            return Visiting.GetChapterManagerAsync(index);
        }
    }
}
