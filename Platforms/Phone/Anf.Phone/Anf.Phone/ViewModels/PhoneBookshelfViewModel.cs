using Anf.Phone.Models;
using Anf.Platform.Models.Impl;
using Anf.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xamarin.Forms;

namespace Anf.Phone.ViewModels
{
    public class PhoneBookshelfViewModel : BookshelfViewModel<WithImageComicStoreBox<ImageSource>>
    {
        protected override WithImageComicStoreBox<ImageSource> CreateBox(FileInfo fileInfo)
        {
            return new WithImageComicStoreBox<ImageSource>(fileInfo);
        }
    }
}
