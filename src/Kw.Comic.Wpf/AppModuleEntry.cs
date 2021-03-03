using Kw.Comic.Wpf.Managers;
using Kw.Core;
using Kw.Core.Commands;
using Kw.Visio.Selectors;
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
            context.Services.AddSingleton<ICommandManager, CommandManager>();
            context.Services.AddSingleton(new MainNavigationService());
#if EnableRecyclableStream
            context.Services.AddSingleton(SoftwareChapterVisitor.recyclableMemoryStreamManager);
#endif
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
