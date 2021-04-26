using Microsoft.Extensions.DependencyInjection;
using System;

namespace Anf.Engine
{
    public readonly struct ProposalProviderBox : IDisposable
    {
        public readonly IProposalProvider Provider;
        public readonly IServiceScope Scope;

        public ProposalProviderBox(IProposalProvider provider, IServiceScope scope)
        {
            Provider = provider;
            Scope = scope;
        }

        public void Dispose()
        {
            Scope.Dispose();
        }
    }
}
