using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Quartz;
using Quartz.Spi;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Anf.Web
{
    public class SingletonJobFactory : IJobFactory, ISingletonJobFactory
    {
        private readonly IServiceScopeFactory serviceScopeFactory;
        private readonly ConcurrentDictionary<IJob, IServiceScope> serviceScopes = new ConcurrentDictionary<IJob, IServiceScope>();
        /// <inheritdoc/>
        public ISchedulerFactory SchedulerFactory { get; }
        /// <summary>
        /// 初始化类型<see cref="SingletonJobFactory"/>
        /// </summary>
        /// <param name="serviceScopeFactory">区域服务工厂</param>
        public SingletonJobFactory(IServiceScopeFactory serviceScopeFactory,
            ISchedulerFactory schedulerFactory)
        {
            this.SchedulerFactory = schedulerFactory ?? throw new ArgumentNullException(nameof(schedulerFactory));
            this.serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));
        }

        /// <inheritdoc/>
        public virtual async Task<IScheduler> GetSchedulerAsync()
        {
            var scheduler = await SchedulerFactory.GetScheduler();
            scheduler.JobFactory = this;
            return scheduler;
        }

        /// <inheritdoc/>
        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            var scope = serviceScopeFactory.CreateScope();
            bundle.JobDetail.JobDataMap.Put(QuartzConst.ServiceProviderKey, scope.ServiceProvider);
            bundle.JobDetail.JobDataMap.Put(QuartzConst.ServiceScopeFactoryKey, serviceScopeFactory);
            bundle.JobDetail.JobDataMap.Put(QuartzConst.SchedulerKey, scheduler);
            var job = scope.ServiceProvider.GetRequiredService(bundle.JobDetail.JobType) as IJob;
            serviceScopes.TryAdd(job, scope);
            OnNewJob(bundle, scheduler, job);
            return job;
        }
        /// <summary>
        /// 准备任务
        /// </summary>
        /// <param name="bundle">触发器包</param>
        /// <param name="scheduler">计划器</param>
        /// <param name="job">目标任务</param>
        protected virtual void OnNewJob(TriggerFiredBundle bundle, IScheduler scheduler, IJob job)
        {

        }

        /// <inheritdoc/>
        public void ReturnJob(IJob job)
        {
            if (serviceScopes.TryRemove(job, out var scope))
            {
                OnReturnJob(job);
                scope.Dispose();
            }
        }
        /// <summary>
        /// 归还任务
        /// </summary>
        /// <param name="job">目标任务</param>
        protected virtual void OnReturnJob(IJob job)
        {

        }
    }
}
