using Quartz;
using Quartz.Spi;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Anf.Web
{
    /// <summary>
    /// 表示单一的任务工厂
    /// </summary>
    public interface ISingletonJobFactory: IJobFactory
    {
        /// <summary>
        /// 计划工厂
        /// </summary>
        ISchedulerFactory SchedulerFactory { get; }
        /// <summary>
        /// 获取计划器
        /// </summary>
        /// <returns></returns>
        Task<IScheduler> GetSchedulerAsync();
    }
}
