﻿namespace Anf
{
    public class SearchComicResult
    {
        public bool Support { get; set; }

        public ComicSnapshot[] Snapshots { get; set; }

        public int? Total { get; set; }
    }
}
