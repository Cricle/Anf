using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace Anf.Engine
{
    public class ProposalEngine : ObservableCollection<IProposalDescription>
    {
        private static readonly string IProposalProviderTypeName = typeof(IProposalProvider).FullName;
        private readonly IServiceScopeFactory serviceScopeFactory;

        public IServiceScopeFactory ServiceScopeFactory => serviceScopeFactory;

        public ProposalEngine(IServiceScopeFactory serviceScopeFactory)
        {
            this.serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));
        }
        public ProposalProviderBox Active(Type type)
        {
            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }
            var scope = serviceScopeFactory.CreateScope();
            var engOrigin = scope.ServiceProvider.GetRequiredService(type);
            if (engOrigin is IProposalProvider eng)
            {
                return new ProposalProviderBox(eng, scope);
            }
            try
            {
                throw new InvalidCastException($"Can't case type {engOrigin.GetType().FullName} to {IProposalProviderTypeName}!");
            }
            finally
            {
                scope.Dispose();
            }
        }
        public ProposalProviderBox Active(int index)
        {
            if (index < 0 || index >= Count)
            {
                throw new ArgumentOutOfRangeException("The index out of range");
            }
            var type = this[index];
            return Active(type.ProviderType);
        }
    }
}
