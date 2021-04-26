using System;

namespace Anf
{
    public interface IComicSourceCondition: IEngine
    {
        int Order { get; }

        EngineDescript Descript { get; }

        Type ProviderType { get; }

        Uri Address { get; }

        Uri FaviconAddress { get; }

        bool Condition(ComicSourceContext context);
    }
}
