using Anf.Engine;
using Anf.Engine.Annotations;
using System;

namespace Anf.KnowEngines.ProposalProviders
{
    [ProposalDescription]
    public class BilibiliProposalDescription : IProposalDescription
    {
        public Type ProviderType { get; } = typeof(BilibiliProposalProvider);

        public string Name { get; } = "Bilibili";

        public Uri DescritionUri { get; } = new Uri("https://manga.bilibili.com/");
    }
}
