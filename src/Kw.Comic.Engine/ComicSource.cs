namespace Kw.Comic.Engine
{
    /// <summary>
    /// 漫画源
    /// </summary>
    public class ComicSource : ComicRef
    {
        public string Name { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
