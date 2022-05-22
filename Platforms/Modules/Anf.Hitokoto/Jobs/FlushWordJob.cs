using Anf.Hitokoto.Caching;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Quartz;
using SecurityLogin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anf.Hitokoto
{
    [DisallowConcurrentExecution]
    public class FlushWordJob : IJob
    {
        private readonly RandomWordResultCacheFinder finder;
        private readonly IOptions<WordFetchOptions> options;

        public FlushWordJob(RandomWordResultCacheFinder finder,
            IOptions<WordFetchOptions> options)
        {
            this.finder = finder;
            this.options = options;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var entry = await finder.FindInCahceNoIncHitAsync(options.Value.IntervalCount);
            if (entry == null || entry.HitCount > 0)
            {
                await finder.FindInDbAsync(options.Value.IntervalCount);
            }
        }
    }
}
