using Anf.WebService;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Anf.Web.Jobs
{
    internal class SaveRankJob : ServicingJobBase
    {
        protected override Task OnExecute(IJobExecutionContext context, IServiceProvider serviceProvider)
        {
            var level = context.MergedJobDataMap.GetAs<RankLevels>(nameof(RankLevels));
            var val = serviceProvider.GetRequiredService<ComicRankSaver>();
            var task= val.SaveAsync(level);
            return task;
        }
    }
}
