namespace Anf.ChannelModel.Mongo
{
    public class WithPageChapterInfoOnly: ComicChapter, IUpdatableData, IRefableData
    {
        public long RefCount { get; set; }

        public long CreateTime { get; set; }

        public long UpdateTime { get; set; }
    }
    public class WithPageChapter : WithPageChapterInfoOnly
    {
        public ComicPage[] Pages { get; set; }

    }
}
