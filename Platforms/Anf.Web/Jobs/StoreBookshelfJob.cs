using Anf.Statistical;
using Anf.WebService;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Anf.Web.Jobs
{
    internal class StoreLazyInsertJob : ServicingJobBase
    {
        public StoreLazyInsertJob(IServiceProvider provider) 
            : base(provider)
        {
        }

        protected override async Task OnExecute(IJobExecutionContext context, IServiceProvider serviceProvider)
        {
            var ser = serviceProvider.GetRequiredService<StatisticalService>();
            await ser.StoreSearchsAsync(50);
            await ser.StoreVisitsAsync(50);
        }
    }
    internal class StoreBookshelfJob : ServicingJobBase
    {
        public StoreBookshelfJob(IServiceProvider provider) : base(provider)
        {
        }

        protected override Task OnExecute(IJobExecutionContext context, IServiceProvider serviceProvider)
        {
            var ser = serviceProvider.GetRequiredService<BookshelfService>();
            return ser.StoreAsync();
        }
    }
}
