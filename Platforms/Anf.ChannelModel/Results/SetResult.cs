using System;
using System.Collections.Generic;
using System.Linq;

namespace Anf.ChannelModel.Results
{
    /// <summary>
    /// 表示实体集合结果
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EntitySetResult<T> : Result
    {
        /// <summary>
        /// 获取或设置一个值，表示附带的集合
        /// </summary>
        public T Datas { get; set; }
        /// <summary>
        /// 获取或设置一个值，表示数据的总数
        /// </summary>
        public long Total { get; set; }
        /// <summary>
        /// 获取或设置一个值，表示跳过的个数
        /// </summary>
        public int? Skip { get; set; }
        /// <summary>
        /// 获取或设置一个值，表示获取的个数
        /// </summary>
        public int? Take { get; set; }
    }
    /// <summary>
    /// 表示集合结果
    /// </summary>
    /// <typeparam name="T">项类型</typeparam>
    public class SetResult<T> : EntitySetResult<IEnumerable<T>>
    {
    }
}
