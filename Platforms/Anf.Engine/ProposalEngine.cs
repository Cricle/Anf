using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace Anf.Engine
{
    public class ProposalEngine : ObservableCollection<Type>
    {
        private readonly IServiceScopeFactory serviceScopeFactory;

        public ProposalEngine(IServiceScopeFactory serviceScopeFactory)
        {
            this.serviceScopeFactory = serviceScopeFactory;
        }

        public ProposalProviderBox Active(int index)
        {
            var type = this[index];
            var scope = serviceScopeFactory.CreateScope();
            var eng = (IProposalProvider)scope.ServiceProvider.GetRequiredService(type);
            return new ProposalProviderBox(eng, scope);
        }
    }
}
