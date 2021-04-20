using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Anf.Easy.Visiting
{
    public abstract class ObjectUsingPool<TKey, TValue> : IDisposable
    {
        class ValueBox
        {
            private int refCount;

            public ValueBox(Task<TValue> task)
            {
                ValueTask = task;
            }

            public int RefCount => Volatile.Read(ref refCount);

            public bool IsUsing => RefCount != 0;

            public Task<TValue> ValueTask { get; }

            public void Use()
            {
                Interlocked.Increment(ref refCount);
            }
            public void UnUse()
            {
                Interlocked.Decrement(ref refCount);
            }
        }
        private readonly ConcurrentDictionary<TKey, ValueBox> valueMap;

        protected ObjectUsingPool()
        {
            this.valueMap = new ConcurrentDictionary<TKey, ValueBox>();
        }

        public int Count => valueMap.Count;
        public ICollection<TKey> Keys => valueMap.Keys;

        public bool ContainsKey(TKey key)
        {
            return valueMap.ContainsKey(key);
        }

        public Task<TValue> GetAsync(TKey key)
        {
            var locker = valueMap.GetOrAdd(key, k => new ValueBox(CreateValueAsync(k)));
            locker.Use();
            return locker.ValueTask;
        }
        public int? GetUseCount(TKey key)
        {
            if (valueMap.TryGetValue(key,out var box))
            {
                return box.RefCount;
            }
            return null;
        }
        private void DetchBox(TKey key,ValueBox box)
        {
            if (box.ValueTask.IsCompleted)
            {
                if (box.ValueTask.Result is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }
            else
            {
                box.ValueTask.ContinueWith(x =>
                {
                    if (x.Result is IDisposable disposable)
                    {
                        disposable.Dispose();
                    }
                });
            }
            valueMap.TryRemove(key, out _);
        }
        public void Return(TKey key)
        {
            if (valueMap.TryGetValue(key, out var box))
            {
                box.UnUse();
                if (!box.IsUsing)
                {
                    DetchBox(key, box);
                }
            }
        }

        protected abstract Task<TValue> CreateValueAsync(TKey key);

        public void Dispose()
        {
            foreach (var item in valueMap.ToArray())
            {
                DetchBox(item.Key, item.Value);
            }
        }
    }
}
