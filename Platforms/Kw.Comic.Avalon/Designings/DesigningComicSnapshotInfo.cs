using Avalonia.Controls;
using Kw.Comic.Engine;
using Kw.Comic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kw.Comic.Avalon.Designings
{
#if DEBUG

    public class DesigningComicSnapshotInfo : ComicSnapshotInfo
    {
        private static readonly ComicSnapshot desisnValue = new ComicSnapshot
        {
            Author = "Author",
            Descript = "DescriptDescriptDescriptDescriptDescriptDescriptDescriptDescript",
            ImageUri = "http://avaloniaui.net/images/logo.svg",
            TargetUrl = "http://avaloniaui.net/images/logo.svg",
            Name = "Name",
            Sources = new ComicSource[]
                  {
                      new ComicSource{ Name="Source1", TargetUrl="http://avaloniaui.net/images/logo.svg"},
                      new ComicSource{ Name="Source2", TargetUrl="http://avaloniaui.net/images/logo.svg"},

                  }
        };
        public DesigningComicSnapshotInfo() 
            : base(desisnValue)
        {
        }
    }
#endif
}
