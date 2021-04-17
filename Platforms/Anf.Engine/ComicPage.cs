namespace Anf
{
#if INERNAL_INFO
    internal
#else
    public
#endif
     class ComicPage : ComicRef
    {
        public string Name { get; set; }
    }
}
