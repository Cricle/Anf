using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anf.Platform.Settings
{
    public class ReadingSettings : ObservableObject
    {
        private bool loadAll;

        public virtual bool LoadAll
        {
            get { return loadAll; }
            set { Set(ref loadAll, value); }
        }
    }
}
