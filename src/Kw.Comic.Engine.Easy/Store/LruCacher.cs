using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kw.Comic.Engine.Easy.Store
{
    /// <summary>
    /// lru缓存器
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class LruCacher<TKey, TValue>
    {
        private readonly Dictionary<TKey, LinkedListNode<KeyValuePair<TKey, TValue>>> caches;
        private readonly LinkedList<KeyValuePair<TKey, TValue>> linkedList;
        private readonly object locker;
        /// <summary>
        /// 最大个数
        /// </summary>
        public int Max { get; }
        /// <summary>
        /// 同步根
        /// </summary>
        public object SyncRoot=>locker;
        /// <summary>
        /// 当前的个数
        /// </summary>
        public int Count => linkedList.Count;

        public IReadOnlyDictionary<TKey, TValue> Datas => linkedList.ToDictionary(x => x.Key, x => x.Value);

        public event Action<TKey, TValue> Removed;

        public LruCacher(int max)
        {
            if (max <= 0)
            {
                throw new RankException("最大值只能大于0");
            }
            locker = new object();
            Max = max;
            caches = new Dictionary<TKey, LinkedListNode<KeyValuePair<TKey, TValue>>>(max);
            linkedList = new LinkedList<KeyValuePair<TKey, TValue>>();
        }
        /// <summary>
        /// 清除当前的缓存
        /// </summary>
        public void Clear()
        {
            lock (locker)
            {
                caches.Clear();
                linkedList.Clear();
            }
        }
        /// <summary>
        /// 移除缓存
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <returns></returns>
        public bool Remove(TKey key, out TValue value)
        {
            if (caches.ContainsKey(key))
            {
                lock (locker)
                {
                    var val = caches[key];
                    linkedList.Remove(val);
                    value = val.Value.Value;
                    var ok= caches.Remove(key);
                    if (ok)
                    {
                        Removed?.Invoke(key, value);
                    }
                    return ok;
                }
            }
            value = default;
            return false;
        }
        /// <summary>
        /// 获取某一项，如跟失败返回默认值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public virtual TValue Get(TKey key)
        {
            if (caches.TryGetValue(key, out var value))
            {
                lock (locker)
                {
                    linkedList.Remove(value);
                    linkedList.AddLast(value);
                }
                return GetValue(key, ref value);
            }
            return default(TValue);
        }
        public virtual bool TryGetValue(TKey key,out TValue value)
        {
            value = Get(key);
            return value != null;
        }
        public virtual TValue GetOrAdd(TKey key, Func<TValue> creator)
        {
            lock (locker)
            {
                if (caches.TryGetValue(key, out var value))
                {
                    linkedList.Remove(value);
                    linkedList.AddLast(value);
                    return GetValue(key, ref value);
                }
                var val = creator();
                UnsafeAdd(key, val);
                return val;
            }
        }
        protected virtual TValue GetValue(TKey key, ref LinkedListNode<KeyValuePair<TKey, TValue>> node)
        {
            return node.Value.Value;
        }
        /// <summary>
        /// 键是否在缓存中
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool ContainsKey(TKey key)
        {
            lock (locker)
            {
                return caches.ContainsKey(key);
            }
        }
        private void UnsafeAdd(TKey key, TValue value)
        {

            if (caches.ContainsKey(key))
            {
                var cacheEntity = caches[key];
                linkedList.Remove(cacheEntity);
                linkedList.AddLast(cacheEntity);
                return;
            }
            if (linkedList.Count >= Max)
            {
                var fs = linkedList.First;
                linkedList.RemoveFirst();
                caches.Remove(fs.Value.Key);
            }
            var par = new KeyValuePair<TKey, TValue>(
                    key, value);
            var val = new LinkedListNode<KeyValuePair<TKey, TValue>>(par);
            caches.Add(key, val);
            linkedList.AddLast(val);
        }
        /// <summary>
        /// 添加一项
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Add(TKey key, TValue value)
        {
            lock (locker)
            {
                UnsafeAdd(key, value);
            }
        }
    }
}
