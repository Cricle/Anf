using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections;
#if NETSTANDARD2_0
using System.Buffers;
#endif

namespace Kw.Comic.Engine.Easy.Visiting
{
    public abstract class BlockSlots<TValue> : IDisposable
        where TValue : class
    {
        private readonly Task<TValue>[] valueTasks;
        private readonly TValue[] values;

        public int Size { get; }

        public TValue this[int index]
        {
            get
            {
                if (index >= Size || index < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(index));
                }
                return values[index];
            }
        }

        protected BlockSlots(int size)
        {
            Size = size;
#if NETSTANDARD2_0
            valueTasks = ArrayPool<Task<TValue>>.Shared.Rent(size);
            values = ArrayPool<TValue>.Shared.Rent(size);
#else
            valueTasks = new Task<TValue>[size];
            values = new TValue[size];
#endif
        }

        public event Action<BlockSlots<TValue>, int, TValue> PageLoaded;

        ~BlockSlots()
        {
            Dispose();
        }

        public virtual void Dispose()
        {
            try
            {
                for (int i = 0; i < Size; i++)
                {
                    var page = values[i];
                    if (page is IDisposable disposable)
                    {
                        disposable.Dispose();
                    }
                }
            }
            finally
            {
#if NETSTANDARD2_0
                ArrayPool<Task<TValue>>.Shared.Return(valueTasks, true);
                ArrayPool<TValue>.Shared.Return(values, true);
#endif
                GC.SuppressFinalize(this);
            }
        }
        public async Task<TValue> GetAsync(int index)
        {
            if (index < 0 || index >= Size)
            {
                return default;
            }
            var page = values[index];
            if (!(page is null))
            {
                return page;
            }
            if (Interlocked.CompareExchange(ref valueTasks[index], null, null) is null)
            {
                var task = valueTasks[index] = OnLoadAsync(index);
                var v = values[index] = await task;
                PageLoaded?.Invoke(this, index, v);
                return v;
            }

            var tsk = valueTasks[index];
            if (tsk.IsCompleted)
            {
                return tsk.Result;
            }
            return await tsk;
        }
        protected abstract Task<TValue> OnLoadAsync(int index);
    }
}
