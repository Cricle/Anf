using System;
using System.Collections.Generic;
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

        public abstract bool Condition(ComicSourceContext context);
    }
}
