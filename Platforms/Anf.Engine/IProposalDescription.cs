using System;

namespace Anf.Engine
{
    public interface IProposalDescription
    {
        Type ProviderType { get; }

        string Name { get; }

        Uri DescritionUri { get; }
    }
}
