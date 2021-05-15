using System;
using System.Collections.Generic;
using System.Text;

namespace Anf.WebService
{
    public class ComicRankOptions
    {
        public const int DefaultSaveRankCount = 100;

        public int SaveRankCount { get; set; } = DefaultSaveRankCount;
    }
}
