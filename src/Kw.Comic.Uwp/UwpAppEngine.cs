using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kw.Comic.Uwp
{
    internal class UwpAppEngine : AppEngine
    {
        private static readonly Lazy<UwpAppEngine> instance = new Lazy<UwpAppEngine>(Create);

        public static UwpAppEngine Instance => instance.Value;

        private static UwpAppEngine Create()
        {
            var eng = new UwpAppEngine();
            eng.Modules.Add(new AppModuleEntry());
            return eng;
        }
    }
}
