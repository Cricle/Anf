using Anf.Engine;
using Anf.Engine.Annotations;
using System;

namespace Anf.KnowEngines.ProposalProviders
{
    [ProposalDescription]
    public class Dm5ProposalDescrition : IProposalDescription
    {
        public Type ProviderType { get; }= typeof(Dm5ProposalProvider);

        public string Name { get; } = "Dm5";

        public Uri DescritionUri { get; } = new Uri(Dm5ProposalProvider.Home);
    }
}
