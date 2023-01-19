using System;
using System.Diagnostics;

namespace Anf
{
    [DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
#if INERNAL_INFO
    internal
#else
    public
#endif
     abstract class ComicRef : IEquatable<ComicRef>
    {
        /// <summary>
        /// 目标地址
        /// </summary>
        public string TargetUrl { get; set; }

        public override bool Equals(object obj)
        {
            return Equals(obj as ComicRef);
        }

        public bool Equals(ComicRef other)
        {
            if (other is null)
            {
                return false;
            }
            return other.TargetUrl == TargetUrl;
        }

        public override int GetHashCode()
        {
            return TargetUrl?.GetHashCode() ?? 0;
        }
        public override string ToString()
        {
            return TargetUrl;
        }

        private string GetDebuggerDisplay()
        {
            return ToString();
        }
    }
}
