using Anf.Easy.Visiting;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Anf.Easy.Test.Visiting
{
    internal class ValueObjectUsingPool : ObjectUsingPool<int, object>
    {
        public ConcurrentDictionary<int, object> Values { get; set; } = new ConcurrentDictionary<int, object>();
        protected override async Task<object> CreateValueAsync(int key)
        {
            await Task.Yield();
            return Values.GetOrAdd(key, _ => new object());
        }
    }
}
