using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Kw.Comic.Visit
{
    public class SimpleCacher<TKey,TValue> : ISimpleCacher<TKey, TValue>
    {
        private readonly Dictionary<TKey, TValue> slots;

        private int? max;

        public SimpleCacher(int max = 5)
        {
            Max = max;
            slots = new Dictionary<TKey, TValue>(max);
            LockSlim = new ReaderWriterLockSlim();
        }

        public int? Max
        {
            get { return max; }
            set
            {
                if (value != null && value <= 0)
                {
                    throw new ArgumentException("Max must null or more than zero!");
                }
                max = value;
                MaxChanged?.Invoke(this, value);
            }
        }

        public ReaderWriterLockSlim LockSlim { get; }

        public bool SwitchDisposable { get; set; } = true;

        public event Action<SimpleCacher<TKey, TValue>, int?> MaxChanged;

        public void Dispose()
        {
            Reset();
            LockSlim.Dispose();
        }

        public TValue GetCache(TKey i)
        {
            LockSlim.EnterReadLock();
            try
            {
                if (slots.TryGetValue(i, out var cur))
                {
                    return cur;
                }
                return default;
            }
            finally
            {
                LockSlim.ExitReadLock();
            }
        }

        public void Reset()
        {
            LockSlim.EnterWriteLock();
            try
            {
                if (SwitchDisposable)
                {

                    foreach (var item in slots)
                    {
                        OnSwitch(item.Key,item.Value);
                    }
                }
                slots.Clear();
            }
            finally
            {
                LockSlim.ExitWriteLock();
            }
        }

        public void SetCache(TKey i, TValue cursor)
        {
            LockSlim.EnterWriteLock();
            try
            {
                if (slots.TryGetValue(i,out var val))
                {
                    if (SwitchDisposable)
                    {
                        OnSwitch(i, val);
                    }
                    slots[i] = cursor;
                }
                else
                {

                    if (max != null && slots.Count > max)
                    {
                        var pair= slots.First();
                        OnSwitch(pair.Key, pair.Value);
                        slots.Remove(pair.Key);
                    }
                    slots.Add(i, cursor);
                }
            }
            finally
            {
                LockSlim.ExitWriteLock();
            }
        }
        protected virtual void OnSwitch(TKey key,TValue value)
        {
            if (SwitchDisposable)
            {
                if (value is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }
        }
    }
}
