using Anf.Avalon.Models;
using Anf.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anf.Avalon.ViewModels
{
    public class AvalonBookshelfViewModel : BookshelfViewModel<AvalonComicStoreBox>
    {
        protected override AvalonComicStoreBox CreateBox(FileInfo fileInfo)
        {
            return new AvalonComicStoreBox(fileInfo);
        }
    }
}
