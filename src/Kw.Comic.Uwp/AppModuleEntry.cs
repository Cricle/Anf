using Kw.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Kw.Comic.Uwp
{
    internal class AppModuleEntry : AutoModuleEntity
    {
        public override void Register(IRegisteContext context)
        {
            base.Register(context);
            context.Services.AddHttpClient();
        }
    }
}
