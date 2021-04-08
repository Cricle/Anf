namespace Anf
{
    public class ComicSnapshot : ComicRef
    {
        /// <summary>
        /// 漫画名字
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 作者
        /// </summary>
        public string Author { get; set; }
        /// <summary>
        /// 图片资源
        /// </summary>
        public string ImageUri { get; set; }
        /// <summary>
        /// 资源集合
        /// </summary>
        public ComicSource[] Sources { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Descript { get; set; }
    }
}
