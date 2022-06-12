using Anf.Core.Finders;
using Microsoft.Extensions.Options;
using Quartz;
using Structing.Quartz.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anf.Core.Jobs
{
    [DisallowConcurrentExecution]
    [SimpleConfigJob]
    [QuartzScheduleType(QuartzScheduleTypes.Simple)]
    [QuartzInterval(TimeTypes.Hour, 1)]
    [QuartzRepeatCount(forevery: true)]
    public class VisitRankFinder50FlushJob : IJob
    {
        public VisitRankFinder50FlushJob(VisitRankFinder finder, IOptions<VisitRankFetcherOptions> options)
        {
            Finder = finder;
            Options = options;
        }

        public VisitRankFinder Finder { get; }
        
        public IOptions<VisitRankFetcherOptions> Options { get; }

        public async Task Execute(IJobExecutionContext context)
        {
            var res = await Finder.FindInCahceNoIncHitAsync(Options.Value.IntervalCount);
            if (res == null || res.HitCount > 0)
            {
                await Finder.FindInDbAsync(Options.Value.IntervalCount);
            }
        }
    }
}
