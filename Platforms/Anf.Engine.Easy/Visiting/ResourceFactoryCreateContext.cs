using System;

namespace Anf.Easy.Visiting
{
    public class ResourceFactoryCreateContext<TResource>
    {
        public IComicVisiting<TResource> Visiting { get; internal set; }
        public string Address { get; internal set; }
        public IComicSourceProvider SourceProvider { get; internal set; }

        public IServiceProvider ServiceProvider => Visiting?.Host;
    }
}
