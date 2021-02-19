using Kw.Comic.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kw.Comic.Blazor.Server.Models
{
#if INERNAL_INFO
    internal
#else
    public
#endif
    class AnalysisResult
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Descript { get; set; }

        public string Url { get; set; }

        public ChapterWithPage[] ChapterWithPages { get; set; }
    }
}
