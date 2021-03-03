using Kw.Comic.Engine;
using System.IO;

namespace Kw.Comic.Wpf.Managers
{
    public abstract class PhysicalComicPart
    {
        protected PhysicalComicPart(ComicEntity info)
        {
            Info = info;
        }

        public ComicEntity Info { get; }

    }
}
