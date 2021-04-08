using System.Threading.Tasks;

namespace Anf.Easy.Visiting
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
