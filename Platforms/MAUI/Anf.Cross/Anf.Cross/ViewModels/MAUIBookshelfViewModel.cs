using Anf.Platform.Models.Impl;
using Anf.ViewModels;
using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anf.Cross.ViewModels
{
    public class MAUIBookshelfViewModel : BookshelfViewModel<WithImageComicStoreBox<Stream,ImageSource>>
    {
        protected override WithImageComicStoreBox<Stream, ImageSource> CreateBox(FileInfo fileInfo)
        {
            return new WithImageComicStoreBox<Stream, ImageSource>(fileInfo);
        }
    }
}
