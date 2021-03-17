using System;
using System.Diagnostics;

namespace Kw.Comic.Engine.Easy.Visiting
{
    [DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
    public readonly struct ComicPos : IEquatable<ComicPos>
    {
        public static readonly ComicPos Zero = new ComicPos(0, 0);

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
            if (chapterIndex < 0 || pageIndex < 0)
            {
                throw new ArgumentException("chapterIndex or pageIndex can't min than zero!");
            }
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

        private string GetDebuggerDisplay()
        {
            return ToString();
        }
    }
}
