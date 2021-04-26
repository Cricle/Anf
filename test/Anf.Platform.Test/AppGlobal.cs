using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace Anf.Platform.Test
{
    public static class AppGlobal
    {
        [TestInitialize]
        public static void Init()
        {
            AppEngine.AddServices();
            AppEngine.Services.AddDefaultEasyComic();
        }
    }
}
