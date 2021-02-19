using System;
using System.Diagnostics;

namespace Kw.Comic.Managers
{
    [DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
    public readonly struct PageCursorIndex : IEquatable<PageCursorIndex>
    {
        public readonly int ChapterIndex;

        public readonly int PageIndex;

        public PageCursorIndex(int chapterIndex, int pageIndex)
        {
            ChapterIndex = chapterIndex;
            PageIndex = pageIndex;
        }

        public bool Equals(PageCursorIndex other)
        {
            return other.ChapterIndex == ChapterIndex &&
                other.PageIndex == PageIndex;
        }
        public override bool Equals(object obj)
        {
            if (obj is PageCursorIndex pci)
            {
                return Equals(pci);
            }
            return false;
        }
        public override int GetHashCode()
        {
            return ((ChapterIndex >> 16) << 16) | PageIndex >> 16;
        }
        public override string ToString()
        {
            return $"{{{ChapterIndex}, {PageIndex}}}";
        }

        private string GetDebuggerDisplay()
        {
            return ToString();
        }
    }
}
