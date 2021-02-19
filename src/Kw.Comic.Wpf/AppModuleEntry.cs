using Kw.Core;
using Kw.Core.Commands;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kw.Comic.Wpf
{
    internal class AppModuleEntry : AutoModuleEntity
    {
        public override void Register(IRegisteContext context)
        {
            base.Register(context);
            context.Services.AddHttpClient();
            context.Services.AddSingleton<ICommandManager, CommandManager>();
        }
        public override Task ReadyAsync(IReadyContext context)
        {
            var cm = context.GetRequiredService<ICommandManager>();
            var builder=cm.Root.GetBuilder();
            builder.AddAssembly(GetType().Assembly, new ProviderInstanceFactory());
            return base.ReadyAsync(context);
        }
    }
}
