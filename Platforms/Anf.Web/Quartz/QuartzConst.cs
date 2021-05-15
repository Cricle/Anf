using System;
using System.Collections.Generic;
using System.Text;

namespace Anf.Web
{
    /// <summary>
    /// 表示Quartz模块的常数
    /// </summary>
    public abstract class QuartzConst
    {
        /// <summary>
        /// 表示范围范围工厂的键
        /// </summary>
        public const string ServiceScopeFactoryKey = "Quartz.ServiceScopeFactory";
        /// <summary>
        /// 表示服务提供者的键
        /// </summary>
        public const string ServiceProviderKey = "Quartz.ServiceProvider";
        /// <summary>
        /// 表示此任务的计划器的键
        /// </summary>
        public const string SchedulerKey = "Quartz.Scheduler";
    }
}
