using System;
using System.Collections.Generic;
using System.Text;

namespace Anf
{
    public abstract class ComicSourceConditionBase<T> : IComicSourceCondition
    {
        protected ComicSourceConditionBase()
        {
            ProviderType = typeof(T);
#if !NETSTANDARD1_3
            if (ProviderType.GetInterface(typeof(IComicSourceProvider).FullName)==null)
            {
                throw new InvalidOperationException($"Type {ProviderType} does not implement IComicSourceProvider");
            }
#endif
            Descript = new EngineDescript();
        }

        public virtual int Order { get; }

        public Type ProviderType { get; }

        public abstract string EnginName { get; }

        public EngineDescript Descript { get; }

        public abstract bool Condition(ComicSourceContext context);
    }
}
