using System;

namespace Anf
{
    public interface IComicSourceCondition
    {
        string EnginName { get; }

        int Order { get; }

        EngineDescript Descript { get; }

        Type ProviderType { get; }

        bool Condition(ComicSourceContext context);
    }
}
