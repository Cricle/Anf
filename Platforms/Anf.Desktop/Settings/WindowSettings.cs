using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anf.Desktop.Settings
{
    public class WindowSettings : ObservableObject
    {
        private double minWidth=650;
        private double minHeight=400;
        private bool topmost;

        public virtual bool Topmost
        {
            get { return topmost; }
            set => Set(ref topmost, value);
        }

        public virtual double MinHeight
        {
            get { return minHeight; }
            set => Set(ref minHeight, value);
        }

        public virtual double MinWidth
        {
            get { return minWidth; }
            set => Set(ref minWidth, value);
        }

    }
}
