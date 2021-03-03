using System;
using System.Linq;
using System.Net.Http;

namespace Kw.Comic.Visit
{
    public static class ComicPageExtensions
    {
        public static PageCursor MakePageCursor(this ComicPage[] pages, ChapterCursor chapterCursor, HttpClient httpClient)
        {
            if (pages is null)
            {
                throw new ArgumentNullException(nameof(pages));
            }

            if (httpClient is null)
            {
                throw new ArgumentNullException(nameof(httpClient));
            }

            return new PageCursor(httpClient,chapterCursor,pages.Select(x => new ChapterVisitor(x, httpClient)));
        }
    }
}
