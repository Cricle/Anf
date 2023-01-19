using Anf.Easy.Visiting;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Anf.Easy.Test.Visiting
{
    internal class DisposableObjectUsingPool : ObjectUsingPool<int, DispoableObject>
    {
        public TimeSpan WaitTime { get; set; }
        public ConcurrentDictionary<int, DispoableObject> Values { get; set; } = new ConcurrentDictionary<int, DispoableObject>();
        protected override async Task<DispoableObject> CreateValueAsync(int key)
        {
            await Task.Delay(WaitTime);
            return Values.GetOrAdd(key, _ => new DispoableObject());
        }
    }
}
