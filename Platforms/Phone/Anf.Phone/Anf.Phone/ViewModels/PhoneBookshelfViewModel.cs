using Anf.Phone.Models;
using Anf.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Anf.Phone.ViewModels
{
    public class PhoneBookshelfViewModel : BookshelfViewModel<PhoneComicStoreBox>
    {
        protected override PhoneComicStoreBox CreateBox(FileInfo fileInfo)
        {
            return new PhoneComicStoreBox(fileInfo);
        }
    }
}
