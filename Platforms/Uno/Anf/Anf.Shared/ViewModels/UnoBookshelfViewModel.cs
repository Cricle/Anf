using Anf.Platform.Models.Impl;
using Anf.Services;
using Anf.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;

namespace Anf.ViewModels
{
    public class UnoBookshelfViewModel : BookshelfViewModel<WithImageComicStoreBox<ImageBox, ImageBox>>
    {
        protected override WithImageComicStoreBox<ImageBox, ImageBox> CreateBox(FileInfo fileInfo)
        {
            return new WithImageComicStoreBox<ImageBox, ImageBox>(fileInfo);
        }
    }
}
