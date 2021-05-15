using Anf.ResourceFetcher.Fetchers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Anf.ResourceFetcher
{
    public class FetcherProvider : IEnumerable<Type>
    {
        private static readonly string FetcherInterface = typeof(ISingleResourceFetcher).FullName;

        private readonly List<Type> types;

        public FetcherProvider()
        {
            types = new List<Type>();
        }
        public FetcherProvider(IEnumerable<Type> types)
        {
            this.types = new List<Type>(types);
        }

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
    }
}
