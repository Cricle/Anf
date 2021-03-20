using Kw.Comic.Engine.Easy;
using Kw.Comic.Engine.Easy.Visiting;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KwC.Services
{
    public class VisitService : IReadOnlyDictionary<string, VisitingBox>
    {
        private readonly IServiceProvider host;
        private readonly LruCacher<string, VisitingBox> visitingMap;

        public IEnumerable<string> Keys => visitingMap.Datas.Keys;

        public IEnumerable<VisitingBox> Values => visitingMap.Datas.Values;

        public int Count => throw new NotImplementedException();

        public VisitingBox this[string key]
        {
            get => visitingMap.Get(key);
        }

        public VisitService(IServiceProvider host, int cacheSize = 50)
        {
            this.host = host;
            visitingMap = new LruCacher<string, VisitingBox>(cacheSize);
        }

        public async Task<VisitingBox> GetVisitingAsync(string address)
        {
            var val = visitingMap.GetOrAdd(address, () => new VisitingBox(address, host.CreateStoreVisitor()));
            if (val != null)
            {
                await val.LoadAsync();
            }
            return null;
        }
        public bool Contains(string connectId)
        {
            return visitingMap.ContainsKey(connectId);
        }
        public void Clear()
        {
            foreach (var item in visitingMap.Datas)
            {
                Remove(item.Key);
            }
        }
        public bool Remove(string address)
        {
            var ok = visitingMap.Remove(address, out var val);
            if (ok)
            {
                val.Visiting.Dispose();
            }
            return ok;
        }

        public bool ContainsKey(string key)
        {
            return visitingMap.ContainsKey(key);
        }

        public bool TryGetValue(string key, out VisitingBox value)
        {
            value = visitingMap.Get(key);
            return value != null;
        }

        public IEnumerator<KeyValuePair<string, VisitingBox>> GetEnumerator()
        {
            return visitingMap.Datas.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
