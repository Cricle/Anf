using System;
using System.Linq;
using System.Net.Http;

namespace Kw.Comic.Visit
{
    public static class ComicPageExtensions
    {
        public static PageCursor MakePageCursor(this ComicPage[] pages, ChapterCursor chapterCursor)
        {
            if (pages is null)
            {
                throw new ArgumentNullException(nameof(pages));
            }

            return new PageCursor(chapterCursor,chapterCursor.SourceProvider,pages.Select(x => new ChapterVisitor(x,chapterCursor.SourceProvider)));
        }
    }
}
