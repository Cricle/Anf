using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Kw.Comic.Visit.Interceptors
{
    public interface IPageLoadInterceptor<TChapterVisitor>
        where TChapterVisitor : ChapterVisitorBase
    {
        Task LoadAsync(PageCursorBase<TChapterVisitor> pageCursor, TChapterVisitor visitor);
    }
}
