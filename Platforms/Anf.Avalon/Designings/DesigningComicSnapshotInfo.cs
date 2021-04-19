using Avalonia.Controls;
using Anf;
using Anf.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anf.Avalon.Designings
{
#if DEBUG

    public class DesigningComicSnapshotInfo : ComicSourceInfo
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
            : base(desisnValue,desisnValue.Sources[0],null)
        {
        }
    }
#endif
}
