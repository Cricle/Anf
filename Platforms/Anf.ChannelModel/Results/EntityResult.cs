namespace Anf.ChannelModel.Results
{
    /// <summary>
    /// 表示实体结果
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EntityResult<T> : Result
    {
        /// <summary>
        /// 获取或设置一个值，表示附带的实体
        /// </summary>
        public T Data { get; set; }
    }
}
