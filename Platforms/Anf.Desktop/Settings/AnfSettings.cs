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
    public class AnfSettings
    {
        [ConfigStepIn]
        public ThemeSettings Theme { get; set; }

        [ConfigStepIn]
        public ReadingSettings Reading{ get; set; }
    }
}
