using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Kw.Comic.Engine.Easy.Visiting
{
    public abstract class BlockSlots<TValue> : IDisposable
    {
        private readonly SemaphoreSlim[] slims;
        private readonly TValue[] values;

        public int Size { get; }

        protected BlockSlots(int size)
        {
            Size = size;
            slims = new SemaphoreSlim[size];
            for (int i = 0; i < slims.Length; i++)
            {
                slims[i] = new SemaphoreSlim(1);
            }
            values = new TValue[size];
        }

        public event Action<BlockSlots<TValue>, int, TValue> PageLoaded;

        public virtual void Dispose()
        {
#if NETSTANDARD1_4
            for (int i = 0; i < slims.Length; i++)
			{
                var s=slims[i];
                s.Wait();
                s.Dispose();
			}
#else
            slims.AsParallel().ForAll(x =>
            {
                x.Wait();
                x.Dispose();
            });
#endif
            DisposeValues();
        }
        protected virtual void DisposeValues()
        {

            for (int i = 0; i < values.Length; i++)
            {
                var page = values[i];
                if (page is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }
        }

        public async Task<TValue> GetAsync(int index)
        {
            if (index < 0 || index >= slims.Length)
            {
                return default;
            }
            var sem = slims[index];
            var page = values[index];
            if (page != null)
            {
                return page;
            }
            await sem.WaitAsync();
            try
            {
                page = values[index];
                if (page != null)
                {
                    return page;
                }
                page = values[index] = await OnLoadAsync(index);
                PageLoaded?.Invoke(this, index, page);
                return page;
            }
            finally
            {
                sem.Release();
            }
        }
        protected abstract Task<TValue> OnLoadAsync(int index);
    }
}
