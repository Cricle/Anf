using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Kw.Comic.Engine.Easy.Concurrnets
{
    public class ThreadSafeList<T> : IList<T>
    {
        private readonly List<T> list;
        private readonly object locker;

        public ThreadSafeList()
        {
            locker = new object();
            list = new List<T>();
        }

        public T this[int index]
        {
            get => list[index];
            set
            {
                lock (locker)
                {
                    list[index] = value;
                }
            }
        }

        public int Count => list.Count;

        public bool IsReadOnly => false;

        public void Add(T item)
        {
            lock (locker)
            {
                list.Add(item);
            }
        }
        public void AddRange(T[] items)
        {
            lock (locker)
            {
                list.AddRange(items);
            }
        }

        public void Clear()
        {
            lock (locker)
            {
                list.Clear();
            }
        }

        public bool Contains(T item)
        {
            lock (locker)
            {
                return list.Contains(item);
            }
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            lock (locker)
            {
                list.CopyTo(array, arrayIndex);
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            lock (locker)
            {
                return list.GetEnumerator();
            }
        }

        public int IndexOf(T item)
        {
            lock (locker)
            {
                return list.IndexOf(item);
            }
        }

        public void Insert(int index, T item)
        {
            lock (locker)
            {
                list.Add(item);
            }
        }

        public bool Remove(T item)
        {
            lock (locker)
            {
                return list.Remove(item);
            }
        }

        public void RemoveAt(int index)
        {
            lock (locker)
            {
                list.RemoveAt(index);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
