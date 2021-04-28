using Anf.Desktop.Models;
using Anf.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anf.Desktop.ViewModels
{
    public class DesktopBookshelfViewModel : BookshelfViewModel<AvalonComicStoreBox>
    {
        protected override AvalonComicStoreBox CreateBox(FileInfo fileInfo)
        {
            return new AvalonComicStoreBox(fileInfo);
        }
    }
}
