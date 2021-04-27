using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anf.Desktop.Settings
{
    public class StartupSettings : ObservableObject
    {
        private StartupTypes startupType;
        private int displayProposalCount;

        public virtual int DisplayProposalCount
        {
            get { return displayProposalCount; }
            set => Set(ref displayProposalCount, value);
        }

        public virtual StartupTypes StartupType
        {
            get { return startupType; }
            set => Set(ref startupType, value);
        }
    }
}
