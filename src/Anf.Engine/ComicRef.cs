namespace Anf
{
#if INERNAL_INFO
    internal
#else
    public
#endif
     abstract class ComicRef
    {
        /// <summary>
        /// 目标地址
        /// </summary>
        public string TargetUrl { get; set; }
    }
}
