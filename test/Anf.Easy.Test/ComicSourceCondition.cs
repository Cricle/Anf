using System;

namespace Anf.Easy.Test
{
    class ComicSourceCondition : IComicSourceCondition
    {
        public int Order { get; set; }

        public EngineDescript Descript { get; set; }

        public Type ProviderType { get; set; } = typeof(ComicSourceProvider);

        public Uri Address { get; set; } = new Uri("http://any.com");

        public Uri FaviconAddress { get; set; } = new Uri("http://any.com");

        public string EngineName { get; set; } = "testing";

        public bool Condition(ComicSourceContext context)
        {
            return true;
        }
    }
}
