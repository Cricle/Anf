using System.Collections.Generic;

namespace Anf.Easy
{
    public class DownloadItemRequest
    {
        public DownloadItemRequest(ComicPage page)
        {
            Page = page;
        }

        public DownloadItemRequest(ComicChapter chapter, ComicPage page)
        {
            Chapter = chapter;
            Page = page;
        }

        /// <summary>
        /// 章节
        /// </summary>
        public ComicChapter Chapter { get; }
        /// <summary>
        /// 目标页
        /// </summary>
        public ComicPage Page { get; }
    }
    public class ComicDownloadRequest
    {
        public ComicDownloadRequest(IComicSaver saver,
            ComicEntity entity,
            ComicDetail detail,
            IReadOnlyCollection<DownloadItemRequest> requests, 
            IComicSourceProvider provider)
        {
            Detail = detail;
            Entity = entity;
            Saver = saver;
            DownloadRequests=requests;
            Provider = provider;
        }

        public IComicSaver Saver { get; }

        public ComicEntity Entity { get; }

        public ComicDetail Detail { get; }

        public IReadOnlyCollection<DownloadItemRequest> DownloadRequests { get; }

        public IDownloadListener Listener { get; set; }

        //TODO:回调

        public IComicSourceProvider Provider { get; }
    }
}
