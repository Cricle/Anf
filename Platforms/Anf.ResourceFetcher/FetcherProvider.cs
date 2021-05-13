using Anf.ResourceFetcher.Fetchers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Anf.ResourceFetcher
{
    public class FetcherProvider : IEnumerable<Type>
    {
        private static readonly string FetcherInterface = typeof(IResourceFetcher).FullName;

        private readonly List<Type> types=new List<Type>();

        public IReadOnlyCollection<Type> Types => types;

        public void Add(Type type)
        {
            if (type.GetInterface(FetcherInterface) is null)
            {
                throw new ArgumentException($"Input type must implement interface {FetcherInterface}");
            }
            types.Add(type);
        }

        public IEnumerator<Type> GetEnumerator()
        {
            return types.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public static FetcherProvider CreateDefault()
        {
            var provider= new FetcherProvider();
            var includeTypes = new Type[]
            {
                typeof(RedisFetcher),
                typeof(MongoFetcher),
                typeof(MssqlFetcher),
                typeof(RemoteFetcher)
            };
            foreach (var item in includeTypes)
            {
                provider.Add(item);
            }
            return provider;
        }
    }
}
