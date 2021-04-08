using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kw.Comic.Engine
{
    public class SearchEngine : List<Type>
    {
        public IServiceScopeFactory ServiceScopeFactory { get; }

        public SearchEngine(IServiceScopeFactory serviceScopeFactory)
        {
            ServiceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));
        }
    }
}
