namespace Kw.Comic.Engine.Easy.Visiting
{
    public class ResourceFactoryCreateContext
    {
        public IComicVisiting Visiting { get; internal set; }
        public string Address { get; internal set; }
        public IComicSourceProvider SourceProvider { get; internal set; }
    }
}
