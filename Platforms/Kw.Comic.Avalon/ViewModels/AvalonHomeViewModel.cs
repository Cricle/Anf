using Kw.Comic.Engine;
using Kw.Comic.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kw.Comic.Avalon.ViewModels
{
    public class AvalonHomeViewModel : HomeViewModel
    {
        public AvalonHomeViewModel()
        {
        }

        public AvalonHomeViewModel(SearchEngine searchEngine) : base(searchEngine)
        {
        }
    }
}
