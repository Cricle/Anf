using Kw.Comic.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kw.Comic.Wpf
{
    public class ComicReadModel
    {
        public ComicReadModel(ChapterWithPage[] chapterWithPages, string httpName, string engineName, string imageEngineName)
        {
            ChapterWithPages = chapterWithPages;
            HttpName = httpName;
            EngineName = engineName;
            ImageEngineName = imageEngineName;
        }

        public ChapterWithPage[] ChapterWithPages { get; }

        public string HttpName { get; }

        public string EngineName { get; }

        public string ImageEngineName { get; }

        public string ComicName { get; set; }
    }
}
