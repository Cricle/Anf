using Anf.Platform.Settings;
using Ao.SavableConfig.Binder.Annotations;
using System;
using System.Collections.Generic;
using System.Text;

namespace Anf.Cross.Settings
{
    [ConfigStepIn]
    public class AnfSettings
    {
        public ReadingSettings Reading { get; set; }

        public PerformaceSettings Performace { get; set; }

        public StartupSettings Startup { get; set; }
    }
}
