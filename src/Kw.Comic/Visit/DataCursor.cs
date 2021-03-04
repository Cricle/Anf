using Kw.Core.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Kw.Comic.Visit
{
    public class DataCursor<T> : ViewModelBase, IDisposable
    {
        private int index = -1;

        public DataCursor(IReadOnlyList<T> datas)
        {
            Datas = datas;
            Length = Datas.Count;
        }
        public DataCursor(IEnumerable<T> datas)
        {
            if (datas is null)
            {
                throw new ArgumentNullException(nameof(datas));
            }

            Datas = datas.ToArray();
            Length = Datas.Count;
        }

        private T current;
        private bool waitLoad;

        public bool WaitLoad
        {
            get { return waitLoad; }
            set => RaisePropertyChanged(ref waitLoad, value);
        }

        public IReadOnlyList<T> Datas { get; }

        public int Length { get; }

        public int Index
        {
            get => index;
            private set
            {
                RaisePropertyChanged(ref index, value);
                IndexChanged?.Invoke(this, value);
            }
        }

        public bool IsFirst => index == 0;

        public bool IsLast => index == Datas.Count - 1;

        public T Current
        {
            get => current;
            private set
            {
                RaisePropertyChanged(ref current, value);
                CurrentChanged?.Invoke(this, value);
            }
        }

        public event Action<DataCursor<T>, int> IndexChanged;
        public event Action<DataCursor<T>, T> CurrentChanged;
        public event Action<DataCursor<T>, T> ResourceLoading;
        public event Action<DataCursor<T>, T> ResourceLoaded;

        public T this[int idx]
        {
            get => Datas[idx];
        }

        public virtual void Dispose()
        {
        }

        public async Task<bool> SetIndexAsync(int idx)
        {
            if (idx < 0 || idx >= Datas.Count)
            {
                return false;
            }
            if (idx == index)
            {
                return true;
            }
            var i = index;
            var waitLoad = WaitLoad;
            var task = LoadIndexAsync(idx);
            if (waitLoad)
            {
                await task;
            }
            if (Interlocked.CompareExchange(ref index, idx, i) == i)
            {
                Index = idx;
                Current = this[idx];
                return true;
            }
            return false;
        }
        protected void RaiseResourceLoading(T val)
        {
            ResourceLoading?.Invoke(this, val);
        }
        protected void RaiseResourceLoaded(T val)
        {
            ResourceLoaded?.Invoke(this, val);
        }
        public virtual Task LoadIndexAsync(int index)
        {
#if NET452
            return Task.FromResult(0);
#else
            return Task.CompletedTask;
#endif
        }
    }
}
