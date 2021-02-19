namespace Kw.Comic.Visit
{
    public class DownloadAllOptions
    {
        /// <summary>
        /// 是否缓存页指针
        /// </summary>
        public bool CachePageCursor { get; set; }
        /// <summary>
        /// 是否并行
        /// </summary>
        public bool Parallel { get; set; }
        /// <summary>
        /// 并行线程
        /// </summary>
        public int ParallelThread { get; set; }
        /// <summary>
        /// 漫画存储器
        /// </summary>
        public IComicSaver ComicSaver { get; set; }
    }
}
