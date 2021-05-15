using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Anf.Web.Jobs
{
    /// <summary>
    /// 表示使用服务的基本任务
    /// </summary>
    public abstract class ServicingJobBase : IJob
    {
        /// <inheritdoc/>
        public async Task Execute(IJobExecutionContext context)
        {
            var provider = context.MergedJobDataMap.GetServiceProvider();
            if (provider == null)
            {
                throw new InvalidOperationException("MergedJobDataMap has not containes ServiceProvider object!");
            }
            try
            {
                var canExecute = await CanExecuteAsync(context, provider);
                if (canExecute)
                {
                    await OnExecute(context, provider);
                }
                else
                {
                    var logger = provider.GetRequiredService<ILogger<ServicingJobBase>>();
                    logger.LogTrace("The job {0} is skiped!", GetType().FullName);
                }
            }
            catch (Exception ex)
            {
                var logger = provider.GetRequiredService<ILogger<ServicingJobBase>>();
                logger.LogError(ex, "When in job {0}", GetType().FullName);
                throw;
            }
        }
        /// <summary>
        /// 执行任务
        /// </summary>
        /// <param name="context">任务执行上下文</param>
        /// <param name="serviceProvider">服务提供者</param>
        /// <returns></returns>
        protected abstract Task OnExecute(IJobExecutionContext context, IServiceProvider serviceProvider);
        /// <summary>
        /// 是否可以执行任务
        /// </summary>
        /// <param name="context"></param>
        /// <param name="serviceProvider"></param>
        /// <returns></returns>
        protected virtual Task<bool> CanExecuteAsync(IJobExecutionContext context, IServiceProvider serviceProvider)
        {
            return Task.FromResult(true);
        }
    }
}
