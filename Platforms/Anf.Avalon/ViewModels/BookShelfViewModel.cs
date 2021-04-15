using Anf.Services;
using Anf.ViewModels;
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anf.Avalon.ViewModels
{
    public class BookShelfViewModel : BookshelfViewModel
    {
        public BookShelfViewModel()
            : base(AppEngine.GetRequiredService<IBookshelfService>())
        {
        }
    }
}
