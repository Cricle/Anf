namespace Anf.ChannelModel.Results
{
    /// <summary>
    /// 表示已知的结果代码
    /// </summary>
    public static class KnowResultCodes
    {
        /// <summary>
        /// 成功
        /// </summary>
        public const int Succeed = 0;
        /// <summary>
        /// 异常
        /// </summary>
        public const int Exception = 5001;
        /// <summary>
        /// 输入数据异常
        /// </summary>
        public const int InputError = 4001;
        /// <summary>
        /// 数据异常
        /// </summary>
        public const int DataError = 4002;
        /// <summary>
        /// 找不到数据
        /// </summary>
        public const int NotFound = 4003;
        /// <summary>
        /// 参数错误
        /// </summary>
        public const int ArgError = 4004;
        /// <summary>
        /// 已经存在了
        /// </summary>
        public const int AlreadyExist = 4005;
        /// <summary>
        /// 在忙
        /// </summary>
        public const int Busy = 1001;
    }
}
