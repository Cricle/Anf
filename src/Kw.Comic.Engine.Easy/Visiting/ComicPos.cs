using System;

namespace Kw.Comic.Engine.Easy.Visiting
{
    public readonly struct ComicPos : IEquatable<ComicPos>
    {
        /// <summary>
        /// 章节索引
        /// </summary>
        public readonly int ChapterIndex;
        /// <summary>
        /// 页索引
        /// </summary>
        public readonly int PageIndex;

        public ComicPos(int chapterIndex, int pageIndex)
        {
            ChapterIndex = chapterIndex;
            PageIndex = pageIndex;
        }

        public override int GetHashCode()
        {
            return ChapterIndex << 16 | (PageIndex & 0xFFFF) >> 16;
        }
        public override string ToString()
        {
            return $"{{{ChapterIndex}, {PageIndex}}}";
        }
        public override bool Equals(object obj)
        {
            if (obj is ComicPos pos)
            {
                return Equals(pos);
            }
            return false;
        }

        public bool Equals(ComicPos other)
        {
            return ChapterIndex == other.ChapterIndex
                && PageIndex == other.PageIndex;
        }
    }
}
