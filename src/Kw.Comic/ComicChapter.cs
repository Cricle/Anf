namespace Kw.Comic
{
#if INERNAL_INFO
    internal
#else
    public
#endif
    class ComicChapter : ComicRef
    {
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }
    }
}
