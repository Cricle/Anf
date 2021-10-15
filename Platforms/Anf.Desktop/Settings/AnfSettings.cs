using Anf.Desktop.Services;
using Anf.Platform.Settings;
using Ao.SavableConfig.Binder.Annotations;
using Avalonia.Themes.Fluent;
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Anf.Desktop.Settings
{
    [ConfigStepIn]
    public class AnfSettings
    {
        public static AnfSettings Instance => AppEngine.GetRequiredService<AnfSettings>();

        public ThemeSettings Theme { get; set; }

        public ReadingSettings Reading{ get; set; }

        public StartupSettings Startup { get; set; }

        public WindowSettings Window { get; set; }

        public PerformaceSettings Performace { get; set; }

        public virtual bool DotShowException { get; set; }
    }
}
