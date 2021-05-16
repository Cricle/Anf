﻿using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anf.Platform.Settings
{
    public class PerformaceSettings : ObservableObject
    {
        private bool enableCDN;
        private bool useStore;
        private bool enableRemoteFetch;

        public virtual bool EnableRemoteFetch
        {
            get { return enableRemoteFetch; }
            set => Set(ref enableRemoteFetch, value);
        }

        public virtual bool UseStore
        {
            get { return useStore; }
            set => Set(ref useStore, value);
        }

        public virtual bool EnableCDN
        {
            get { return enableCDN; }
            set => Set(ref enableCDN, value);
        }

    }
}
