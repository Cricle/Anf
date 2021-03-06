namespace Anf
{

#if INERNAL_INFO
    internal
#else
    public
#endif 
     class ComicInfo
    {
        public string ComicUrl { get; set; }

        public string Name { get; set; }

        public string Descript { get; set; }

        public string ImageUrl { get; set; }

    }
#if INERNAL_INFO
    internal
#else
    public
#endif 
        class ComicEntity: ComicInfo
    {

        public ComicChapter[] Chapters { get; set; }
    }
#if INERNAL_INFO
    internal
#else
    public
#endif
    class ChapterWithPage
    {
        public ChapterWithPage(ComicChapter chapter, ComicPage[] pages)
        {
            Chapter = chapter;
            Pages = pages;
        }

        public ComicChapter Chapter { get; }

        public ComicPage[] Pages { get; }
    }
#if INERNAL_INFO
    internal
#else
    public
#endif
    class ComicDetail
    {
        public ComicEntity Entity { get; set; }

        public ChapterWithPage[] Chapters { get; set; }
    }
}
