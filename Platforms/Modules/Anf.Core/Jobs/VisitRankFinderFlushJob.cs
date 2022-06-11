using Anf.Core.Finders;
using Quartz;
using Structing.Quartz.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anf.Core.Jobs
{
    [SimpleConfigJob]
    [QuartzInterval(TimeTypes.Hour, 1)]
    [QuartzRepeatCount(forevery: true)]
    public class VisitRankFinder50FlushJob : IJob
    {
        public VisitRankFinder50FlushJob(VisitRankFinder finder)
        {
            Finder = finder;
        }

        public VisitRankFinder Finder { get; }

        public async Task Execute(IJobExecutionContext context)
        {
            var res = await Finder.FindInCahceNoIncHitAsync(50);
            if (res == null || res.HitCount > 0)
            {
                await Finder.FindInDbAsync(50);
            }
        }
    }
}
