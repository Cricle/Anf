using Anf.WebService;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Anf.Web.Jobs
{
    internal class StoreBookshelfJob : ServicingJobBase
    {
        protected override Task OnExecute(IJobExecutionContext context, IServiceProvider serviceProvider)
        {
            var ser = serviceProvider.GetRequiredService<BookshelfService>();
            return ser.StoreAsync();
        }
    }
}
