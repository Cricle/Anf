using Anf.Engine;
using System;

namespace Anf.Test
{
    internal class NullProposalDescription : IProposalDescription
    {
        public Type ProviderType { get; set; }

        public string Name { get; set; }

        public Uri DescritionUri { get; set; }
    }
}
