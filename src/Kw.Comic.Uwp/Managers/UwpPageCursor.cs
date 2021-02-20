using Kw.Comic.Visit;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Kw.Comic.Uwp.Managers
{
    public class UwpPageCursor : PageCursorBase<UwpChapterVisitor>
    {
        public UwpPageCursor(HttpClient httpclient, ImmutableArray<UwpChapterVisitor> datas) : base(httpclient, datas)
        {
        }

        public UwpPageCursor(HttpClient httpclient, IEnumerable<UwpChapterVisitor> datas) : base(httpclient, datas)
        {
        }
    }
}
