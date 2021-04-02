using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kw.Comic.Avalon.Services
{
    internal class TitleService
    {
        public TitleService(MainWindow window)
        {
            Window = window;
        }

        public string Title
        {
            get => Window.Title;
            set => Window.Title = value;
        }
        
        public MainWindow Window { get; }


    }
}
