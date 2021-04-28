using Anf.Desktop.Services;
using Ao.SavableConfig.Binder.Annotations;
using Avalonia.Themes.Fluent;
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anf.Desktop.Settings
{
    [ConfigStepIn]
    public class AnfSettings
    {
        public ThemeSettings Theme { get; set; }

        public ReadingSettings Reading{ get; set; }

        public StartupSettings Startup { get; set; }

        public WindowSettings Window { get; set; }
    }
}
