using System;
using System.Collections.Generic;
using System.Text;

namespace Kw.Comic.Engine
{
    public abstract class ComicSourceConditionBase<T> : IComicSourceCondition
    {
        protected ComicSourceConditionBase()
        {
            ProviderType = typeof(T);
            if (ProviderType.GetInterface(typeof(IComicSourceProvider).FullName)==null)
            {
                throw new InvalidOperationException($"Type {ProviderType} does not implement IComicSourceProvider");
            }
            Descript = new EngineDescript();
        }

        public virtual int Order { get; }

        public Type ProviderType { get; }

        public abstract string HttpName { get; }

        public abstract string ImageHttpName { get; }

        public abstract string EnginName { get; }

        public EngineDescript Descript { get; }

        public abstract bool Condition(ComicSourceContext context);
    }
}
