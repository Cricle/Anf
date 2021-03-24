using Kw.Comic.Engine.Easy.Visiting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media;

namespace Kw.Comic.Uwp.Managers
{
    internal class UwpComicVisiting : ComicVisiting<ImageSource>
    {
        public UwpComicVisiting(IServiceProvider host, IResourceFactoryCreator<ImageSource> resourceFactoryCreator) : base(host, resourceFactoryCreator)
        {
        }
    }
}
