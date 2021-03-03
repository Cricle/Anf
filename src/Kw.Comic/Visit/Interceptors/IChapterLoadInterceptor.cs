using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Kw.Comic.Visit.Interceptors
{
    public interface IChapterLoadInterceptor
    {
        Task LoadAsync(ChapterCursor chapterCursor, ComicVisitor visitor);
    }
}
