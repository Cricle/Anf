using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Anf
{
    public abstract class ComicSourceConditionBase<T> : IComicSourceCondition
        where T: IComicSourceProvider
    {
        protected ComicSourceConditionBase()
        {
            ProviderType = typeof(T);
            Descript = new EngineDescript();
        }

        public virtual int Order { get; }

        public Type ProviderType { get; }

        public abstract string EnginName { get; }

        public EngineDescript Descript { get; }

        public abstract Uri Address { get; }

        public virtual Uri FaviconAddress => new Uri(Path.Combine(Address.AbsoluteUri, "favicon.ico"));

        public abstract bool Condition(ComicSourceContext context);
    }
}
