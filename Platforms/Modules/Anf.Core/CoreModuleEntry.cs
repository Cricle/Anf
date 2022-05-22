using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Anf.Core.Services;
using Anf.Statistical;
using Anf.WebService;
using Microsoft.Extensions.DependencyInjection;
using Structing;
using Structing.AspNetCore;
using Structing.Core;
using Structing.Web;

namespace Anf.Core
{
    [EnableApplicationPart]
    public class CoreModuleEntry : AutoModuleEntry
    {
        public override void Register(IRegisteContext context)
        {
            base.Register(context);

            var services = context.Services;

            services.AddScoped<ComicRankService>();
            services.AddScoped<BookshelfService>();
            services.AddScoped<StatisticalService>();

            services.AddOptions<BookshelfOptions>();
            services.AddOptions<ComicRankOptions>();
            services.AddScoped<ReadingManager>();

            services.AddScoped<AppService>();
        }
    }
}
