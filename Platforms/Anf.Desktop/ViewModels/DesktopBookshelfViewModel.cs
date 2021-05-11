using Anf.Platform.Models.Impl;
using Anf.ViewModels;
using Avalonia.Media.Imaging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anf.Desktop.ViewModels
{
    public class DesktopBookshelfViewModel : BookshelfViewModel<WithImageComicStoreBox<Bitmap>>
    {
        protected override WithImageComicStoreBox<Bitmap> CreateBox(FileInfo fileInfo)
        {
            return new WithImageComicStoreBox<Bitmap>(fileInfo);
        }
    }
}
