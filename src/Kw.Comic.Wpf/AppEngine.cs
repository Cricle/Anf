using Kw.Comic.Wpf.Managers;
using Kw.Core;
using Kw.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kw.Comic.Wpf
{
    internal class WpfAppEngine : AppEngine
    {
        private static Lazy<WpfAppEngine> instance = new Lazy<WpfAppEngine>(Create);

        public WpfAppEngine()
            : base(new ServiceCollection())
        {
        }

        public static WpfAppEngine Instance
        {
            get
            {
                return instance.Value;
            }
        }
        private static WpfAppEngine Create()
        {
            var eng = new WpfAppEngine();
            eng.Modules.Add(new AppModuleEntry());
            return eng;
        }
        public static MainNavigationService GetNavigationService()
        {
            return Instance.GetRequiredService<MainNavigationService>();
        }

        protected override IServiceProvider BuildProvider()
        {
            return Services.BuildServiceProvider(true);
        }
    }
}
