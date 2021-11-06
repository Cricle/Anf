using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anf.Platform.Settings
{
    public class StartupSettings : ObservableObject
    {
        private StartupTypes startupType;
        private int displayProposalCount;

        public virtual int DisplayProposalCount
        {
            get { return displayProposalCount; }
            set => SetProperty(ref displayProposalCount, value);
        }

        public virtual StartupTypes StartupType
        {
            get { return startupType; }
            set => SetProperty(ref startupType, value);
        }
    }
}
