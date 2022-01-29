using Anf.Platform.Models.Impl;
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
    public class UnoBookshelfViewModel : BookshelfViewModel<WithImageComicStoreBox<ImageSource, ImageSource>>
    {
        protected override WithImageComicStoreBox<ImageSource, ImageSource> CreateBox(FileInfo fileInfo)
        {
            return new WithImageComicStoreBox<ImageSource, ImageSource>(fileInfo);
        }
    }
}
