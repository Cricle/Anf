using Anf.Platform.Settings;
using Ao.SavableConfig.Binder.Annotations;

namespace Anf.Settings
{
    [ConfigStepIn]
    public class AnfSettings
    {
        public ReadingSettings Reading{ get; set; }

        public StartupSettings Startup { get; set; }

        public WindowSettings Window { get; set; }

        public PerformaceSettings Performace { get; set; }

        public virtual bool DotShowException { get; set; }
    }
}
