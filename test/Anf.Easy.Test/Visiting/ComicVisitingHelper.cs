using Anf.Easy.Test.Provider;
using Anf.Easy.Visiting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;

namespace Anf.Easy.Test.Visiting
{
    internal static class ComicVisitingHelper
    {
        public static readonly Uri AnyUri = new Uri("http://localhost:8765/");
        public static ComicVisiting<Stream> CreateResrouceVisitor()
        {
            var creator = new StreamResourceFactoryCreator();
            var eng = new ComicEngine();
            eng.Add(new ResourceComicCondition());
            var prov = new ResourceComicProvider();
            var provider = new ValueServiceProvider
            {
                ServiceMap = new Dictionary<Type, Func<object>>
                {
                    [typeof(ComicEngine)] = () => eng,
                    [typeof(ResourceComicProvider)] = () => prov,

                }
            };
            provider.ServiceMap.Add(typeof(IServiceScopeFactory), () => new ValueServiceScopeFactory { ScopeFactory = () => new ValueServiceScope { ServiceProvider = provider } });
            var visit = new ComicVisiting<Stream>(provider, creator);
            return visit;
        }
    }
}
