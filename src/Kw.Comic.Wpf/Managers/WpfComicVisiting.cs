using Kw.Comic.Engine;
using Kw.Comic.Engine.Easy.Visiting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace Kw.Comic.Wpf.Managers
{
    internal class WpfComicVisiting : ComicVisiting<ImageSource>
    {
        public WpfComicVisiting(IServiceProvider host, IResourceFactoryCreator<ImageSource> resourceFactoryCreator) : base(host, resourceFactoryCreator)
        {
        }
    }
}
