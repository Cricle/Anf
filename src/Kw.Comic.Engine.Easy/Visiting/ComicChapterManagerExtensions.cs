namespace Kw.Comic.Engine.Easy.Visiting
{
    public static class ComicChapterManagerExtensions
    {
        public static PageSlots<TResource> CreatePageSlots<TResource>(this IComicChapterManager<TResource> chapterManager)
        {
            if (chapterManager is null)
            {
                throw new System.ArgumentNullException(nameof(chapterManager));
            }

            return new PageSlots<TResource>(chapterManager);
        }
        public static ChapterSlots<TResource> CreateChapterSlots<TResource>(this IComicVisiting<TResource> visiting)
        {
            if (visiting is null)
            {
                throw new System.ArgumentNullException(nameof(visiting));
            }

            return new ChapterSlots<TResource>(visiting);
        }
    }
}
