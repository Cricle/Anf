using System.Diagnostics;

namespace Anf
{
    [DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
#if INERNAL_INFO
    internal
#else
    public
#endif
     class ComicPage : ComicRef
    {
        public string Name { get; set; }

        public override string ToString()
        {
            return $"{{{Name}, {TargetUrl}}}";
        }

        private string GetDebuggerDisplay()
        {
            return ToString();
        }
    }
}
