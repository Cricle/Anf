using System;
using System.Collections.Generic;
using System.Text;
using Anf.Web;
using Microsoft.Extensions.DependencyInjection;

namespace Quartz
{
    /// <summary>
    /// 对类型<see cref="JobDataMap"/>的扩展
    /// </summary>
    public static class JobMapExtensions
    {
        public static T GetAs<T>(this JobDataMap map,string key)
        {
            if (map is null)
            {
                throw new ArgumentNullException(nameof(map));
            }

            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException($"“{nameof(key)}”不能是 Null 或为空。", nameof(key));
            }

            if (map.TryGetValue(key, out var oprovider)
                && oprovider is T provider)
            {
                return provider;
            }
            return default;
        }
        /// <summary>
        /// 从任务数据图中获取类型<see cref="IServiceProvider"/>
        /// </summary>
        /// <param name="map">任务数据图</param>
        /// <returns></returns>
        public static IServiceProvider GetServiceProvider(this JobDataMap map)
        {
            return map.GetAs<IServiceProvider>(QuartzConst.ServiceProviderKey);
        }
        /// <summary>
        /// 从任务数据图中获取类型<see cref="IServiceScopeFactory"/>
        /// </summary>
        /// <param name="map">任务数据图</param>
        /// <returns></returns>
        public static IServiceScopeFactory GetServiceScopeFactory(this JobDataMap map)
        {
            return map.GetAs<IServiceScopeFactory>(QuartzConst.ServiceScopeFactoryKey);
        }
        /// <summary>
        /// 从任务数据图中获取类型<see cref="IScheduler"/>
        /// </summary>
        /// <param name="map">任务数据图</param>
        /// <returns></returns>
        public static IScheduler GetScheduler(this JobDataMap map)
        {
            return map.GetAs<IScheduler>(QuartzConst.SchedulerKey);
        }
    }
}
